# ExtensionManager.VisualStudio

These projects provide a facade around the Visual Studio API, streamlining the handling of different dependency versions.

## Problems & Solutions

#### Version conflicts

Different major versions of Visual Studio require specific dependencies that may not be compatible with each other.
To manage this, separate projects are established for each Visual Studio version to specify their required dependency versions.
A shared project is also maintained to house the common code applicable across all versions.

#### API Facade

To simplify the codebase in SDK-style projects, direct interaction with the Visual Studio API is limited, due to the need for specific versions of dependencies across different Visual Studio versions.

An API facade abstracts away the complexity of the Visual Studio API, enabling the bulk of the code to interact with this facade rather than directly with varying versions of dependencies.

## Projects

| Project | Description |
|---|---|
| ExtensionManager.VisualStudio.Abstractions | Houses the facade abstraction |
| ExtensionManager.VisualStudio.VS2017 | Specifies dependencies for projects targeting Visual Studio 2017 |
| ExtensionManager.VisualStudio.VS2019 | Specifies dependencies for projects targeting Visual Studio 2019 |
| ExtensionManager.VisualStudio.VS2022 | Specifies dependencies for projects targeting Visual Studio 2022 |
| ExtensionManager.VisualStudio.Shared | Contains the implementation of the abstractions |
