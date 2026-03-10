using Gedx8MusicDriver.Api;

namespace Gedx8MusicDriver.Core
{
    internal sealed class Gedx8GlobalRegistry
    {
        private readonly Gedx8SlotTable<Gedx8DriverInstance> _instances = new(1);

        internal bool IsBootstrapped { get; set; }

        internal void Register(Gedx8DriverInstance instance)
        {
            _instances.Add(instance);
        }

        internal void Unregister(Gedx8DriverInstance instance)
        {
            _instances.Remove(instance);
        }

        internal void Reset()
        {
            IReadOnlyList<Gedx8DriverInstance> liveInstances = _instances.GetLiveObjects();
            for (int i = 0; i < liveInstances.Count; i++)
            {
                liveInstances[i].ShutdownFromRegistry();
            }

            _instances.Clear();
            IsBootstrapped = false;
        }
    }
}
