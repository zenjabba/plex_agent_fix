# plex_agent_fix

# Linux Install

Grab the 0.3 release of the binaries
 
`https://github.com/zenjabba/plex_agent_fix/releases/download/0.3/plex_agent_fix-0.3-release.zip`

Unzip the binaries

`wget https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb`

`sudo dpkg -i packages-microsoft-prod.deb`

`sudo apt-get update`

`sudo apt-get install apt-transport-https`

`sudo apt-get update`

`sudo apt-get install dotnet-runtime-3.1`

`sudo apt-get install -qqy sqlite3 libsqlite3-dev`

# Configuration File

fill out config.json
required fields are:

plexUser
plexPass
plexProtocol
plexPort
plexHost
sectionsToProcess

`{
  "plexUser": "plexuser@gmail.com",
  "plexPass": "pl3xp455w0rd!",
  "plexProtocol": "https",
  "plexPort": 443,
  "plexHost": "martin.plexsupermegafuninstall.org",
  "preferredAgent": "plex://movie",
  "sectionsToProcess": [
    1,
    6,
    9
  ]
}
`
# Agent Fix
`dotnet ./plex_agent_fix.dll --agentfix --verbose`

# Remove Unavailable Files
`dotnet ./plex_agent_fix.dll --remove --verbose`
