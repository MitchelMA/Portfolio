using Portfolio.Shared.Components.Lightbox;

namespace Portfolio.Services;

public class LightboxRegistry
{
    private Dictionary<string, Lightbox> _registry = new();

    public bool TryRegister(string name, Lightbox box)
    {
        if (_registry.ContainsKey(name))
            return false;
        
        _registry.Add(name, box);
        box.SetImages();
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