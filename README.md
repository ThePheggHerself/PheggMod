# PheggMod
 
PheggMod API for SCP: Secret Laboratory

# Plugin Directory
There are 2 locations where plugins can be loaded from:
 - config/ServerPort/plugins - This allows you to launch multiple servers using different plugins, without having all servers run them.
 - serverinstall/plugins - This is the same way as done with Smod, and allows for multiple servers to use the same plugin setup if `universal_config_file` is set to true


# Configs
As always, these configs all go inside of the `config_gameplay` file

| Config Option | Default value  | Description  |
|:-------------:|:---------------:|:---------------:|
| pheggmod_debug | false | Enables debug mode for Pheggmod and supporting plugins |
| universal_config_file | false | Specifies if the server should load plugins from your config location, or the plugins folder from the server's install directory |
| announce_chaos_spawn | true | Announces Chaos respawn |
| chaos_announcement | `ATTENTION ALL PERSONNEL . CHAOS INSURGENCY BREACH IN PROGRESS` | Message that plays when Chaos spawn |
| cassie_glitch | false | Disable's CASSIE's random glitching during (non-decontamination) announcements |
| cassie_glitch_post_detonation | false | Limit's CASSIE's random glitching to post-warhead detonation |
| fix_sticky_round | true | Fixes sticky rounds (Rounds that stay active despite no players being on the server) |
| auto_restart_time | null | Specifies (on 24-hour format) when the server should automatically restart itself |

# Commands
These are the available custom commands that Pheggmod offers
| Command (Aliases) | Variables | Permission | Description |
|:---------------:|:---------------:|:---------------:|:---------------:|
| oban (offlineban ltapban) | [UserID] [Duration (e.g 3d, 1y)] [Reason] | BanningUpToDay | Adds the UserID to the UserIdBans file |
| nukelock (nlock nukel locknuke) |  | WarheadEvents | Locks the nuke, preventing it from being enabled/disabled |
| nuke | [Enable\disable] | WarheadEvents | Enables/Disables the nuke's lever |
| pbc (personalbroadcast privatebroadcast) | [PlayerId] [Seconds] [Message] |  | Sends a broadcast to only the specified player |
| drop (dropall dropinv) | [PlayerID] | PlayersManagement | Drops all items from a player's inventory |
| kill | [PlayerID] | PlayersManagement | Kills the targeted player |
| lightsout | | FacilityManagement | Causes the lights to go out inside the facility (Due to the limitations of the game's code, this is quite buggy ATM) |
| pluginreload (reloadplugins plreload) | | ServerConfigs | Reloads the server's plugins on the next round restart |
| nevergonna | [Give] [You] [Up] | | See for yourself |
