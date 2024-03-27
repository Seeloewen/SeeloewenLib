/////////////////////////////////////////////////////////////////////
//                                                                 //
// SealLib v1.1.0                                                  //
// Wednesday, 27th March 2024                                      //
// Created by Seeloewen                                            //
//                                                                 //
// Simple library that contains some code that is used by my apps. //
// You are free to use this library in your apps if you wish.      //
// Make sure to acknowledge the license at the bottom of the file. //
//                                                                 //
// Find the library on GitHub:                                     //
// https://github.com/Seeloewen/SeeloewenLib                       //
//                                                                 //
/////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using System.Windows.Media.Animation;
using System.Windows.Forms.VisualStyles;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.IO;
using System.Linq;

namespace SeeloewenLib
{

    public class SeeloewenLibTools
    {
        public string ConvertListToString(List<string> list)
        {
            //Set the default output string
            string outputString = "";

            //Add each item in the list as a new line in the string
            foreach (string item in list)
            {
                outputString = string.Format("{0}{1}\n", outputString, item);
            }

            //Output the string
            return outputString;
        }

        public T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            //Takes a child object and returns its visual parent
            child = VisualTreeHelper.GetParent(child);
            return child as T;
        }

        public T FindVisualChild<T>(DependencyObject parent, int index) where T : DependencyObject
        {
            //Get the child corresponding to the index and parent
            var child = VisualTreeHelper.GetChild(parent, index);
            if (child is T typedChild)
            {
                //return the child
                return typedChild;
            }
            return null;
        }

        private string ConvertNumberUnit(double number)
        {
            //Define unit
            string unit = "";

            if (number > 1000)
            {
                //If number is one thousand or more
                unit = "k";
                number = number / 1000;
                if (number > 1000)
                {
                    //If number is one million or more
                    unit = "m";
                    number = number / 1000;
                    if (number > 1000)
                    {
                        //If number is one billion or more
                        unit = "b";
                        number = number / 1000;
                        if (number > 1000)
                        {
                            //If number is one trillion ore more
                            unit = "t";
                            number = number / 1000;
                        }
                    }
                }
            }


            //Return the combination of number and unit
            return string.Format("{0}{1}", number, unit);
        }
    }

    public class SaveSystem
    {
        public string path;
        public string saveFileHeader;
        public string saveFileName;
        public List<SaveEntry> saveEntries = new List<SaveEntry>();

        public SaveSystem(string path, string saveFileHeader, string saveFileName)
        {
            //Set the path where the settings file will be saved
            this.path = path;
            this.saveFileHeader = saveFileHeader;
            this.saveFileName = saveFileName;
        }

        public void Save()
        {
            List<string> file = new List<string>();

            //Save file header
            file.Add(saveFileHeader);

            //Save the settings to the file
            foreach (SaveEntry saveEntry in saveEntries)
            {
                if (saveEntry.isCategory == true)
                {
                    file.Add(string.Format("\n#{0}", saveEntry.name));
                }
                else
                {
                    file.Add(string.Format("{0}={1}", saveEntry.name, saveEntry.content));
                }
            }
            File.WriteAllLines(string.Format("{0}/{1}", path, $"{saveFileName}.txt"), file);
        }

        public void Load()
        {
            //Read the settings from the file
            IEnumerable<string> output = File.ReadLines($"{path}/{saveFileName}.txt");
            IEnumerable<string> oldFile;
            bool oldFileSaveNeeded = false;

            foreach (string entry in output)
            {
                string[] entrySplit = entry.Split('=');

                //Check if the entry is not empty and not the header
                if (entry != saveFileHeader && !string.IsNullOrEmpty(entry))
                {
                    foreach (SaveEntry saveEntry in saveEntries)
                    {
                        //Match the loaded entry to an existing entry
                        if (saveEntry.name == entrySplit[0] && !saveEntry.isCategory)
                        {
                            //Check for corruption. If there is none, load the entry from the file
                            if(!IsCorrupted(saveEntry, entrySplit[1]))
                            {
                                saveEntry.content = entrySplit[1];
                            }
                            else
                            {
                                //If there is corruption, ask the user if they want to correct it
                                MessageBoxResult dialogResult = MessageBox.Show($"The save entry you are trying to load is corrupted and will most likely not work ({saveEntry.name} with content {entrySplit[1]}). Do you want to try to correct it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (dialogResult == MessageBoxResult.Yes)
                                {
                                    oldFile = output;
                                    oldFileSaveNeeded = true;
                                    saveEntry.content = saveEntry.possibleValues[0];
                                    MessageBox.Show($"The save entry {saveEntry.name} was corrected to {entrySplit[0]}.", "Corrected", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else if (dialogResult == MessageBoxResult.No)
                                {
                                    saveEntry.content = entrySplit[1];
                                }
                            }
                        }
                    }
                }
            }

            //Save the old file and new file if a conversion because of corruption happended
            if(oldFileSaveNeeded == true)
            {
                File.WriteAllLines(string.Format("{0}/{1}", path, $"{saveFileName}-{DateTime.Now.ToFileTime()}.old"), output);
                Save();
            }
        }

        public bool IsCorrupted(SaveEntry EntryTemplate, string entry)
        {
            //Check if the entry even needs to be checked or can be anything
            if (EntryTemplate.hasDefinedValues == true)
            {
                //Check if the entry is one of the possible values
                foreach (string possibleValue in EntryTemplate.possibleValues)
                {
                    if (possibleValue == entry)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetEntry(string name, string content)
        {
            //Sets the given entry to the given content
            foreach (SaveEntry entry in saveEntries)
            {
                if (entry.name == name)
                {
                    if (entry.isCategory == false)
                    {
                        entry.content = content;
                    }
                    else
                    {
                        MessageBox.Show("SeeloewenLib Error: You're trying to set the content of a category, which is not possible.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public string GetEntry(string name)
        {
            //Gets the content of the given entry
            foreach (SaveEntry entry in saveEntries)
            {
                if (entry.name == name)
                {
                    if (entry.isCategory == false)
                    {
                        return entry.content;
                    }
                    else
                    {
                        MessageBox.Show("SeeloewenLib Error: You're trying to get the content of a category, which is not possible.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return "";
                    }
                }
            }

            return "";
        }
    }

    public class SaveEntry
    {
        public string name;
        public string content;
        public bool isCategory;
        public bool hasDefinedValues; //If this is false, a corruption check will be skipped
        public string[] possibleValues;
        private int index;

        public SaveEntry(string name, string defaultContent, bool isCategory, bool hasDefinedValues, string[] possibleValues, int index)
        {
            //Map all variables
            this.name = name;
            this.content = defaultContent;
            this.isCategory = isCategory;
            this.hasDefinedValues = hasDefinedValues;
            this.possibleValues = possibleValues;
            this.index = index;
        }
    }

    public class Wizard
    {
        //Attributes
        public List<WizardPage> pages = new List<WizardPage>();
        public GroupBox gbWizard;
        public Button btnContinue;
        public Button btnBack;
        public int currentPage = 1;
        public int pagesAmount = 0;
        public Action codeCancel = null;
        public Action codeFinish = null;

        //-- Constructor --//

        public Wizard(int pagesAmount, int height, int width, Button btnContinue, Button btnBack, Action codeCancel, Action codeFinish, Thickness margin)
        {
            //Create references
            this.btnContinue = btnContinue;
            this.btnBack = btnBack;
            this.pagesAmount = pagesAmount;
            this.codeCancel = codeCancel;
            this.codeFinish = codeFinish;

            //Create Wizard GroupBox
            gbWizard = new GroupBox();
            gbWizard.Width = width;
            gbWizard.Height = height;
            gbWizard.Margin = margin;

            //Setup buttons
            this.btnContinue.Click += new RoutedEventHandler(btnContinue_Click);
            this.btnBack.Click += new RoutedEventHandler(btnBack_Click);

            //Create pages based on amount
            for (int i = 0; i < pagesAmount; i++)
            {
                pages.Add(new WizardPage(i + 1, string.Format("Step {0}", i + 1), defaultRequirement, true, true, "Cannot continue to the next page because the requirements are not fulfilled."));
            }

            //Show first page
            ShowPage(1);
        }

        //-- Custom Methods --//

        public void ShowPage(int pageNum)
        {
            if (pages[pageNum - 1].requirements() == true)
            {
                //Set button text based on page number
                if (pageNum > 1 && pageNum < pagesAmount)
                {
                    btnContinue.Content = "Continue";
                    btnBack.Content = "Back";
                }
                else if (pageNum <= 1)
                {
                    btnContinue.Content = "Continue";
                    btnBack.Content = "Cancel";
                }
                else if (pageNum >= pagesAmount)
                {
                    btnContinue.Content = "Finish";
                    btnBack.Content = "Back";
                }

                //Set button state based on page settings
                if (pages[pageNum - 1].canGoBack == true)
                {
                    btnBack.IsEnabled = true;
                }
                else
                {
                    btnBack.IsEnabled = false;
                }
                if (pages[pageNum - 1].canContinue == true)
                {
                    btnContinue.IsEnabled = true;
                }
                else
                {
                    btnContinue.IsEnabled = false;
                }

                //Only show page if it's in range
                if (pageNum > 0 && pageNum <= pagesAmount)
                {
                    //Show page based on the page number and execute code
                    currentPage = pageNum;
                    gbWizard.Content = pages[pageNum - 1].grdContent;
                    gbWizard.Header = pages[pageNum - 1].header;
                    pages[pageNum - 1].ExecuteCode();
                }
            }
            else
            {
                //Show error message
                MessageBox.Show(pages[pageNum - 1].requirementsNotFulfilledMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ShowNextPage()
        {
            if (currentPage < pagesAmount)
            {
                //Shows the next page in the wizard
                ShowPage(currentPage + 1);
            }
            else
            {
                //Invoke code, that gets run when users tries to go to the next page, even though it was the last page      
                codeFinish?.Invoke();
            }
        }

        public void ShowPreviousPage()
        {
            if (currentPage > 1)
            {
                //Show the previous page in the wizard
                ShowPage(currentPage - 1);
            }
            else
            {
                //Invoke code, that gets run when user tries to go to the previous page even though it was the first page
                codeCancel?.Invoke();
            }
        }

        public bool defaultRequirement()
        {
            //Default requirement used by the pages, always returns true
            return true;
        }

        //-- Event Handlers --//
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Show the next page
            ShowNextPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Show the previous page
            ShowPreviousPage();
        }
    }

    public class WizardPage
    {
        //Attributes
        public Grid grdContent;
        public Action code = null;
        public string header;
        public int pageNum;
        public Func<bool> requirements;
        public bool canGoBack;
        public bool canContinue;
        public string requirementsNotFulfilledMsg;

        //-- Constructor --//

        public WizardPage(int pageNum, string header, Func<bool> requirements, bool canGoBack, bool canContinue, string requirementsNotFulfilledMsg)
        {
            //Create references
            this.pageNum = pageNum;
            this.header = header;
            this.requirements = requirements;
            this.canGoBack = canGoBack;
            this.canContinue = canContinue;
            this.requirementsNotFulfilledMsg = requirementsNotFulfilledMsg;

            //Create the grid that contains the page content. You can add whatever you want to that grid, it's up to you what the page contains
            grdContent = new Grid();
        }

        //-- Custom Methods --//

        public void ExecuteCode()
        {
            //Run the code that is assigned to this page
            code?.Invoke();
        }
    }
}

////////////////////////////////////////////////////////////////////////////
//                                                                        //
// SeeloewenLib - A simple yet powerful C# library                        //
// Copyright(C) 2024 Louis/Seeloewen                                      //
//                                                                        //
// This program is free software: you can redistribute it and/or modify   //
// it under the terms of the GNU General Public License as published by   //
// the Free Software Foundation, either version 3 of the License, or      //
// (at your option) any later version.                                    //
//                                                                        //
// This program is distributed in the hope that it will be useful,        //
// but WITHOUT ANY WARRANTY; without even the implied warranty of         //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          //
// GNU General Public License for more details.                           //
//                                                                        //
// You should have received a copy of the GNU General Public License      //
// along with this program.  If not, see <https://www.gnu.org/licenses/>. //
//                                                                        //
////////////////////////////////////////////////////////////////////////////