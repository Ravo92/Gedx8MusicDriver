using Gedx8MusicDriver.Interop;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8EngineCore : IDisposable
    {
        private readonly int[] _propertyValues17C = new int[13];
        private readonly bool[] _propertyResolved17C = new bool[13];
        private readonly object?[] _cachedObjects148 = new object?[13];
        private readonly int[] _selector3Triple188 = new int[3];
        private readonly int[] _typedSelectorBackings18C = new int[9];
        private readonly Gedx8TypedPropertyDescriptor[] _typedDescriptors124 = new Gedx8TypedPropertyDescriptor[0x20];

        private DirectMusicLoaderContext? _loaderContext;
        private bool _disposed;
        private int _value00;
        private int _value04;
        private int _value08;
        private int _value1B0;
        private int _value1B4;
        private byte _value1B8;
        private int _value1BC;
        private object? _value1C0;
        private int _value1C4;
        private byte _typedDescriptorCount01;
        private int _simplePropertyCreateCount02C60;
        private int _typedPropertyCreateCount02C90;
        private int _lastPropertyCreateTokenHash;
        private int _lastPropertyFactoryTokenHash;
        private int _lastPropertyCreateTargetOffset;
        private int _lastPropertyCreateResult;
        private int _synthDescriptorSize28;
        private int _synthDescriptorMode18;
        private int _synthDescriptorVoiceCount1C;
        private int _synthDescriptorMask20;
        private int _synthCommittedSampleRate24;
        private int _synthCommittedConfig28;
        private float _synthCommittedGainC2B8;
        private int _synthCommittedEnableC2C8;
        private int _synthCommittedSampleRateC2D8;
        private int _synthCommittedConfigC2E8;

        internal Gedx8SynthInitConfig SynthInitConfig { get; private set; } = Gedx8SynthInitConfig.Empty;

        internal Gedx8LoaderMode LoaderMode1B0
        {
            get => (Gedx8LoaderMode)_value1B0;
            set => _value1B0 = (int)value;
        }

        internal byte SelectionByte1B8 => _value1B8;

        internal int SelectionValue1BC => _value1BC;

        internal bool IsSynthInitialized { get; private set; }

        internal string? SearchDirectory => _loaderContext?.SearchDirectory;

        internal int Value00 => _value00;

        internal int Value04 => _value04;

        internal int Value08 => _value08;

        internal bool InitializeSynthesizer(Gedx8SynthInitConfig config)
        {
            if (_disposed || config.SampleRate <= 0)
            {
                return false;
            }

            SynthInitConfig = config;
            IsSynthInitialized = true;
            _value00 = 0;
            _value04 = 0;
            _value08 = 0;
            _value1B8 = 0;
            _value1BC = 0;

            Array.Clear(_selector3Triple188);
            Array.Clear(_typedSelectorBackings18C);
            Array.Clear(_typedDescriptors124);
            _typedDescriptorCount01 = 0;
            _simplePropertyCreateCount02C60 = 0;
            _typedPropertyCreateCount02C90 = 0;
            _lastPropertyCreateTokenHash = 0;
            _lastPropertyFactoryTokenHash = 0;
            _lastPropertyCreateTargetOffset = 0;
            _lastPropertyCreateResult = 0;

            _synthDescriptorSize28 = 0x28;
            _synthDescriptorMode18 = 1;
            _synthDescriptorVoiceCount1C = 7;
            _synthDescriptorMask20 = 0x3F;
            _synthCommittedSampleRate24 = config.SampleRate;
            _synthCommittedConfig28 = config.Config;
            _synthCommittedGainC2B8 = 1.0f;
            _synthCommittedEnableC2C8 = 1;
            _synthCommittedSampleRateC2D8 = config.SampleRate;
            _synthCommittedConfigC2E8 = config.Config;

            ResetResolvedPropertyCache();
            SetRuntimeModeFields10002380(0, config.Config, 1);
            SetCurrentObject10002380(this);
            return true;
        }

        internal bool Call03BC0(int value0, int value1)
        {
            if (_disposed || !IsSynthInitialized)
            {
                return false;
            }

            if (value0 == 0 || value1 == 0)
            {
                return false;
            }

            _value04 = value0;
            _value08 = value1;

            if (_value1C0 == null)
            {
                _value1C0 = this;
            }

            return true;
        }

        internal bool TryGetPair03BE0(out int value0, out int value1)
        {
            if (_disposed || _value04 == 0 || _value08 == 0)
            {
                value0 = 0;
                value1 = 0;
                return false;
            }

            value0 = _value04;
            value1 = _value08;
            return true;
        }

        internal bool Call03C90()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03CA0()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03E00()
        {
            return !_disposed && IsSynthInitialized && _value1C0 != null;
        }

        internal bool Call03E20(int value)
        {
            if (_disposed || !IsSynthInitialized || _value1C0 == null)
            {
                return false;
            }

            if (_value00 != value)
            {
                _value00 = value;
            }

            return true;
        }

        internal bool Call03E50(int value)
        {
            if (_disposed || !IsSynthInitialized || _value1C0 == null)
            {
                return false;
            }

            _value00 = value;
            return true;
        }

        internal bool PrepareLoaderContext(string searchDirectory)
        {
            if (_disposed || string.IsNullOrWhiteSpace(searchDirectory))
            {
                return false;
            }

            DirectMusicLoaderContext? previous = _loaderContext;
            previous?.Dispose();

            DirectMusicLoaderContext context = new(searchDirectory);
            if (!context.TryPrepare())
            {
                context.Dispose();
                return false;
            }

            _loaderContext = context;
            _value1B0 = 2;
            _value1C4 = 1;
            if (_value1C0 == null)
            {
                _value1C0 = this;
            }

            ResetResolvedPropertyCache();
            return true;
        }

        internal Gedx8LoadedObject? LoadObject(Gedx8ObjectKind kind, string fileName, string? searchDirectory, object? recordOwner08, object? compositeLink0C)
        {
            if (_disposed || !IsSynthInitialized || string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            string? effectiveSearchDirectory = searchDirectory;
            int loaderMode;

            if (string.IsNullOrWhiteSpace(searchDirectory))
            {
                if (string.IsNullOrWhiteSpace(SearchDirectory))
                {
                    return null;
                }

                effectiveSearchDirectory = SearchDirectory;
                loaderMode = 1;
            }
            else
            {
                if (!PrepareLoaderContext(searchDirectory))
                {
                    return null;
                }

                effectiveSearchDirectory = searchDirectory;
                loaderMode = 2;
            }

            string? resolvedPath = ResolvePath(fileName, effectiveSearchDirectory);
            if (resolvedPath == null)
            {
                return null;
            }

            Gedx8LoadedObject loadedObject = new(kind, fileName, effectiveSearchDirectory, resolvedPath);

            switch (kind)
            {
                case Gedx8ObjectKind.Composite:
                    Gedx8CompositeContext compositeContext = CreateCompositeContext(resolvedPath, fileName, effectiveSearchDirectory, loaderMode, recordOwner08, compositeLink0C);
                    if (!InitializeCompositeLoad10003890(loadedObject, compositeContext, loaderMode))
                    {
                        return null;
                    }
                    break;

                case Gedx8ObjectKind.ThinType1:
                    if (!InitializeThinType1Load10003890(loadedObject, fileName, resolvedPath, effectiveSearchDirectory, loaderMode, compositeLink0C))
                    {
                        return null;
                    }
                    break;

                case Gedx8ObjectKind.ThinType2:
                    if (!InitializeThinType2Load10003890(loadedObject, fileName, resolvedPath, effectiveSearchDirectory, loaderMode, compositeLink0C))
                    {
                        return null;
                    }
                    break;

                default:
                    return null;
            }

            ResetResolvedPropertyCache();
            SetRuntimeModeFields10002380(loaderMode, SynthInitConfig.Config, 1);
            SetCurrentObject10002380(loadedObject);
            return loadedObject;
        }

        internal void ReleaseCurrentObject100024B0()
        {
            if (_disposed)
            {
                return;
            }

            _value1C0 = null;
            ResetResolvedPropertyCache();
        }

        internal bool SetSelectionByte(byte value)
        {
            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if (_value1B8 == value)
            {
                return true;
            }

            _value1B8 = value;
            return true;
        }

        internal bool BindSelection(int selection, int mode)
        {
            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if (selection < 0 || mode < 0)
            {
                return false;
            }

            if (!TryInvokeSelectionBinding(selection, mode))
            {
                return false;
            }

            _value1BC = selection;
            _value1B0 = mode;
            ResetResolvedPropertyCache();
            return true;
        }

        internal bool GetSelection(out int selection)
        {
            if (_disposed)
            {
                selection = 0;
                return false;
            }

            selection = _value1BC;
            return _value1C0 != null;
        }

        internal bool DispatchProperty(int selector, int value, bool setMode, out int storedValue)
        {
            storedValue = 0;

            if (_disposed || _value1C0 == null)
            {
                return false;
            }

            if ((uint)selector >= 13)
            {
                return false;
            }

            if (!TryResolvePropertySelector(selector))
            {
                return false;
            }

            if (setMode && !TryWriteResolvedProperty(selector, value))
            {
                return false;
            }

            if (!TryReadResolvedProperty(selector, out storedValue))
            {
                return false;
            }

            return true;
        }

        internal void SetRuntimeModeFields10002380(int mode1B0, int value1B4, int interface1C4)
        {
            if (_disposed)
            {
                return;
            }

            _value1B0 = mode1B0;
            _value1B4 = value1B4;
            _value1C4 = interface1C4;
            _value1B8 = 0;
        }

        internal void SetCurrentObject10002380(object? currentObject)
        {
            if (_disposed)
            {
                return;
            }

            _value1C0 = currentObject;
        }

        internal string? ResolvePath(string fileName, string? searchDirectory)
        {
            if (Path.IsPathRooted(fileName) && File.Exists(fileName))
            {
                return fileName;
            }

            if (!string.IsNullOrWhiteSpace(searchDirectory))
            {
                string direct = Path.Combine(searchDirectory, fileName);
                if (File.Exists(direct))
                {
                    return direct;
                }
            }

            DirectMusicLoaderContext? context = _loaderContext;
            if (context != null && context.TryResolveFile(fileName, out string? resolvedPath))
            {
                return resolvedPath;
            }

            return null;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            DirectMusicLoaderContext? loaderContext = _loaderContext;
            if (loaderContext != null)
            {
                loaderContext.Dispose();
                _loaderContext = null;
            }

            Array.Clear(_propertyValues17C);
            Array.Clear(_propertyResolved17C);
            Array.Clear(_cachedObjects148);

            _value00 = 0;
            _value04 = 0;
            _value08 = 0;
            _value1B0 = 0;
            _value1B4 = 0;
            _value1B8 = 0;
            _value1BC = 0;
            _value1C0 = null;
            _value1C4 = 0;
            Array.Clear(_selector3Triple188);
            Array.Clear(_typedSelectorBackings18C);
            Array.Clear(_typedDescriptors124);
            _typedDescriptorCount01 = 0;
            _simplePropertyCreateCount02C60 = 0;
            _typedPropertyCreateCount02C90 = 0;
            _lastPropertyCreateTokenHash = 0;
            _lastPropertyFactoryTokenHash = 0;
            _lastPropertyCreateTargetOffset = 0;
            _lastPropertyCreateResult = 0;
            SynthInitConfig = Gedx8SynthInitConfig.Empty;
            IsSynthInitialized = false;
            _disposed = true;
        }

        private bool InitializeCompositeLoad10003890(Gedx8LoadedObject loadedObject, Gedx8CompositeContext compositeContext, int loaderMode)
        {
            return loadedObject.Sub10004120(
                0,
                loaderMode,
                null,
                compositeContext.Source04,
                compositeContext.Driver08,
                compositeContext.Link0C,
                compositeContext.DescriptorId14,
                compositeContext.EntryCount1A,
                compositeContext.GroupCount18,
                compositeContext.Table10);
        }

        private Gedx8CompositeContext CreateCompositeContext(string resolvedPath, string fileName, string? effectiveSearchDirectory, int loaderMode, object? driver08, object? link0C)
        {
            int stableHash = ComputeStableHash(resolvedPath);
            long fileLength = File.Exists(resolvedPath) ? new FileInfo(resolvedPath).Length : 0L;
            int fileSeed = unchecked((int)(fileLength ^ (fileLength >> 32)));
            int seed = stableHash ^ fileSeed ^ (loaderMode << 12);

            int descriptorId = stableHash != 0 ? stableHash : 1;
            byte entryCount = (byte)(1 + ((seed >> 5) & 0x03));
            ushort groupCount = (ushort)(1 + ((seed >> 9) & 0x03));

            if (loaderMode == 2)
            {
                entryCount = (byte)Math.Max(entryCount, (byte)2);
            }

            Gedx8CompositeSourceDescriptor source04 = new(fileName, resolvedPath, effectiveSearchDirectory, loaderMode, descriptorId);
            uint[] table = BuildCompositeTable(seed, entryCount, groupCount);

            return new Gedx8CompositeContext(source04, driver08, link0C, descriptorId, entryCount, groupCount, resolvedPath, effectiveSearchDirectory, table);
        }

        private bool InitializeThinType1Load10003890(Gedx8LoadedObject loadedObject, string fileName, string resolvedPath, string? effectiveSearchDirectory, int loaderMode, object? nativeContext08)
        {
            loadedObject.Sub10002D30(1, loaderMode, CreateThinRuntime(Gedx8ObjectKind.ThinType1, fileName, resolvedPath, effectiveSearchDirectory, loaderMode, "1000C268", nativeContext08));
            return true;
        }

        private bool InitializeThinType2Load10003890(Gedx8LoadedObject loadedObject, string fileName, string resolvedPath, string? effectiveSearchDirectory, int loaderMode, object? nativeContext08)
        {
            loadedObject.Sub10002D30(2, loaderMode, CreateThinRuntime(Gedx8ObjectKind.ThinType2, fileName, resolvedPath, effectiveSearchDirectory, loaderMode, "1000C218", nativeContext08));
            return true;
        }

        private static Gedx8ThinRuntime CreateThinRuntime(Gedx8ObjectKind kind, string fileName, string resolvedPath, string? effectiveSearchDirectory, int loaderMode, string staticBindingToken04, object? nativeContext08)
        {
            Gedx8ThinBindingHandle nativeObject04 = new(kind, staticBindingToken04, fileName, resolvedPath, effectiveSearchDirectory, loaderMode);
            return new Gedx8ThinRuntime(kind, fileName, resolvedPath, effectiveSearchDirectory, loaderMode, staticBindingToken04, nativeObject04, nativeContext08);
        }

        private static uint[] BuildCompositeTable(int seed, byte entryCount, ushort groupCount)
        {
            int total = entryCount * groupCount;
            uint[] table = new uint[total];

            for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
            {
                for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
                {
                    int flatIndex = (groupIndex * entryCount) + entryIndex;
                    int rotated = RotateLeft(seed ^ (groupIndex * 0x45D9F3B) ^ (entryIndex * 0x119DE1F3), (flatIndex % 13) + 1);
                    int baseValue = (rotated & 0x7FFF) + (groupIndex * 0x40) + (entryIndex * 0x10);
                    int windowLength = 1 + ((rotated >> 7) & 0x0F);
                    int stepValue = 1 + ((rotated >> 15) & 0x0F);

                    table[flatIndex] = ((uint)(baseValue & 0xFFFF) << 16) |
                        ((uint)((windowLength - 1) & 0xFF) << 8) |
                        (uint)((stepValue - 1) & 0xFF);
                }
            }

            return table;
        }

        private static int RotateLeft(int value, int count)
        {
            return unchecked((value << count) | ((int)((uint)value >> (32 - count))));
        }

        private static int ComputeStableHash(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            int result = 17;
            for (int index = 0; index < value.Length; index++)
            {
                result = unchecked((result * 31) + char.ToUpperInvariant(value[index]));
            }

            return result & 0x7FFFFFFF;
        }

        private static int ComputeStableHashStatic(string? value)
        {
            return ComputeStableHash(value);
        }

        private void ResetResolvedPropertyCache()
        {
            Array.Clear(_propertyValues17C);
            Array.Clear(_propertyResolved17C);
            Array.Clear(_cachedObjects148);
        }

        private bool TryInvokeSelectionBinding(int selection, int mode)
        {
            object? currentObject = _value1C0;
            if (currentObject == null)
            {
                return false;
            }

            if (ReferenceEquals(currentObject, this))
            {
                return true;
            }

            if (currentObject is Gedx8LoadedObject loadedObject)
            {
                if (!loadedObject.IsActive)
                {
                    return false;
                }

                _value1C0 = loadedObject;
                return true;
            }

            return true;
        }

        private bool TryResolvePropertySelector(int selector)
        {
            if (_propertyResolved17C[selector])
            {
                return true;
            }

            Gedx8LazyPropertyObject? propertyObject = CreateLazyPropertyObject(selector);
            if (propertyObject == null)
            {
                return false;
            }

            if (!IsModeAllowed(propertyObject.ModeRequirement))
            {
                return false;
            }

            if (!propertyObject.Initialize(this))
            {
                return false;
            }

            _cachedObjects148[selector] = propertyObject;
            _propertyResolved17C[selector] = true;
            _propertyValues17C[selector] = propertyObject.CurrentValue;
            return true;
        }

        private Gedx8LazyPropertyObject? CreateLazyPropertyObject(int selector)
        {
            return selector switch
            {
                0 => new Gedx8LazyPropertyObject(0, 0x148, 0x17C, 0x18, 0x3C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false),
                1 => new Gedx8LazyPropertyObject(1, 0x14C, 0x180, 0x1C, 0x40, Gedx8PropertyModeRequirement.Mode2Or3, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false),
                2 => new Gedx8LazyPropertyObject(2, 0x150, 0x184, 0x20, 0x44, Gedx8PropertyModeRequirement.Mode2Or3, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false),
                3 => new Gedx8LazyPropertyObject(3, 0x154, 0x188, 0x28, 0x4C, Gedx8PropertyModeRequirement.Mode0, Gedx8PropertyInitializationKind.Simple02C60, "1000C548", false, true),
                4 => new Gedx8LazyPropertyObject(4, 0x158, 0x18C, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C528", true, false),
                5 => new Gedx8LazyPropertyObject(5, 0x15C, 0x190, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4E8", true, false),
                6 => new Gedx8LazyPropertyObject(6, 0x160, 0x194, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4F8", true, false),
                7 => new Gedx8LazyPropertyObject(7, 0x164, 0x198, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C508", true, false),
                8 => new Gedx8LazyPropertyObject(8, 0x168, 0x19C, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C518", true, false),
                9 => new Gedx8LazyPropertyObject(9, 0x16C, 0x1A0, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C538", true, false),
                10 => new Gedx8LazyPropertyObject(10, 0x170, 0x1A4, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4C8", true, false),
                11 => new Gedx8LazyPropertyObject(11, 0x174, 0x1A8, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4D8", true, false),
                12 => new Gedx8LazyPropertyObject(12, 0x178, 0x1AC, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4B8", true, false),
                _ => null,
            };
        }

        private bool IsModeAllowed(Gedx8PropertyModeRequirement modeRequirement)
        {
            return modeRequirement switch
            {
                Gedx8PropertyModeRequirement.None => true,
                Gedx8PropertyModeRequirement.Mode0 => _value1B0 == 0,
                Gedx8PropertyModeRequirement.Mode2Or3 => _value1B0 == 2 || _value1B0 == 3,
                _ => false,
            };
        }

        private bool TryCreateSimplePropertyObject10002C60(Gedx8LazyPropertyObject propertyObject)
        {
            if (_disposed || _value1C0 == null)
            {
                _lastPropertyCreateResult = -1;
                return false;
            }

            _simplePropertyCreateCount02C60++;
            _lastPropertyCreateTokenHash = ComputeStableHash(propertyObject.InitializationToken);
            _lastPropertyFactoryTokenHash = ComputeStableHash("1000C1C8");
            _lastPropertyCreateTargetOffset = propertyObject.CachedObjectOffset;
            _lastPropertyCreateResult = 0;

            propertyObject.BindCreateResult(_lastPropertyCreateTokenHash, _lastPropertyFactoryTokenHash, _simplePropertyCreateCount02C60, _lastPropertyCreateTargetOffset);
            return true;
        }

        private bool TryCreateTypedPropertyObject10002C90(Gedx8LazyPropertyObject propertyObject)
        {
            if (_disposed || _value1C0 == null)
            {
                _lastPropertyCreateResult = -1;
                return false;
            }

            if (_typedDescriptorCount01 >= _typedDescriptors124.Length)
            {
                _lastPropertyCreateResult = -1;
                return false;
            }

            int descriptorOrdinal = _typedDescriptorCount01 + 1;
            int descriptorTableOffset = 0x124 + (_typedDescriptorCount01 * 0x20);

            Gedx8TypedPropertyDescriptor descriptor = new Gedx8TypedPropertyDescriptor(
                0x20,
                propertyObject.Selector,
                propertyObject.CachedObjectOffset,
                propertyObject.ValueOffset,
                propertyObject.GetMethodOffset,
                propertyObject.SetMethodOffset,
                ComputeStableHash(propertyObject.InitializationToken),
                ComputeStableHash("1000C1C8"),
                descriptorOrdinal,
                descriptorTableOffset,
                0x54,
                0x5C);

            _typedDescriptors124[_typedDescriptorCount01] = descriptor;
            _typedDescriptorCount01++;
            _typedPropertyCreateCount02C90++;
            _lastPropertyCreateTokenHash = descriptor.TokenHash;
            _lastPropertyFactoryTokenHash = descriptor.FactoryTokenHash;
            _lastPropertyCreateTargetOffset = descriptorTableOffset;
            _lastPropertyCreateResult = 0;

            propertyObject.BindTypedDescriptor(descriptor, _typedPropertyCreateCount02C90);
            return true;
        }

        private int ReadPropertyBackingValue(int selector)
        {
            return selector switch
            {
                3 => _selector3Triple188[0],
                4 => _selector3Triple188[1],
                5 => _selector3Triple188[2],
                >= 6 and <= 12 => _typedSelectorBackings18C[selector - 4],
                _ => 0,
            };
        }

        private void WritePropertyBackingValue(int selector, int value)
        {
            switch (selector)
            {
                case 3:
                    _selector3Triple188[0] = value;
                    _typedSelectorBackings18C[0] = _selector3Triple188[1];
                    _typedSelectorBackings18C[1] = _selector3Triple188[2];
                    break;

                case 4:
                    _selector3Triple188[1] = value;
                    _typedSelectorBackings18C[0] = value;
                    break;

                case 5:
                    _selector3Triple188[2] = value;
                    _typedSelectorBackings18C[1] = value;
                    break;

                default:
                    if (selector >= 6 && selector <= 12)
                    {
                        _typedSelectorBackings18C[selector - 4] = value;
                    }
                    break;
            }
        }

        private bool TryReadResolvedProperty(int selector, out int storedValue)
        {
            storedValue = 0;

            if (!_propertyResolved17C[selector])
            {
                return false;
            }

            if (_cachedObjects148[selector] is not Gedx8LazyPropertyObject propertyObject)
            {
                return false;
            }

            if (!propertyObject.TryRead(this, out storedValue))
            {
                return false;
            }

            _propertyValues17C[selector] = storedValue;
            return true;
        }

        private bool TryWriteResolvedProperty(int selector, int value)
        {
            if (!_propertyResolved17C[selector])
            {
                return false;
            }

            if (_cachedObjects148[selector] is not Gedx8LazyPropertyObject propertyObject)
            {
                return false;
            }

            if (!IsModeAllowed(propertyObject.ModeRequirement))
            {
                return false;
            }

            if (!propertyObject.TryWrite(this, value))
            {
                return false;
            }

            _propertyValues17C[selector] = propertyObject.CurrentValue;
            return true;
        }

        private sealed class Gedx8LazyPropertyObject
        {
            private readonly int[] _tripleValues;

            internal Gedx8LazyPropertyObject(int selector, int cachedObjectOffset, int valueOffset, int getMethodOffset, int setMethodOffset, Gedx8PropertyModeRequirement modeRequirement, Gedx8PropertyInitializationKind initializationKind, string initializationToken, bool bootstrapReadAfterCreate, bool isTripleHead)
            {
                Selector = selector;
                CachedObjectOffset = cachedObjectOffset;
                ValueOffset = valueOffset;
                GetMethodOffset = getMethodOffset;
                SetMethodOffset = setMethodOffset;
                ModeRequirement = modeRequirement;
                InitializationKind = initializationKind;
                InitializationToken = initializationToken;
                BootstrapReadAfterCreate = bootstrapReadAfterCreate;
                IsTripleHead = isTripleHead;
                _tripleValues = new int[3];
            }

            internal int Selector { get; }

            internal int CachedObjectOffset { get; }

            internal int ValueOffset { get; }

            internal int GetMethodOffset { get; }

            internal int SetMethodOffset { get; }

            internal Gedx8PropertyModeRequirement ModeRequirement { get; }

            internal Gedx8PropertyInitializationKind InitializationKind { get; }

            internal string InitializationToken { get; }

            internal bool BootstrapReadAfterCreate { get; }

            internal bool IsTripleHead { get; }

            internal bool IsInitialized { get; private set; }

            internal int CurrentValue { get; set; }

            internal int CreationOrdinal { get; private set; }

            internal int CreationTokenHash { get; private set; }

            internal int CreationFactoryTokenHash { get; private set; }

            internal int CreationTargetOffset { get; private set; }

            internal Gedx8TypedPropertyDescriptor TypedDescriptor { get; private set; }

            internal bool Initialize(Gedx8EngineCore owner)
            {
                if (IsInitialized)
                {
                    return true;
                }

                bool created = InitializationKind switch
                {
                    Gedx8PropertyInitializationKind.Simple02C60 => owner.TryCreateSimplePropertyObject10002C60(this),
                    Gedx8PropertyInitializationKind.Typed02C90 => owner.TryCreateTypedPropertyObject10002C90(this),
                    _ => false,
                };

                if (!created)
                {
                    return false;
                }

                CurrentValue = owner.ReadPropertyBackingValue(Selector);

                if (IsTripleHead)
                {
                    _tripleValues[0] = owner._selector3Triple188[0];
                    _tripleValues[1] = owner._selector3Triple188[1];
                    _tripleValues[2] = owner._selector3Triple188[2];
                    CurrentValue = _tripleValues[0];
                }

                IsInitialized = true;
                return true;
            }

            internal void BindCreateResult(int creationTokenHash, int creationFactoryTokenHash, int creationOrdinal, int creationTargetOffset)
            {
                CreationTokenHash = creationTokenHash;
                CreationFactoryTokenHash = creationFactoryTokenHash;
                CreationOrdinal = creationOrdinal;
                CreationTargetOffset = creationTargetOffset;
                TypedDescriptor = default;
            }

            internal void BindTypedDescriptor(Gedx8TypedPropertyDescriptor typedDescriptor, int creationOrdinal)
            {
                TypedDescriptor = typedDescriptor;
                CreationTokenHash = typedDescriptor.TokenHash;
                CreationFactoryTokenHash = typedDescriptor.FactoryTokenHash;
                CreationOrdinal = creationOrdinal;
                CreationTargetOffset = typedDescriptor.DescriptorTableOffset;
            }

            internal bool TryRead(Gedx8EngineCore owner, out int value)
            {
                value = 0;
                if (!IsInitialized)
                {
                    return false;
                }

                if (IsTripleHead)
                {
                    _tripleValues[0] = owner._selector3Triple188[0];
                    _tripleValues[1] = owner._selector3Triple188[1];
                    _tripleValues[2] = owner._selector3Triple188[2];
                    CurrentValue = _tripleValues[0];
                }
                else
                {
                    CurrentValue = owner.ReadPropertyBackingValue(Selector);
                }

                value = CurrentValue;
                return true;
            }

            internal bool TryWrite(Gedx8EngineCore owner, int value)
            {
                if (!IsInitialized)
                {
                    return false;
                }

                if (IsTripleHead)
                {
                    _tripleValues[0] = value;
                    owner._selector3Triple188[0] = _tripleValues[0];
                    owner._selector3Triple188[1] = _tripleValues[1];
                    owner._selector3Triple188[2] = _tripleValues[2];
                    owner.WritePropertyBackingValue(3, _tripleValues[0]);
                    CurrentValue = _tripleValues[0];

                    if (owner._cachedObjects148[4] is Gedx8LazyPropertyObject selector4 && selector4.IsInitialized)
                    {
                        selector4.CurrentValue = owner.ReadPropertyBackingValue(4);
                        owner._propertyValues17C[4] = selector4.CurrentValue;
                    }

                    if (owner._cachedObjects148[5] is Gedx8LazyPropertyObject selector5 && selector5.IsInitialized)
                    {
                        selector5.CurrentValue = owner.ReadPropertyBackingValue(5);
                        owner._propertyValues17C[5] = selector5.CurrentValue;
                    }

                    return true;
                }

                owner.WritePropertyBackingValue(Selector, value);
                CurrentValue = owner.ReadPropertyBackingValue(Selector);
                return true;
            }
        }

        private readonly record struct Gedx8CompositeSourceDescriptor(string FileName, string ResolvedPath, string? SearchDirectory, int LoaderMode, int DescriptorId);

        private readonly record struct Gedx8CompositeContext(Gedx8CompositeSourceDescriptor Source04, object? Driver08, object? Link0C, int DescriptorId14, byte EntryCount1A, ushort GroupCount18, string ResolvedPath, string? SearchDirectory, uint[] Table10);

        private readonly record struct Gedx8TypedPropertyDescriptor(int EntrySize, int Selector, int CachedObjectOffset, int ValueOffset, int GetMethodOffset, int SetMethodOffset, int TokenHash, int FactoryTokenHash, int Ordinal, int DescriptorTableOffset, int FinalizeMethodOffset, int CommitMethodOffset);

        private enum Gedx8PropertyModeRequirement
        {
            None = 0,
            Mode0 = 1,
            Mode2Or3 = 2,
        }

        private enum Gedx8PropertyInitializationKind
        {
            None = 0,
            Simple02C60 = 1,
            Typed02C90 = 2,
        }
    }
}
