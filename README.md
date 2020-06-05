![Rush!](assets/rush.png)

# Rush!
Custom ruleset for osu!lazer, based loosely on Muse Dash.

Demo: https://www.youtube.com/watch?v=HXdQd65CwkU

## Status
WIP but playable.

## Installation

Prebuilt binary releases are available if you're looking for testing the ruleset or don't possess a developpment environment:
| [Releases Page](https://github.com/swoolcock/rush/releases/) |
| -------------|

**NOTE**: For the moment, only desktop platforms (Windows, mac OSX, Linux) are supported.

The ruleset consists of a single DLL file that you'll need to drop in the `rulesets` directory of osu!lazer data directory.

### Installation instructions

1. Navigate to the osu!lazer data directory. You can navigate to it by opening the osu! settings panel and using "open osu! folder" button. Alternatively you can directly navigate to the `rulesets` folder via your OS directory explorer at the following locations:
    - `%AppData%/osu/rulesets` on Windows
    - `~/.local/share/osu/rulesets` on Linux / mac OSX 

    **NOTE**: These paths may be incorrect if you have relocated your osu! data directory to another directory.

2. Drag and drop into the `rulesets` directory the ruleset DLL file.

3. Have fun!
  If lazer was running while installing the ruleset, you may need to restart the game in order for the ruleset to appear in-game.

**NOTE**: rulesets don't automatically update alongside lazer but have a compatibility mechanism to continue using them with newer lazer versions. However, some changes made on game-side may break that compatibility and require installing a newer version of the ruleset.
Thus it is recommended that you periodically head over the releases page and replace the ruleset DLL in the `rulesets` folder with the latest available version.


## Map Translation
* Circles above half map height are converted to flying minions, below half are ground minions.
* Circles approximately mid map height are converted to double hits.
* Sliders are converted to note sheets in the same positional fashion as minions.
* Sliders with repeats add minions in the opposite lane of the note sheet at the start, end, and each repeat.
* Spinners are converted to minibosses.
* Circles with all of clap, whistle, and finish are converted to a sawblade in the opposite lane.

## TODO
* Installation instructions
* Better player sprite
* Rewinding
* Boss sprite during kiai sections
* Hammers and other minion variants
* Health pickups
* Custom avatars/abilities via mods?