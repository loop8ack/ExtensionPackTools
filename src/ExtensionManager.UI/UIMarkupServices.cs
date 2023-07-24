using System;

using ExtensionManager.UI.Attached;
using ExtensionManager.VisualStudio.Themes;

using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.UI;

public static class UIMarkupServices
{
    public static void Initialize(IServiceProvider services)
    {
        VSTheme.Initialize(services.GetRequiredService<IVSThemes>());
    }
}
