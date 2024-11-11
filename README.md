# SeeloewenLib
SeeloewenLib is a simple C# library with useful code mainly used in software by Seeloewen.
You're free to use this library in your own code if it's useful for you. Make sure to acknowledge the license the library lies under, which can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html.
Please note that most of this code only works in WPF.

## Content
Down below is a quick overview of all the main components that this library has to offer.
You can find more detailed explanations in the library's code itself.


### Tools
The Tools class contains a bunch of useful methods like ConvertListToString, FindVisualParent, FindVisualChild, ConvertNumberUnit and much more, which all do what the name says.

### SaveSystem
The SaveSystem class together with the SaveEntry class provide a solid base for a working save system, where you can save strings in a file to your disk. You are able to create a save system with save entries that have the following parameters, allowing for customization: *string name, string content, bool isCategory, bool hasDefinedValues, string[] possibleValues, int index*. When starting your app, you will need to create a saveEntry for each of your settings. During runtime, you can then use Get and Set methods to access the saveEntries. A simple Save() and Load() method can save and load the saveEntries to your drive. When loading/saving a save entry, it will also get checked for corruption and can be corrected.

### Wizard
The Wizard class can be used to create simple wizards in your software. It creates a groupbox and a specified amount of pages, which contains Grids that are added to the groupbox. There are predefined methods for changing the page and you can customize it by changing the content, header, page requirements and even the code that gets executed when a page is shown.

Below is an example for a wizard taken out of the Random Item Giver Updater, used in a Duplicate Finder window.

            //Create the wizard
            wzdDuplicateFinder = new Wizard(2, 580, 742, btnContinue, btnBack, Close, Close, new Thickness(0, 0, 0, 0)); //Creates the wizard with 2 pages at coordinates x580 and y742 as well as mapping the buttons and setting the options on first and final page
            grdDuplicateFinder.Children.Add(wzdDuplicateFinder.gbWizard); //Adds the wizard to the window grid
            wzdDuplicateFinder.gbWizard.Foreground = new SolidColorBrush(Colors.White); //Style the wizard
            wzdDuplicateFinder.gbWizard.FontSize = 16; //Style the wizard

            //Setup the pages
            wzdDuplicateFinder.pages[0].grdContent.Children.Add(cvsStep1); //Add the content to the first page
            wzdDuplicateFinder.pages[1].grdContent.Children.Add(cvsStep2); //Add the content to the second page
            wzdDuplicateFinder.pages[1].code = CheckForDuplicates; //Set which code gets executed when going to page 2
            wzdDuplicateFinder.pages[1].requirements = pageTwoRequirements; //Set the requirements for the second page
            wzdDuplicateFinder.pages[1].requirementsNotFulfilledMsg = "Cannot search for duplicates. Please make sure that a datapack or loot table is loaded."; //Set the message that shows when requirements for page 2 are not met
            wzdDuplicateFinder.pages[1].canGoBack = false; //Set if you can go back on page 2
            cvsStep1.Margin = new Thickness(10, 10, 0, 0); //Style the first page
            cvsStep2.Margin = new Thickness(10, 10, 0, 0); //Style the second page
            cvsStep1.Visibility = Visibility.Visible; //Set visibility of the first page
            cvsStep2.Visibility = Visibility.Visible; //Set visibility of the second page

