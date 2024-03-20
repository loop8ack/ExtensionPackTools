using System;
using System.Reflection;

using ExtensionManager.VisualStudio.Adapter.Generator.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator;

internal static class ReflectedMemberSourceExtensions
{
    public static Type VS(this ReflectedMemberSource source) => source.GetType("Community.VisualStudio.Toolkit.VS");
    public static Type IInstalledExtension(this ReflectedMemberSource source) => source.GetType("Microsoft.VisualStudio.ExtensionManager.IInstalledExtension");
    public static Type IVsExtensionManager(this ReflectedMemberSource source) => source.GetType("Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager");
    public static Type SVsExtensionManager(this ReflectedMemberSource source) => source.GetType("Microsoft.VisualStudio.ExtensionManager.SVsExtensionManager");
    public static Type IVsExtensionRepository(this ReflectedMemberSource source) => source.GetType("Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository");
    public static Type SVsExtensionRepository(this ReflectedMemberSource source) => source.GetType("Microsoft.VisualStudio.ExtensionManager.SVsExtensionRepository");

    public static MethodInfo VS_GetRequiredServiceAsync(this ReflectedMemberSource source, Type serviceType, Type interfaceType)
        => source.VS().GetMethodOrThrow("GetRequiredServiceAsync").MakeGenericMethod(serviceType, interfaceType);

    public static MethodInfo IVsExtensionManager_GetInstalledExtensions(this ReflectedMemberSource source)
        => source.IVsExtensionManager().GetMethodOrThrow("GetInstalledExtensions");

    public static MethodInfo IVsExtensionRepository_GetVSGalleryExtensions(this ReflectedMemberSource source, Type galleryExtensionType)
        => source.IVsExtensionRepository().GetMethodOrThrow("GetVSGalleryExtensions").MakeGenericMethod(galleryExtensionType);
}
