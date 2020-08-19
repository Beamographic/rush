![Rush!](assets/rush.png)

# Rush!
Custom ruleset for osu!lazer, based loosely on Muse Dash.

Demo: https://www.youtube.com/watch?v=HXdQd65CwkU

Discord: https://discord.gg/2P9E8MS

The Discord server is currently just for realtime discussion, but could be expanded if there is community interest.

## Status
WIP but playable.

## Installation

Prebuilt binary releases are available if you are looking to test or play and do not wish to compile yourself.
| [Releases Page](https://github.com/swoolcock/rush/releases/) |
| -------------|

**NOTE**: osu!lazer runs custom rulesets only on desktop platforms (Windows, macOS, Linux) for the time being.

The ruleset consists of a single DLL file that you'll need to place in the `rulesets` subdirectory of your osu!lazer installation.

### Installation instructions

1. Navigate to the osu!lazer data directory. You can do so by opening the settings panel in osu!lazer and clicking on the "open osu! folder" button. Alternatively you can directly navigate to the `rulesets` directory via your OS directory explorer at the following locations:
    - `%AppData%/osu/rulesets` on Windows
    - `~/.local/share/osu/rulesets` on Linux / macOS

    **NOTE**: If you have relocated your osu! data directory to another directory, the `rulesets` directory will be there instead.

2. Drag and drop the ruleset's DLL file into the `rulesets` directory.

3. Have fun!
    If osu!lazer was running while installing the ruleset, you may need to restart the game in order for the ruleset to appear.

**NOTE**: Custom Rulesets do not automatically update alongside osu!lazer but have a compatibility mechanism to continue using them on newer game versions. However, some changes made game-side may break that compatibility and require installing a newer version of the ruleset.
Thus it is recommended that you periodically head over the releases page and replace the ruleset DLL in the `rulesets` directory with the latest available version.

## TODO
* Better player sprite
* Boss sprite during kiai sections
* Hammers and other minion variants
* Custom avatars/abilities via mods?
* Editor (waiting on updates in lazer)
