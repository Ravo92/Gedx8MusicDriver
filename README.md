# Gedx8MusicDriver

This project is a cleaned-up C# port of the directly observed wrapper layer of `gedx8musicdrv.dll`.

## Scope

Only code that can be tied directly to the DLL wrapper layer is kept here:

- `GetInterface2` bootstrap / interface creation
- global instance registry behavior
- driver-instance creation and release layout
- the observed synth-init profile mapping
- the observed slot-table growth rules
- the wrapper-level object-kind gates for kinds `0`, `1`, and `2`
- the DirectMusic loader search-directory preparation path
- the directly observed published method-table surface currently exposed through the native wrapper table
- explicit native helper exports for the still-unmapped wrappers `10001CF0`, `10001D30`, `10001F10`, and `10001FF0`

## Notes

Several deep internal functions (`sub_10003D00`, `sub_10003890`, `sub_10003F00`, and similar) are still only partially understood.
Their wrapper entry points are represented here, but the inner behavior remains conservative and stays close to the currently observed state machine instead of inventing playback behavior.

The current native export layer intentionally keeps the confirmed method-table layout stable.
Wrappers that exist in the assembly but whose exact slot offsets are still not confirmed are exported separately by name instead of being forced into guessed table offsets.

Composite and ThinType2 object behavior is now stateful enough for wrapper-level querying and configuration,
but it should still be treated as a conservative reimplementation of the observed wrapper semantics rather than a fully decoded clone of the original playback internals.