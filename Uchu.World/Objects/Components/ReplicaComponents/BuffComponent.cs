using System;
using System.Collections.Generic;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.World
{
    public class BuffInfo
    {
        public uint BuffId;
        public uint DurationSecs;
        public bool CancelOnUnequip;
        public bool CancelOnRemoveBuff;
        public bool CancelOnUi;
        public bool CancelOnZone;
        public bool CancelOnDamaged;
        public bool CancelOnDeath;
        public bool CancelOnLogout;
        public bool ApplyOnTeammates;
        public bool UseRefCount;
    }

    public class BuffComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.BuffComponent;

        private readonly List<BuffInfo> _buffs = new List<BuffInfo>();

        protected BuffComponent()
        {

        }

        public void AddBuff(BuffInfo buffInfo)
        {
            _buffs.Add(buffInfo);

            if (!GameObject.TryGetComponent<DestroyableComponent>(out var destroyable)) return;
            if (buffInfo.CancelOnDeath)
            {
                Delegate onDeathDelegate = null;
                onDeathDelegate = Listen(destroyable.OnDeath, () =>
                {
                    ReleaseListener(onDeathDelegate);
                    RemoveBuff(buffInfo);
                });
            }

            if (buffInfo.CancelOnDamaged)
            {
                // this looks ugly, maybe there's a better way to listen once
                Delegate cancelOnDamaged1 = null;
                Delegate cancelOnDamaged2 = null;
                cancelOnDamaged1 = Listen(destroyable.OnArmorChanged, (newValue, delta) =>
                {
                    if (delta >= 0) return;
                    ReleaseListener(cancelOnDamaged1);
                    ReleaseListener(cancelOnDamaged2);
                    RemoveBuff(buffInfo);
                });
                cancelOnDamaged2 = Listen(destroyable.OnHealthChanged, (newValue, delta) =>
                {
                    if (delta >= 0) return;
                    ReleaseListener(cancelOnDamaged1);
                    ReleaseListener(cancelOnDamaged2);
                    RemoveBuff(buffInfo);
                });
            }

            if (buffInfo.DurationSecs != 0)
                GameObject.Zone.Schedule(() =>
                {
                    RemoveBuff(buffInfo);
                }, buffInfo.DurationSecs * 1000);

            // TODO: handle other cancel events
            // if (buffInfo.CancelOnZone) ;
            // if (buffInfo.CancelOnUi) ;
            // if (buffInfo.CancelOnLogout) ;
        }

        public bool HasBuff(uint buffId)
        {
            return _buffs.Exists(buffInfo => buffInfo.BuffId == buffId);
        }

        public void RemoveBuff(BuffInfo buffInfo)
        {
            _buffs.Remove(buffInfo);
        }

        public void RemoveBuffById(uint buffId)
        {
            _buffs.Remove(_buffs.FirstOrDefault(buffInfo => buffInfo.BuffId == buffId));
        }

        public override void Construct(BitWriter writer)
        {

        }

        public override void Serialize(BitWriter writer)
        {

        }
    }
}
