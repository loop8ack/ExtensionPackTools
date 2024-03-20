# ExtensionManager.VisualStudio

These projects provide a facade around the Visual Studio API, enabling a simple solution for conflicts between DLLs of different Visual Studio versions.

## Problems & Solutions

#### Version conflicts

The preview version of Visual Studio internally uses modified implementations that are not compatible with the normal version.

To solve this problem, there are separate projects for each version that define divergent code, as well as a shared project that defines common code.
In particular, retrieving data from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/) poses a challenge because it requires a `GalleryExtension` class,
whose definition and content, however, differ.

#### API Facade

To manage the majority of the code in regular SDK-style projects, direct usage of the Visual Studio API is not allowed,
as different Visual Studio versions require different dependencies.

The solution is to use a facade that abstracts the entire Visual Studio API, allowing the majority of the code to only depend on this facade, not the concrete API.

The `VSFacade` class should be used in a similar manner as the `VS` class from [Community.VisualStudio.Toolkit](https://github.com/VsixCommunity/Community.VisualStudio.Toolkit),
but it is extended with additional functions.

## Projects

| Project | Description |
|---|---|
| ExtensionManager.VisualStudio.Abstractions | Provides the abstraction for the facade and includes the `VSFacade` class |
| ExtensionManager.VisualStudio.V15 | Divergent implementation and referenced DLLs for Visual Studio 2017 |
| ExtensionManager.VisualStudio.V16 | Divergent implementation and referenced DLLs for Visual Studio 2019 |
| ExtensionManager.VisualStudio.V17 | Divergent implementation and referenced DLLs for Visual Studio 2022 |
| ExtensionManager.VisualStudio.Shared | Contains the shared code and the implementation of the `VSFacade` class. |
