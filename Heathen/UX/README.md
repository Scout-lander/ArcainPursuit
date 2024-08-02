# UX Complete

For more information on what this asset is capable of or how to use it please visit our **[Knowledge Base](https://kb.heathenengineering.com/assets/ux)** or if you have questions please visit our [Discord](https://discord.gg/6X3xrRc).

* Author: [Heathen Group](https://www.heathen.group)
* License: [Heathen License Agreement](https://github.com/heathen-engineering/SourceRepo/blob/main/LICENSE.md)
* [Documentation](https://kb.heathenengineering.com/assets/ux)
* [Support](https://discord.gg/6X3xrRc)
* [Reporting Issues](https://github.com/heathen-engineering/SourceRepo/issues)
  
## Installation
We strongly recommend you clone the repository to your local disk. The simplest way to do this is to use [GitHub Desktop](https://desktop.github.com/), once you have that installed you can easily clone any repository to your local disk.
![image](https://user-images.githubusercontent.com/31243736/215474459-458b8908-ea90-4163-812d-cf135e02d840.png)

Once you have the repository cloned to your local disk you can use Unity's Package Manager to import any of the packages you like into your project.

1. Open the Unity Package Manager
2. Click the + button
3. Select the "Add from Disk" option

![image](https://user-images.githubusercontent.com/31243736/215474968-4bbe88b6-f2ec-448e-a7cc-d6667b95b4b3.png)

4. Select the package.json for the asset you would like to import. for example ./SourceRepo/Unity/Steamworks/com.heathen.ux/package.json

## Description
UX is a collection of system and tools to help you deliver a better user experience and includes features like

### [Command System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/command-system)
Parse text commands and raise game events in responce. This system simplifies the creation of emote system, in game command systems and simple in game scripting tools.

### [Cursor System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/cursor-tools)
Codes free solution for creating context sinsative animated mouse pointers and cursors using hardware or software pointers.

### [Drag and Drop System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/drag-and-drop-system)
Code free solution for handling drag and drop UI features and includes the ability to filter or assign rules to drop slots makign it possible to create inventory systems, spell bars and more with zero code

### [Interaction Tools](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/interaction-tools)
Quick efficent and easy to use interaction event tools, simply add the desired componenet and handle click and hold, play on click, play on enter and many more user interaction events not exposed by stock Unity features.

### [Logging & Feedback](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/feedback-tools)
User eXperience kit helps you gather rich game data in a well formatted log, and then feed that data from within your game tools such as Trello, Zendesk, Unity's User Reporting, etc. and to help you capture and save screenshots for the full screen or sections of the screen.

### [Scene Tools](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/scenes-management)
The Scenes object helps you manage multi-scene, additive scene game architecture patterns. Multi-scene and additive scene paradigms can significantly improve performance and give you the developer a lot more flexibility in terms of design and architecture decisions; that is it allows you to group your scenes with respect to your games use as opposed to a single scene system where each scene exists on its own in isolation, and as a result must be a large monolithic structure which is slow to load and cumbersome to manage.

### [Selection System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/selection-system)
UX provides an easy to use, flexable, searchable multi-select system. Read more about the API.Selection interface in its API article.

### [Tooltip System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/tooltips)
The UX Tooltip tools are a simple and flexible approach to driving tooltips in your game. In the simplest form the tool can be thought of as a simple trigger that responds to mouse over event and provides basic feedback to use as a developer, e.g. an event you can listen on an the ability to control delays and similar.

By combining this simple concept with existing tools you can create any sort of tooltip your game may require. From simple popup messages (classic tooltips) to interactive tips with cascading tips contained with in.

### [Window System](https://kb.heathenengineering.com/assets/ux/learning/core-concepts/window-tools)
The Window system makes it easy to define drag-able, re-sizable and dock-able windows in your game. The simple system can be attached to any Rect Transform and supports all canvas modes and all anchoring types. Window groups can be defined easily by simply parenting them in there own Rect Transforms where windows focus (move to top) and clamp (remain in frame) according to there parent.


