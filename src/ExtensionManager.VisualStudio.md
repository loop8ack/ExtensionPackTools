# ExtensionManager.VisualStudio

These projects provide a facade around the Visual Studio API, streamlining the handling of different dependency versions.

## Proble & Solution

#### Version conflicts

Different major versions of Visual Studio require specific dependencies that may not be compatible with each other.

To manage this, an Abstractions project is created to define interfaces, which are then implemented in the various Vsix projects.
The implementation code of these abstractions is housed in a Shared project. This approach allows normal SDK projects to utilize the
functionality without needing to directly manage the dependencies for each Visual Studio version.

## Projects

| Project | Description |
|---|---|
| ExtensionManager.VisualStudio.Abstractions | Houses the facade abstraction |
| ExtensionManager.VisualStudio.Shared | Contains the implementation of the abstractions |
