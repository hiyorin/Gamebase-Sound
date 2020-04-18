# Gamebase-Sound

<br><br><br><br>
## Table Of Contents
- [Description](#description)
- [Install](#install)
- [Usege](#usage)
- [NodeCanvas Integration](#nodecanvas-integration)
- [CRI ADX2 Integration](#cri-adx2-integration)
- [Master Audio Integration](#master-audio-integration)
- [License](#license)

<br><br><br><br>
## Description

<br><br><br><br>
## Install
Find `Packages/manifest.json` in your project and edit it to look like this:
```json
{
  "scopedRegistries": [
  {
    "name": "OpenUPM",
    "url": "https://package.openupm.com",
    "scopes": [
      "com.neuecc",
      "com.cysharp",
      "com.svermeulen",
      "com.coffee",
      "com.demigiant",
      ...
    ]
  }],
  "dependencies": {
    "com.coffee.git-dependency-resolver": "1.1.3",
    "com.coffee.upm-git-extension": "1.1.0-preview.12",
    "com.gamebase.sound": "https://github.com/hiyorin/Gamebase-Sound.git",
    ...
  }
}
```
To update the package, change `#{version}` to the target version.  
Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension.git) to install or update the package.

<br><br><br><br>
## Usage

<br><br><br><br>
## NodeCanvas Integration
[NodeCanvas](https://assetstore.unity.com/packages/tools/visual-scripting/nodecanvas-14914) is the Complete Visual Behaviour Authoring solution for Unity. For more details see the [NodeCanvas docs](https://nodecanvas.paradoxnotion.com/documentation/).  

Gamebase integration with NodeCanvas is disabled by default. To enable, you must add the define `GAMEBASE_ADD_NODECANVAS` to your project, which you can do by selecting Edit -> Project Settings -> Player and then adding `GAMEBASE_ADD_NODECANVAS` in the `Scripting Define Symbols` section.

<br><br><br><br>
## CRI ADX2 Integration
[CRI ADX2](https://game.criware.jp/products/adx2/) is the best audio middleware for cross-platform development. For more details see the [CRI ADX2 Manual](https://game.criware.jp/manual/unity_plugin/jpn/contents/index.html).  

Gamebase integration with CRI ADX2 is disabled by default. To enable, you must add the define `GAMEBASE_ADD_ADX2` to your project, which you can do by selecting Edit -> Project Settings -> Player and then adding `GAMEBASE_ADD_ADX2` in the `Scripting Define Symbols` section.  

<br><br><br><br>
## Master Audio Integration
[Master Audio](https://assetstore.unity.com/packages/tools/audio/master-audio-aaa-sound-5607) gives you tremendous ease of use, speed, power and flexibility far beyond any contender. For more details see the [Master Audio docs](https://www.dtdevtools.com/docs/masteraudio/TOC.htm).  

Gamebase integration with MasterAudio is disabled by default. To enable, you must add the define `GAMEBASE_ADD_MASTERAUDIO` to your project, which you can do by selecting Edit -> Project Settings -> Player and then adding `GAMEBASE_ADD_MASTERAUDIO` in the `Scripting Define Symbols` section.

<br><br><br><br>
## License
This library is under the MIT License.  
[here](LICENSE.md)
