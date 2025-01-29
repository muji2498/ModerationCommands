# Moderation Commands - Nuclear Option

## Bepinex Version
This mod requires bepinex version [5.4.23.2](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) and NewtonsoftJson

## Notes
The mod will reply back showing the calling players name, i intend to replace this with something like `Server` or `Host`, this could be a configurable name aswell.

# Built in commands
`kick playername|steamid` (Owner | Admin | Moderator) - Will kick the player specified. <br>
`ban playername|steamid` (Owner | Admin | Moderator) - Will ban the player specified. <br>
`unban steamid` (Owner | Admin | Moderator) - Will unban the player specified. <br>
`setfps` (Owner) - Will set the fps of the server. <br>
`ticket` - Will send a ticket to the discord webhook. <br>
`select number|cancel` (Owner | Admin | Moderator) - command used to select the player, selection can be cancelled by typing cancel <br>
`friendlyfire` (Owner | Admin) - Will Toggle whether friendly fire should be on or off. <br>
`friendlyfire.kickonkill` (Owner | Admin) - Will Toggle whether players should be kicked if they kill another player. <br>
`friendlyfire.player` (Owner | Admin) - Will Toggle whether players should be kicked if they damage/kill players. <br>
`friendlyfire.unit` (Owner | Admin) - Will Toggle whether players should be kicked if they damage/kill units. <br>
`friendlyfire.maxincidents` (Owner | Admin) - Will modify the max amounts of incidents. <br>
`friendlyfire.unitthreshold` (Owner | Admin) - Will modify the amount of friendly units a player can kill before hitting a threshold. <br>
`friendlyfire.reset` (Owner | Admin) - Will clear the incidents. <br>

## Building Project

Change `<GameDir>YourGameDirectoryHere</GameDir>` inside GameDir.targets to the install location of your game. <br>
Example: `<GameDir>C:\Program Files (x86)\Steam\steamapps\common\Nuclear Option</GameDir>`
