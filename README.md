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

## Current state

The exported wrapper surface is in place through slot `+0x6C`, including the previously missing helper exports.

`10002580` is modeled close to the native jump-table layout, and the composite load path around `10003890` is now also much closer to the observed native structure.

The C# side now reflects these confirmed points from the ASM and runtime traces:

- selector `0` uses the lazy object slot at `this+0x148`
- selectors `1` and `2` use `this+0x14C` / `this+0x150` and are only valid when `this+0x1B0` is `2` or `3`
- selector `3` uses `this+0x154` and is only valid when `this+0x1B0` is `0`
- selectors `4` through `12` use `this+0x158` through `this+0x178`
- selectors `4` through `12` do **not** form a simple dependency chain; each has its own lazy-init path
- selectors `4` through `12` initialize through the deeper `10002C90` helper and then immediately perform a bootstrap read back into the cached `this+0x17C` field area
- selector `3` is special: the write path uses a 3-dword structure starting at `this+0x188`, which overlaps with the cached scalar slots later used by selectors `4` and `5`
- the `10003890` kind-0 path is effectively a two-stage build:
  - prepare the loader / search-directory state
  - build an inner composite object of native size `0x1C`
  - then build an outer wrapper record of native size `0x10`
- the outer composite record is not marked active immediately at allocation time; activation happens later in the caller-side registration path
- the outer composite record layout is now modeled around the observed native shape:
  - `+0x04` kind
  - `+0x08` loader mode
  - `+0x0C` pointer/reference to the inner composite object
- the inner composite object is now modeled around the observed `10004120` layout:
  - `+0x04` source/interface-like object
  - `+0x08` driver/owner object
  - `+0x0C` link/helper object
  - `+0x10` table pointer/state
  - `+0x14` descriptor/state value
  - `+0x18` group count
  - `+0x1A` entry count
  - `+0x1B` init result
- the caller-side registration path still owns the slot-table growth / insertion logic, which matches the runtime trace where the loaded object is inserted only after the composite record has been built

This means the old model of “one C# object that is instantly active and directly doubles as the native composite build result” was too flat.
The current C# code now follows the observed outer-record / inner-object split much more closely.

## Remaining open points

The project is still not at a full original-behavior end state.

The largest remaining gaps are the deep inner semantics of paths such as:

- `sub_10003D00`
- the exact COM/object semantics inside `sub_10003890` for kinds `1` and `2`
- the exact metadata/query semantics behind `10004120` and `10004670`
- `10004250` beyond the currently observed forwarding and HRESULT behavior
- `100042C0`
- the exact inner COM/object behavior behind each `10002580` selector once the lazy object has been created
- the deeper thin-type query/configure paths around `10003F00`, `10003F50`, and `10003FA0`

So the wrapper surface, selector layout, and the composite kind-0 build structure are now much better grounded,
but the deeper inner COM-style semantics behind those objects are still modeled conservatively on the C# side.
