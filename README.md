# com.bovinelabs.analyzers

A package to easily add Roslyn Analyzers to Unity projects.

### Install
Add the package to your manifest.json file located under the Packages folder. 

```
  "dependencies": {
    "com.bovinelabs.analyzers": "https://github.com/tertle/com.bovinelabs.analyzers.git",
```

Alternatively download into the Packages/com.bovinelabs.analyzers folder. Please note importing to a directory outside of the Packages folder is not supported.

### Using
A simple UI interface is available at Windows/BovineLabs/Analyzers. This allows you change the target directory for the analyzers.

An option to import StyleCop (with a few custom rules for Unity) is available for use or as a test. If working, you should see your analyzers in your project. For example, References/Analyzers/StyleCop.Analyzers

### Requirements
* Unity 2018.3.0f2 onwards.
* Visual Studios (Rider support will come at some point)

Unfortunately as far as I can tell, other code editors such as Visual Studio Code do not support Roslyn Analyzers at this point. If I am mistaken please point me in the right direction and I'll see if there is anyway to support it.
