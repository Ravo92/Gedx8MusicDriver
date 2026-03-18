namespace Gedx8MusicDriver.Models
{
    internal sealed class Gedx8ThinRuntime
    {
        internal Gedx8ThinRuntime(Gedx8ObjectKind kind, string fileName, string resolvedPath, string? searchDirectory, int loaderMode08, string staticBindingToken04, object? nativeObject04, object? nativeContext08)
        {
            Kind = kind;
            FileName = fileName;
            ResolvedPath = resolvedPath;
            SearchDirectory = searchDirectory;
            LoaderMode08 = loaderMode08;
            StaticBindingToken04 = staticBindingToken04;
            NativeObject04 = nativeObject04;
            NativeContext08 = nativeContext08;
        }

        internal Gedx8ObjectKind Kind { get; }

        internal string FileName { get; }

        internal string ResolvedPath { get; }

        internal string? SearchDirectory { get; }

        internal int LoaderMode08 { get; }

        internal int NativeSize0C => 0x0C;

        internal string StaticBindingToken04 { get; }

        internal object? NativeObject04 { get; }

        internal object? NativeContext08 { get; }

        internal string? LastOpenedName { get; private set; }

        internal void SetLastOpenedName(string? value)
        {
            LastOpenedName = value;
        }
    }

    internal sealed class Gedx8ThinBindingHandle
    {
        internal Gedx8ThinBindingHandle(Gedx8ObjectKind kind, string staticBindingToken04, string fileName, string resolvedPath, string? searchDirectory, int loaderMode08)
        {
            Kind = kind;
            StaticBindingToken04 = staticBindingToken04;
            FileName = fileName;
            ResolvedPath = resolvedPath;
            SearchDirectory = searchDirectory;
            LoaderMode08 = loaderMode08;
        }

        internal Gedx8ObjectKind Kind { get; }

        internal string StaticBindingToken04 { get; }

        internal string FileName { get; }

        internal string ResolvedPath { get; }

        internal string? SearchDirectory { get; }

        internal int LoaderMode08 { get; }
    }
}
