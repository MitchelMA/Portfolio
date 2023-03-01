using Portfolio.Shared.Components.Lightbox;

namespace Portfolio.Services;

public class LightboxRegistry
{
    public delegate void RegisteredDelegate(LightboxRegistry sender, Lightbox lightbox);
    public event RegisteredDelegate? Registered;
    
    private readonly Dictionary<string, Lightbox> _registry = new();

    public bool TryRegister(string name, Lightbox box)
    {
        if (_registry.ContainsKey(name))
            return false;
        
        _registry.Add(name, box);
        Registered?.Invoke(this, box);
        return true;
    }

    public (bool succes, Lightbox? box) Get(string name)
    {
        if (!_registry.ContainsKey(name))
            return (false, default);

        return (true, _registry[name]);
    }

    public bool IsRegistered(string name) => _registry.ContainsKey(name);
}