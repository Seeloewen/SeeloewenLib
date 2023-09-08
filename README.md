# SeeloewenLib
SeeloewenLib is a simple C# library with useful code mainly used in software by Seeloewen.
You're free to use this library in your own code if it's useful for you. Make sure to acknowledge the license the library lies under, which can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

## Content
Down below is a quick overview of all the main components that this library has to offer.
You can find more detailed explanations in the library's code itself.


### SeeloewenLibTools
The SeeloewenLibTools class contains a bunch of useful methods like ConvertListToString, FindVisualParent, FindVisualChild, ConvertNumberUnit and much more, which all do what the name says.

### SaveSystem
The SaveSystem class contains a simple system for saving and loading content of your software to/from the harddisk. It is currently in alpha shall only used for test purposes. It is capable of saving a list of strings and also returning one when loading. This will get an overhaul in the future.

### Wizard
The Wizard class can be used to create simple wizards in your software. It creates a groupbox and a specified amount of pages, which contains Grids that are added to the groupbox. There are predefined methods for changing the page and you can customize it by changing the content, header, page requirements and even the code that gets executed when a page is shown.

