using System;

namespace ExtensionManager
{
    public readonly struct InstalledExtension
    {
        public InstalledExtension(string identifier, bool systemComponent, bool isPackComponent)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            SystemComponent = systemComponent;
            IsPackComponent = isPackComponent;
        }

        public string Identifier { get; }
        public bool SystemComponent { get; }
        public bool IsPackComponent { get; }
    }
}
