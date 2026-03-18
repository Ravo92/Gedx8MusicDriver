# Gedx8MusicDriver

This update finishes the `10002580` / `10002C60` / `10002C90` block as an ASM-first reconstruction, even though the normal Cultures runtime path did not hit these slots during live testing.

## What changed

### 10002580

The selector dispatcher remains modeled as the native `0..12` jump-table path with the already confirmed lazy-slot layout:

- selector `0` -> cached slot `+0x148`
- selector `1` -> cached slot `+0x14C`
- selector `2` -> cached slot `+0x150`
- selector `3` -> cached slot `+0x154`
- selectors `4..12` -> cached slots `+0x158 .. +0x178`

The mode gates also stay aligned with the ASM:
- selector `0` has no extra mode gate
- selectors `1` and `2` require mode `2` or `3`
- selector `3` requires mode `0`

The fallback write-slot behavior at `this+0x17C + selector*4` remains part of the model, but this C# side still exposes it through the simplified `DispatchProperty(...)` API rather than through raw pointer-based caller storage.

### 10002C60

`10002C60` is now treated as a two-token helper-creation path instead of a generic "simple create" shortcut.

The native call shape is:

- caller passes a selector token such as `1000C558` / `1000C548`
- caller also passes the target lazy-slot address (`this+0x148` etc.)
- `10002C60` then forwards:
  - target slot pointer
  - selector token
  - `0`
  - fixed factory token `1000C1C8`
  - `0`
  - `0x6000`
  - `-5`
  - current object at `this+0x1C0`
  - into `call [vtbl+0x0C]`

The C# model now records both token layers:

- selector token hash
- fixed factory token hash for `1000C1C8`
- target cached-slot offset
- creation ordinal

This is still a state model, not the real COM helper behind native `+[vtbl+0x0C]`, but it now matches the observed outer ABI much more closely.

### 10002C90

`10002C90` is now modeled as a descriptor-table producer first and a typed helper finalizer second.

The native flow is reflected as:

- use byte counter at `this+1`
- compute descriptor-table entry at `this+0x04 + count*0x20`
- write four dwords into the descriptor body
- call `10002C60("1000C558", &localHelperSlot)`
- call helper `+[vtbl+0x54]`
- increment the descriptor count byte
- call helper `+[vtbl+0x5C]`

The C# model now records for each typed selector helper:

- descriptor entry size `0x20`
- descriptor table offset (`0x124 + ordinal*0x20` model)
- selector id
- cached-object offset
- backing-value offset
- get/set method offsets
- selector token hash
- fixed factory token hash (`1000C1C8`)
- finalize/commit method offsets (`+0x54` / `+0x5C`)
- creation ordinal

This is intentionally still not claiming full native equivalence for the helper object itself, but it is now much closer to the actual layered helper construction visible in the ASM.

## Runtime-observation status

The important limitation is unchanged:

- `10002580`
- `10002C60`
- `10002C90`

were **not** observed on the normal tested Cultures playback path at runtime.

So this block should currently be read as:

- **strong static reconstruction from ASM**
- **not yet runtime-confirmed from the game's normal music path**

## Remaining open points for this block

Still open are:

- the real COM/helper object returned through native `10002C60`
- the exact meaning of every dword written by native `10002C90`
- the full semantics of helper `+[vtbl+0x54]`
- the full semantics of helper `+[vtbl+0x5C]`
- whether Cultures exercises this whole selector-helper path only in uncommon or unused scenarios

## Best next targets

With this block now statically tightened, the most productive next reverse targets remain:

- `10004250`
- `100042C0`
- `10003D00`
- inner COM semantics of `10003890` for kind `1/2`
