# Height Adjustment Plugin

This unofficial TaleSpire plugin allows the user to adjust the height of a mini. The base is automatically hidden if
the mini is not grounded. This can be used as an alternative to fly which both the player and GM can access or it can
be used to adjust the height of converted props.

## Change Log

1.0.0: Initial release

## Install

Use R2ModMan or similar installer to install this plugin.

The R2ModMan configuration for this plugin can be used to changed the shortcut keys and dig setting.

## Usage

Select a mini and then use the configured shortcut keys (default numpad minus and numpad plus) to adjust the height
of the mini. When the mini is not grounded, the base automatically disappears. The height of the mini (assuming 5 feet
per tile) is displayed at the top of the screen while a height adjusted mini is selected.

If the allowDig setting is false (default false) the mini adjustment can range from the floor and higher. If the allowDig
setting is set to true, the mini can also be moved in the opposite direction (i.e. into the ground).

### Board Load

Like many other plugins, this plugin has the problem that height adjusting on board load can cause issues because not
all of the assets are completely ready when TS reports the board as loaded. As such, minis are not automatically adjusted
on load. To adjust them to their previous settings, press the transformation keyboard shortcut (default RCTRL+T). This will
cause all minis to update their height adjustments, if any, and show or hide the base accordingly. 

## Limitation

On board load, the user needs to press the Transformation key to re-apply all of the adjustments to all the minis. Note that
using the transformation keyboard shortcut re-applies the height adjustment on all present minis (not just the selected one).
However, if new height adjusted minis are added to the board, it may be necessary to re-apply the transformation.
