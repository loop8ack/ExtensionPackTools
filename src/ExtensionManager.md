# ExtensionManager

These projects constitute the actual core of the solution.

They are developed in the SDK style to simplify future development and better support newer Visual Studio and language features.

## Projects

| Project | Description |
|---|---|
| ExtensionManager | The main project that contains the concrete functionalities of this extension |
| ExtensionManager.Manifest | Contains code for reading and writing the manifest JSON file and supports versioning |
| ExtensionManager.UI | Contains the entire user interface using the MVVM pattern |
| ExtensionManager.Shared | Contains shared code. This should only include attribute classes from newer .NET versions to support newer C# features |
