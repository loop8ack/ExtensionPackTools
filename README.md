# Import/Export Visual Studio extensions

[![Build status](https://ci.appveyor.com/api/projects/status/hc78u7wnqya38mur?svg=true)](https://ci.appveyor.com/project/madskristensen/extensionpacktools)

[Download extension](http://vsixgallery.com/extension/e83d71b8-8bfc-4e06-b145-b0388910c016/)

This extension allows you to export a list of extensions and importing them back into any instance of VS 2017.

## Export

![Tools menu](art/menu_tools.png)

The output is a JSON file with an .vsext file extension looking like this:

```json
{
  "id": "a5952e33-576b-44cf-af5b-466cb6d0ca50",
  "name": "My Visual Studio extensions",
  "description": "A collection of my Visual Studio extensions",
  "version": "1.0",
  "extensions": [
    {
      "name": "Extensibility Tools",
      "vsixId": "f8330d54-0469-43a7-8fc0-7f19febeb897"
    },
    {
      "name": "Web Essentials 2017",
      "vsixId": "bb7e2273-9a70-4e5e-b4dd-1f361b6166c0"
    },
    {
      "name": "EditorConfig Language Service",
      "vsixId": "1209461d-57f8-46a4-814a-dbe5fecef941"
    }
  ]
}
```

## Import
Clicking the import button prompts you to select a .vsext file. Doing that will present you with this dialog that lists all the extensions found in the .vsext file.

![Import](art/import.png)

Before showing the list it will verify that the extensions exist on the Marketplace and that can take a few seconds.

Clicking the **Select** button in the dialog will start the VSIX Installer in a separate process and you can follow the normal install flow from there.

## License
[Apache 2.0](LICENSE)