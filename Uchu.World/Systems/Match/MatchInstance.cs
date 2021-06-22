using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using InfectedRose.Lvl;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Systems.Match
{
    public class MatchInstance
    {
        /// <summary>
        /// Event for the match time expiring.
        /// </summary>
        public Event TimeEnded { get; } = new Event();

        /// <summary>
        /// Time to wait before starting if not all players press "Play!".
        /// </summary>
        private int _waitTime;

        /// <summary>
        /// Time to set before starting if all players press "Play!".
        /// </summary>
        private int _playWaitTime;
        
        /// <summary>
        /// Players that are required to start the match.
        /// </summary>
        private int _requiredPlayers;
        
        /// <summary>
        /// Max players in the match.
        /// </summary>
        private int _maxPlayers;
        
        /// <summary>
        /// Players currently in the match.
        /// </summary>
        private List<Player> _players = new List<Player>();
        
        /// <summary>
        /// Players currently ready for the match.
        /// </summary>
        private List<Player> _readyPlayers = new List<Player>();
        
        /// <summary>
        /// Players that were sent the time.
        /// Sending the time multiple times causes
        /// problems on the client.
        /// </summary>
        private List<Player> _playersSentTime = new List<Player>();

        /// <summary>
        /// Timer for starting the round.
        /// </summary>
        private Timer _countdown = new Timer()
        {
            AutoReset = false,
        };

        /// <summary>
        /// Stopwatch for the elapsed time.
        /// </summary>
        private Stopwatch _countdownStopwatch = new Stopwatch();
        
        /// <summary>
        /// Creates an instance of a match.
        /// </summary>
        /// <param name="type">Activity id of the match.</param>
        public MatchInstance(int type, Zone zone)
        {
            // Load the match data.
            var matchData = ClientCache.GetTable<Activities>().First(activity => activity.ActivityID == type);
            var matchZoneId =  matchData.InstanceMapID ?? 0;
            var matchCurrencyLot = matchData.OptionalCostLOT;
            var matchCurrencyCount = matchData.OptionalCostCount;
            _waitTime = matchData.WaitTime ?? 60000;
            _playWaitTime = matchData.StartDelay ?? 5000;
            _requiredPlayers = (matchData.MinTeams ?? 1) * (matchData.MinTeamSize ?? 1);
            _maxPlayers = (matchData.MaxTeams ?? 1) * (matchData.MaxTeamSize ?? 1);
            
            // Connect the timer ending.
            _countdown.Elapsed += async (sender, args) =>
            {
                // Remove the round from the provisioner.
                TimeEnded.Invoke();
                
                // Take the optional currency from the players.
                if (matchCurrencyLot.HasValue && matchCurrencyCount.HasValue)
                {
                    foreach (var player in _players)
                    {
                        if (!player.TryGetComponent<InventoryManagerComponent>(out var inventoryManager)) continue;
                        await inventoryManager.RemoveLotAsync(matchCurrencyLot.Value,(uint) matchCurrencyCount.Value);
                    }
                }
                
                // Allocate the new zone.
                InstanceInfo allocatedInstance;
                try
                {
                    allocatedInstance = await ServerHelper.RequestNewWorldServerAsync(zone.Server, (ZoneId) matchZoneId);
                    if (allocatedInstance == default)
                    {
                        Logger.Debug($"Could not find server for: {matchZoneId}");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return;
                }

                // Start the match.
                foreach (var player in _players)
                {
                    await player.SendToWorldAsync(allocatedInstance, (ZoneId) matchZoneId);
                }
            };
        }

        /// <summary>
        /// Updates the state of the timer.
        /// </summary>
        private void UpdateTimer()
        {
            // Update the countdown.
            if (_players.Count >= _requiredPlayers)
            {
                var remainingTimeChanged = false;
                if (!_countdown.Enabled)
                {
                    // Send the initial time.
                    remainingTimeChanged = true;
                    _countdown.Interval = _waitTime;
                    _countdown.Start();
                    _countdownStopwatch.Start();
                }
                else if (_players.Count == _readyPlayers.Count && _countdown.Interval - _countdownStopwatch.ElapsedMilliseconds > _playWaitTime)
                {
                    // Set the time to 5 seconds to prepare the round.
                    remainingTimeChanged = true;
                    _countdown.Stop();
                    _countdownStopwatch.Reset();
                    _countdown.Interval = _playWaitTime;
                    _countdown.Start();
                    _countdownStopwatch.Start();
                }
                
                // Send the clients the updated time.
                foreach (var player in _players)
                {
                    if (!_playersSentTime.Contains(player))
                    {
                        _playersSentTime.Add(player);
                        player.Message(new MatchUpdateMessage()
                        {
                            Associate = player,
                            Data = new LegoDataDictionary
                            {
                                {"time", (_countdown.Interval - _countdownStopwatch.ElapsedMilliseconds)/1000.0, 3},
                            },
                            Type = MatchUpdateType.SetInitialTime,
                        });
                    }
                    else if (remainingTimeChanged)
                    {
                        player.Message(new MatchUpdateMessage()
                        {
                            Associate = player,
                            Data = new LegoDataDictionary
                            {
                                {"time", (_countdown.Interval - _countdownStopwatch.ElapsedMilliseconds)/1000.0, 3},
                            },
                            Type = MatchUpdateType.SetTime,
                        });
                    }
                }
            }
            else
            {
                // Stop the time.
                _countdown.Stop();
                _countdownStopwatch.Reset();
                // TODO: What is used to disable the time?
            }
        }

        /// <summary>
        /// Returns if a player can be added to the round.
        /// </summary>
        /// <param name="player">Player to add.</param>
        /// <returns>If the player can be added.</returns>
        public bool CanAddPlayer(Player player)
        {
            return _players.Count < _maxPlayers;
        }
        
        /// <summary>
        /// Returns a player is in the match.
        /// </summary>
        /// <param name="player">Player to check.</param>
        /// <returns>If the player is in the round.</returns>
        public bool HasPlayer(Player player)
        {
            return _players.Contains(player);
        }

        /// <summary>
        /// Adds a player to the match.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void AddPlayer(Player player)
        {
            // Send the response.
            player.Message(new MatchResponseMessage()
            {
                Associate = player,
                Response = MatchResponseType.Ok,
            });
            
            // Store the player and send the player.
            _players.Add(player);
            foreach (var otherPlayer in _players)
            {
                player.Message(new MatchUpdateMessage()
                {
                    Associate = player,
                    Data = new LegoDataDictionary
                    {
                        {"player", player.Id, 9},
                        {"nplayerName", player.Name, 0},
                    },
                    Type = MatchUpdateType.PlayerAdded,
                });
                otherPlayer.Message(new MatchUpdateMessage()
                {
                    Associate = otherPlayer,
                    Data = new LegoDataDictionary
                    {
                        {"player", player.Id, 9},
                        {"nplayerName", player.Name, 0},
                    },
                    Type = MatchUpdateType.PlayerAdded,
                });
            }
            
            // Update the timer.
            UpdateTimer();
            
            // Send the current state of the players.
            // TODO: For some reason, a delay is required. Otherwise, the client won't display the existing players.
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                foreach (var otherPlayer in _players)
                {
                    player.Message(new MatchUpdateMessage()
                    {
                        Associate = player,
                        Data = new LegoDataDictionary {{"player", player.Id, 9}},
                        Type = (_readyPlayers.Contains(otherPlayer) ? MatchUpdateType.PlayerReady : MatchUpdateType.PlayerNotReady),
                    });
                }
            });
        }
        
        /// <summary>
        /// Removes a player from the match.
        /// </summary>
        /// <param name="player">Player to remove.</param>
        public void RemovePlayer(Player player)
        {
            // Remove the player.
            _players.Remove(player);
            _readyPlayers.Remove(player);
            _playersSentTime.Remove(player);
            foreach (var otherPlayer in _players)
            {
                otherPlayer.Message(new MatchUpdateMessage()
                {
                    Associate = otherPlayer,
                    Data = new LegoDataDictionary {{"player", player.Id, 9}},
                    Type = MatchUpdateType.PlayerRemoved,
                });
            }
            
            // Update the time.
            UpdateTimer();
        }

        /// <summary>
        /// Set a player as ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerReady(Player player)
        {
            // Send to all players that the player is ready.
            _readyPlayers.Add(player);
            player.Message(new MatchResponseMessage()
            {
                Associate = player,
                Response = 0,
            });
            foreach (var otherPlayer in _players)
            {
                otherPlayer.Message(new MatchUpdateMessage()
                {
                    Associate = otherPlayer,
                    Data = new LegoDataDictionary {{"player", player.Id, 9}},
                    Type = MatchUpdateType.PlayerReady,
                });
            }
            
            // Update the time.
            UpdateTimer();
        }

        /// <summary>
        /// Set a player as not ready in their current match.
        /// </summary>
        /// <param name="player">Player to mark as ready.</param>
        public void SetPlayerNotReady(Player player)
        {
            // Send to all players that the player is no longer ready.
            _readyPlayers.Remove(player);
            player.Message(new MatchResponseMessage()
            {
                Associate = player,
                Response = 0,
            });
            foreach (var otherPlayer in _players)
            {
                otherPlayer.Message(new MatchUpdateMessage()
                {
                    Associate = otherPlayer,
                    Data = new LegoDataDictionary {{"player", player.Id, 9}},
                    Type = MatchUpdateType.PlayerNotReady,
                });
            }
        }
    }
}