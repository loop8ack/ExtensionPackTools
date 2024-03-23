using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI;

public record struct VSExtensionToInstall(IVSExtension Extension, VSExtensionStatus Status);
