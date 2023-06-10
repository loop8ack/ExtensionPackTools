# ExtensionManager.Vsix

These projects represent the actual extensions.
They are separated from all other projects as they should contain minimal custom logic.
Their sole purpose is to define the VSIX-specific contents and configurations.

## Projects

| Project | Description |
|---|---|
| ExtensionManager.Vsix.2017 | The Visual Studio 2017 project |
| ExtensionManager.Vsix.2019 | The Visual Studio 2019 project |
| ExtensionManager.Vsix.2022 | The Visual Studio 2022 project |
| ExtensionManager.Vsix.Shared | Contains the code that is not possible in SDK projects or is not worth outsourcing. This should only include the package implementation and the AssemblyInfo.cs file. |
