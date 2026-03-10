namespace Gedx8MusicDriver.Models
{
    public readonly record struct Gedx8SynthInitConfig(int Reserved00, int SampleRate, int Config)
    {
        public static Gedx8SynthInitConfig Empty => new(0, 0, 0);
    }
}
