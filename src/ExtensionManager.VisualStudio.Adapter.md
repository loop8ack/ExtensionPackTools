# ExtensionManager.VisualStudio.Adapter

The primary goal of these projects is to ensure seamless compatibility with various versions and updates of Visual Studio.
This aims to reduce the need for frequent updates and maintenance.

## Problems & Solutions

Different versions of Visual Studio, including full and preview versions, come with different internal DLLs.
Since these DLLs are part of active development, they can vary between versions, posing a challenge not only for supporting the latest updates but also for older versions.

## Solution

The solution relies on generating IL code to create the necessary code according to the differences in various Visual Studio versions.
This way, the required DLLs only need to be searched for and loaded when the extension is loaded,
eliminating the need for manual updates and making the extension compatible with older updates as well.

It is crucial that the abstraction is as simple as possible to minimize the complexity of the IL code to be generated.

## Projects

| Project | Description |
|---|---|
| ExtensionManager.VisualStudio.Adapter.Abstractions | Contains the abstractions for IL generation. |
| ExtensionManager.VisualStudio.Adapter.Generator | Contains classes for generating the specific code. |
