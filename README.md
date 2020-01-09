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
| external_api_location | string.Empty | Url of any external API that you wish to use for features such as custom tags |
| universal_config_file | false | Specifies if the server should load plugins from your config location, or the plugins folder from the server's install directory |
| cassie_glitch | false | Enables CASSIE glitches during announcements |
| cassie_glitch_post_detonation | false | Enables CASSIE glitches after warhead detonation only (You do not need to enable `cassie_glitch` to use this) |
| chaos_announcement | true | Announces Chaos respawn |
| chaos_announcement | `ATTENTION ALL PERSONNEL . CHAOS INSURGENCY BREACH IN PROGRESS` | Message that plays when Chaos spawn |
