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
- the directly observed top-level wrapper methods from the assembly around `10001920` through `100022B0`

## Notes

Several deep internal functions (`sub_10003D00`, `sub_10003890`, `sub_10003F00`, and similar) are still only partially understood.
Their wrapper entry points are represented here, but the inner behavior is intentionally conservative and stays close to the currently observed state machine instead of inventing playback behavior.
