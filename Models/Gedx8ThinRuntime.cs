namespace Gedx8MusicDriver.Models
{
    internal sealed class Gedx8ThinRuntime
    {
        internal Gedx8ThinRuntime(Gedx8ObjectKind kind, string fileName, string resolvedPath, string? searchDirectory, int loaderMode08, string bindingToken, int descriptorToken, object? owner08, object? helper0C)
        {
            Kind = kind;
            FileName = fileName;
            ResolvedPath = resolvedPath;
            SearchDirectory = searchDirectory;
            LoaderMode08 = loaderMode08;
            BindingToken = bindingToken;
            DescriptorToken = descriptorToken;
            Owner08 = owner08;
            Helper0C = helper0C;
        }

        internal Gedx8ObjectKind Kind { get; }

        internal string FileName { get; }

        internal string ResolvedPath { get; }

        internal string? SearchDirectory { get; }

        internal int LoaderMode08 { get; }

        internal string BindingToken { get; }

        internal int DescriptorToken { get; }

        internal object? Owner08 { get; }

        internal object? Helper0C { get; }

        internal string? LastOpenedName { get; private set; }

        internal void SetLastOpenedName(string? value)
        {
            LastOpenedName = value;
        }
    }
}
