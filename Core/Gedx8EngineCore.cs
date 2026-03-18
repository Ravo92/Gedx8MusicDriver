using Gedx8MusicDriver.Interop;
using Gedx8MusicDriver.Models;

namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8EngineCore : IDisposable
    {
        private readonly int[] _propertyValues17C = new int[13];
        private readonly bool[] _propertyResolved17C = new bool[13];
        private readonly object?[] _cachedObjects148 = new object?[13];
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

            SynthInitConfig = new Gedx8SynthInitConfig(0x28, config.SampleRate, config.Config);
            IsSynthInitialized = true;
            _value00 = 0;
            _value04 = 0;
            _value08 = 0;
            _value1B8 = 0;
            _value1BC = 0;

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
                    loadedObject.Sub10002D30(1, loaderMode, resolvedPath);
                    break;

                case Gedx8ObjectKind.ThinType2:
                    loadedObject.Sub10002D30(2, loaderMode, resolvedPath);
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

            if (setMode)
            {
                if (!TryWriteResolvedProperty(selector, value))
                {
                    return false;
                }
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

        private static uint[] BuildCompositeTable(int seed, byte entryCount, ushort groupCount)
        {
            int totalCount = entryCount * groupCount;
            uint[] table = new uint[totalCount];

            for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
            {
                for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
                {
                    int flatIndex = (groupIndex * entryCount) + entryIndex;
                    int rotatedSeed = RotateLeft(seed ^ (groupIndex * 0x1F1F) ^ (entryIndex * 0x0101), flatIndex & 15);

                    int baseValue = ((rotatedSeed >> 8) & 0x0FFF) + (groupIndex * 0x40) + (entryIndex * 0x10);
                    int windowLength = 1 + ((rotatedSeed >> 4) & 0x07);
                    int stepValue = 4 + ((rotatedSeed >> 1) & 0x0F);

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

            Gedx8ResolvedProperty binding;
            switch (selector)
            {
                case 0:
                    binding = new Gedx8ResolvedProperty(selector, 0x148, 0x17C, 0x18, 0x3C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false);
                    break;

                case 1:
                    binding = new Gedx8ResolvedProperty(selector, 0x14C, 0x180, 0x1C, 0x40, Gedx8PropertyModeRequirement.Mode2Or3, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false);
                    break;

                case 2:
                    binding = new Gedx8ResolvedProperty(selector, 0x150, 0x184, 0x20, 0x44, Gedx8PropertyModeRequirement.Mode2Or3, Gedx8PropertyInitializationKind.Simple02C60, "1000C558", false, false);
                    break;

                case 3:
                    binding = new Gedx8ResolvedProperty(selector, 0x154, 0x188, 0x28, 0x4C, Gedx8PropertyModeRequirement.Mode0, Gedx8PropertyInitializationKind.Simple02C60, "1000C548", false, true);
                    break;

                case 4:
                    binding = new Gedx8ResolvedProperty(selector, 0x158, 0x18C, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C528", true, false);
                    break;

                case 5:
                    binding = new Gedx8ResolvedProperty(selector, 0x15C, 0x190, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4E8", true, false);
                    break;

                case 6:
                    binding = new Gedx8ResolvedProperty(selector, 0x160, 0x194, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4F8", true, false);
                    break;

                case 7:
                    binding = new Gedx8ResolvedProperty(selector, 0x164, 0x198, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C508", true, false);
                    break;

                case 8:
                    binding = new Gedx8ResolvedProperty(selector, 0x168, 0x19C, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C518", true, false);
                    break;

                case 9:
                    binding = new Gedx8ResolvedProperty(selector, 0x16C, 0x1A0, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C538", true, false);
                    break;

                case 10:
                    binding = new Gedx8ResolvedProperty(selector, 0x170, 0x1A4, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4C8", true, false);
                    break;

                case 11:
                    binding = new Gedx8ResolvedProperty(selector, 0x174, 0x1A8, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4D8", true, false);
                    break;

                case 12:
                    binding = new Gedx8ResolvedProperty(selector, 0x178, 0x1AC, 0x10, 0x0C, Gedx8PropertyModeRequirement.None, Gedx8PropertyInitializationKind.Typed02C90, "1000C4B8", true, false);
                    break;

                default:
                    return false;
            }

            if (!IsModeAllowed(binding.ModeRequirement))
            {
                return false;
            }

            if (!TryInitializeResolvedProperty(binding))
            {
                return false;
            }

            _cachedObjects148[selector] = binding;
            _propertyResolved17C[selector] = true;
            return true;
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

        private bool TryInitializeResolvedProperty(Gedx8ResolvedProperty binding)
        {
            if (binding.InitializationKind == Gedx8PropertyInitializationKind.Typed02C90)
            {
                _propertyValues17C[binding.Selector] = ComputeBootstrapPropertyValue(binding.Selector);
            }
            else if (binding.IsTripleHead)
            {
                _propertyValues17C[binding.Selector] = 0;
            }

            return true;
        }

        private int ComputeBootstrapPropertyValue(int selector)
        {
            object? currentObject = _value1C0;
            string? seedSource = currentObject switch
            {
                Gedx8LoadedObject loadedObject => loadedObject.ResolvedPath ?? loadedObject.FileName,
                _ => SearchDirectory,
            };

            if (string.IsNullOrWhiteSpace(seedSource))
            {
                return 0;
            }

            int seed = ComputeStableHash(seedSource);
            int rotated = RotateLeft(seed ^ (selector * 0x1111), selector & 15);
            return rotated & 0x7FFFFFFF;
        }

        private bool TryReadResolvedProperty(int selector, out int storedValue)
        {
            storedValue = 0;

            if (!_propertyResolved17C[selector])
            {
                return false;
            }

            storedValue = _propertyValues17C[selector];
            return true;
        }

        private bool TryWriteResolvedProperty(int selector, int value)
        {
            if (!_propertyResolved17C[selector])
            {
                return false;
            }

            object? cachedObject = _cachedObjects148[selector];
            if (cachedObject is not Gedx8ResolvedProperty binding)
            {
                return false;
            }

            if (!IsModeAllowed(binding.ModeRequirement))
            {
                return false;
            }

            _propertyValues17C[selector] = value;
            return true;
        }

        private enum Gedx8PropertyModeRequirement
        {
            None = 0,
            Mode0 = 1,
            Mode2Or3 = 2,
        }

        private enum Gedx8PropertyInitializationKind
        {
            Simple02C60 = 0,
            Typed02C90 = 1,
        }

        private sealed class Gedx8ResolvedProperty
        {
            internal Gedx8ResolvedProperty(int selector, int cachedObjectOffset, int valueOffset, int getMethodOffset, int setMethodOffset, Gedx8PropertyModeRequirement modeRequirement, Gedx8PropertyInitializationKind initializationKind, string initializationToken, bool bootstrapReadAfterCreate, bool isTripleHead)
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
        }
    }

    internal sealed class Gedx8CompositeContext
    {
        internal Gedx8CompositeContext(object source04, object? driver08, object? link0C, int descriptorId14, byte entryCount1A, ushort groupCount18, string resolvedPath, string? searchDirectory, uint[] table10)
        {
            Source04 = source04;
            Driver08 = driver08;
            Link0C = link0C;
            DescriptorId14 = descriptorId14;
            EntryCount1A = entryCount1A;
            GroupCount18 = groupCount18;
            ResolvedPath = resolvedPath;
            SearchDirectory = searchDirectory;
            Table10 = table10;
        }

        internal object Source04 { get; }

        internal object? Driver08 { get; }

        internal object? Link0C { get; }

        internal int DescriptorId14 { get; }

        internal byte EntryCount1A { get; }

        internal ushort GroupCount18 { get; }

        internal string ResolvedPath { get; }

        internal string? SearchDirectory { get; }

        internal uint[] Table10 { get; }
    }

    internal sealed class Gedx8CompositeSourceDescriptor
    {
        internal Gedx8CompositeSourceDescriptor(string fileName, string resolvedPath, string? searchDirectory, int loaderMode08, int descriptorId14)
        {
            FileName = fileName;
            ResolvedPath = resolvedPath;
            SearchDirectory = searchDirectory;
            LoaderMode08 = loaderMode08;
            DescriptorId14 = descriptorId14;
        }

        internal string FileName { get; }

        internal string ResolvedPath { get; }

        internal string? SearchDirectory { get; }

        internal int LoaderMode08 { get; }

        internal int DescriptorId14 { get; }
    }
}
