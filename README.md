# Gedx8MusicDriver reverse status

## Published wrapper surface

The currently uploaded C# snapshot already covers the exported/native wrapper surface through method-table slot `+0x94`.
That means there are no completely missing *published* wrapper entries left in the current DLL-facing surface.

Implemented slots in the current model:

- `+0x00` bootstrap / global init
- `+0x04` global destroy / registry cleanup
- `+0x08` create driver instance
- `+0x0C` destroy driver instance
- `+0x10` init synthesizer
- `+0x14` / `+0x18` / `+0x1C` synth follow-up wrappers
- `+0x20` load cached object
- `+0x24` / `+0x28` / `+0x2C` / `+0x30` loader and lifetime helpers
- `+0x34` create audiopath
- `+0x38` activate audiopath
- `+0x3C` set audiopath volume
- `+0x40` / `+0x44` / `+0x48` / `+0x4C` selector and property helpers
- `+0x50` destroy audiopath
- `+0x54` / `+0x58` / `+0x5C` playback helpers
- `+0x60` / `+0x64` / `+0x68` composite helper/query paths
- `+0x6C` destroy composite or segment wrapper
- `+0x70` / `+0x78` thin type-1 helper paths
- `+0x7C` / `+0x80` / `+0x84` / `+0x88` / `+0x90` thin type-2 helper paths
- `+0x94` stub returning failure / false

## New facts from the large x32dbg trace

The new trace tightens the understanding of the thin type-2 configure/query area.

### `+0x88` (`10002180`) is not just a simple key/value setter

The trace shows a generic linked-list record store behind the dispatcher:

- the list head is at driver offset `+0x104`
- each node is `0x1C` bytes large
- observed node layout is consistent with:
  - `+0x00` next pointer
  - `+0x04..+0x13` a 16-byte signature/key
  - `+0x14` payload pointer
  - `+0x18` payload size / mode
- when no existing entry matches, a new node is allocated, payload memory is allocated separately, the payload bytes are copied, and the node is inserted at the list head

This means the current C# thin type-2 configure model is still too flat if it only stores name/value state in dictionaries.
A linked-list-like named record store is closer to the native behavior.

### There are special-case fast paths before the generic fallback

Two special signature tables are checked before falling back to generic list insertion:

- one table rooted at `6B212B24`
- one table rooted at `6B212CA4`

The trace shows at least these additional side effects:

- one special path updates driver offset `+0x340` using a `float` payload with range checks
- another special path updates driver offset `+0x33C` from a 32-bit payload and also touches the lock-protected block at `+0x27C`

The exact semantic names of the static 16-byte keys passed from the game are still unresolved.

### The game-side names/keys are still not semantically mapped

Observed static 16-byte key blocks include the values passed from addresses:

- `1577C2B8`
- `1577C2C8`
- `1577C2D8`
- `1577C2E8`

Their *behavior* is partially known from the trace, but their exact logical meaning is still not fully named.

## What is still missing for a real finish

The reverse project is now mostly blocked by deep inner semantics, not by missing exported wrappers.

### Still needs x32dbg/runtime confirmation

- exact inner COM behavior of `sub_10003D00`
- exact thin kind `1` / `2` object construction semantics inside `sub_10003890`
- exact metadata meaning of the composite table/state fields created by `10004120` and `10004670`
- real DMIME semantics of `10004250`, `100042C0`, and `10004490`
- full meaning of the lazily created selector helper objects behind `10002580`
- exact query-path readback behavior for the thin type-2 named-record list behind `10002110`
- exact semantic mapping of the special 16-byte keys used by the `+0x84` / `+0x88` dispatcher
- exact meaning of the lock-protected callback/refresh path guarded around offsets `+0x27C` and `+0x294`
- confirmation whether the current aliasing of `+0x70` / `+0x74` / `+0x78` is identical in all callers

## Added C# helper

A new helper class can now be used to model the observed generic named-record mechanism more faithfully:

- `Gedx8ThinType2NamedRecordStore.cs`

This class mirrors the trace-visible behavior of:

- 16-byte signature matching
- update-or-insert semantics
- head insertion into a singly linked logical record chain
- separate payload storage with explicit payload size

It does **not** claim that all special-case behavior is decoded already.
It is only a stricter model of the generic fallback path that the trace clearly showed.
