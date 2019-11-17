# Uchu [![appveyor](https://img.shields.io/appveyor/ci/yuwui/Uchu/rewrite.svg?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/yuwui/uchu)

LEGO Universe server written in C#

## Disclaimer
> The LEGO Group has not endorsed or authorized the operation of this game and is not liable for any safety issues in relation to its operation.

## Contributions
Feel free to join this development discord server https://discord.gg/7vxaZ2M, to communicate with other contributors.

## Setup
Uchu is built with .NET Core 3.0 which is compatible with 64-bit versions Windows, Linux, and MacOS. Although Uchu can run on both Windows and MacOS it is highly recommended that you run it on Linux if passible.

### Prerequisite

#### .NET Core 3.0
Install .NET Core 3.0 SDK for your OS using the instructions found on https://dotnet.microsoft.com/download/dotnet-core/3.0.

#### Entity Frameword Command Line Interface
Run ```dotnet tool install --global dotnet-ef``` in the terminal to install the interface that is required to build the database.

#### PostgresSQL
Uchu uses PostgresSQL as its database provider. If and when prompted to choose a password for the "postgres" user, choose "postgres" for ease of setup later on.

##### Linux (Debian)
```
sudo apt update
sudo apt install postgresql postgresql-contrib
```
##### Windows
https://www.enterprisedb.com/downloads/postgres-postgresql-downloads#windows

##### MacOS
https://postgresapp.com/

#### Redis (optional)
Uchu uses Redis as its Cache service provider. If you decide to skip this step, the server will fall back to the database for caching. The latest version of Redis is only nativly supported on Linux so setting it up on MacOS or Windows requires some workarounds.

##### Linux (Debian)
```
sudo apt install redis-server
```

##### MacOS
https://medium.com/@petehouston/install-and-config-redis-on-mac-os-x-via-homebrew-eb8df9a4f298

##### Windows
https://redislabs.com/blog/redis-on-windows-10/

#### LEGO Universe Client
Go to https://docs.google.com/document/d/1v9GB1gNwO0C81Rhd4imbaLN7z-R0zpK5sYJMbxPP3Kc/ and naviage to the "Client" section to download a LEGO Universe client.
IMPORTANT! Download the unpacket client.

#### TcpUdp Mod
Uchu utilizes a client mod called TcpUdp to make the server/client connection more stable. 
1. Downloaded "shim_dll.zip" at https://bitbucket.org/lcdr/raknet_shim_dll/downloads/.
2. Extract it into your LEGO Universe client directory.

##### Linux and MacOS
The only publicly available LEGO Universe clients are for Windows. So you will need a compatibility layer if you are on a operation system which is not Windows.
1. Download and install WINE using the instructions on https://wiki.winehq.org/Download.
2. To run LEGO Universe, run ```WINEDLLOVERRIDES="dinput8.dll=n,b" wine ./legouniverse.exe``` using the terminal.

#### IDE (optional)
Although you can build and run Uchu using the command line, it is recommaned that you utilize an IDE to make your life just a little bit easier. Here are some recommendations.

##### Rider
The contributors of Uchu highly recommend JetBrains Rider, https://www.jetbrains.com/rider/, it is crossplatform and free if you are a student.

##### Visual Studio / VS Code
Visual Studio is a great IDE for Windows users and Visual Studio Code is a great crossplatform IDE https://visualstudio.microsoft.com/.

#### Git (optional)
Some IDEs comes preinstalled with git version control but most do not. For having a smooth download experience, we recommend you download git as an independent program.

##### Linux
```
sudo apt install git
```
##### MacOS and Windows
https://desktop.github.com/

### Download

#### Linux
```
git clone https://github.com/yuwui/Uchu.git
git checkout rewrite
```

#### MacOS and Windows
1. https://help.github.com/en/desktop/contributing-to-projects/cloning-a-repository-from-github-to-github-desktop
2. Change your "Current Branch" to "rewrite"

### Execution Prerequisite

#### Build
Before running Uchu for the first time, build it once to make sure it compiles and to generate config files. Navigate to the Uchu.Auth, Uchu.Char, Uchu.World, and Uchu.Master directories and run ```dotnet build``` using the terminal.

#### Configure Servers
Uchu is not a LEGO Universe repository and you will have to supply your own game resource files for the servers to work with. 
1. Navigate to where you have LEGO Universe installed and enter the "/res" directory, copy the full path to it.
2. For Uchu.Master, navigate to where you built the server. This is commonly /bin/Debug/netcoreapp3.0 but may differ.
3. Run ```dotnet run``` using the terminal, this should generate a config file.
3. Find the "config.default.xml" file, duplicate it, rename the new copy to "config.xml".
4. Find where you see ```<GameResourceFolder></GameResourceFolder>``` and copy your LEGO Universe "/res" path inbetween the tags.

##### Set PostgresSQL credentials
If you chose something else than "postgres" from your PostgresSQL password, put your password inbetween the ```<Password></Password>``` tags.

#### Database Setup
Make sure a PostgresSQL server is running and run ```dotnet ef database update -c UchuContext``` in the terminal to build the database form the Uchu.Core directory.

#### Redis (optional)
Make sure a Redis Cache server is running.

### Run it!
Finally after this long and tedius setup process, you are ready to run Uchu!

#### Build Servers
Navigate to the Uchu.Auth, Uchu.Char, Uchu.World, Uchu.Master, and Uchu.StandardScripts directories and run ```dotnet build``` using the terminal.

#### Run Master Server
Navigate to the output directory of Uchu.Master and run ```dotnet Uchu.Master.dll``` using the terminal.

#### Create a user
In the Uchu.Master console, type ```/adduser (username)``` and press enter. You will than have to enter a password which will be displayed as stars (*), and when you are done, press enter.

#### Login
Using your LEGO Universe client you may now login and play what parts of LEGO Universe Uchu has to offer :)

#### Extra
In the Uchu.Master console, type ```/gamemaster (username) 2```. This will make your account an Admin and will give you access to a lot of commands. Type "/" using the ingame chat to have your options displayed to you.
