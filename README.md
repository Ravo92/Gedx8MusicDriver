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

## New facts from the latest traces

### The recent `.sgt` map-load trace went through the composite branch

The latest runtime trace was triggered by loading a `.sgt` file for map music playback.
Even though the file type suggested a thin-path candidate at first glance, the branch discriminator inside `10003890` went to the `descriptor[0] == 0` path.
That means this concrete `.sgt` load used the composite construction path rooted at `10003B1D` / `10004120`, not the thin kind `1` / `2` branches.

### Thin kind `1` / `2` construction inside `10003890` is now structurally resolved

The disassembly around `10003A2B` and `10003AAD` now shows the native thin-object construction shape clearly.

#### Thin kind `2`

- loader helper call uses static token block `1000C218`
- on success an inner object of size `0x0C` is allocated
- `sub_10002D30` writes:
  - `inner + 0x04 = native loader result object`
  - `inner + 0x08 = [esi + 0x0C]`
- then an outer wrapper of size `0x10` is allocated and filled as:
  - `outer + 0x04 = 2`
  - `outer + 0x08 = loader mode`
  - `outer + 0x0C = inner`

#### Thin kind `1`

- same native layout as thin kind `2`
- loader helper call uses static token block `1000C268`
- the outer wrapper differs only by:
  - `outer + 0x04 = 1`

So the thin kind split is primarily a different static loader token plus a different outer kind value.
The inner object shape itself is shared.

### `+0x88` (`10002180`) is not just a simple key/value setter

The earlier large trace already showed a generic linked-list record store behind the dispatcher:

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
- exact semantic identity of the thin inner slot `+0x04` object returned by the native loader call
- exact method semantics behind the thin inner object calls used by `10002D50`, `10002D70`, `10003EB0`, `10003F00`, `10003F50`, and `10003FA0`
- exact metadata meaning of the composite table/state fields created by `10004120` and `10004670`
- real DMIME semantics of `10004250`, `100042C0`, and `10004490`
- full meaning of the lazily created selector helper objects behind `10002580`
- exact query-path readback behavior for the thin type-2 named-record list behind `10002110`
- exact semantic mapping of the special 16-byte keys used by the `+0x84` / `+0x88` dispatcher
- exact meaning of the lock-protected callback/refresh path guarded around offsets `+0x27C` and `+0x294`
- confirmation whether the current aliasing of `+0x70` / `+0x74` / `+0x78` is identical in all callers

## Added / adjusted C# modeling

The current C# snapshot now models the native thin-object shape more explicitly:

- inner thin runtime modeled as the native `0x0C` block
- outer loaded-object wrapper modeled as the native `0x10` block
- thin kind `1` uses static token `1000C268`
- thin kind `2` uses static token `1000C218`

A helper class can also be used to model the observed generic thin type-2 named-record mechanism more faithfully:

- `Gedx8ThinType2NamedRecordStore.cs`

This class mirrors the trace-visible behavior of:

- 16-byte signature matching
- update-or-insert semantics
- head insertion into a singly linked logical record chain
- separate payload storage with explicit payload size

It does **not** claim that all special-case behavior is decoded already.
It is only a stricter model of the generic fallback path that the trace clearly showed.
