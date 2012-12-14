using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accessibility;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

//NOTE : Have removed all the IAccessible Code for now. Will put it in the other branch
namespace EnablePluginFF
{
    public class AccessibleCode
    {
        //The handle of firefox : this is set by the callee function
        IntPtr handle;

        public bool add { get; set; }

        public int doAccessibleHandle(string extension, string logfile)
        {
            //Stream writer to output to a log file
            System.IO.StreamWriter file = null;
            bool found = false;

            //This varible stores the path to the log file - Please change it, Now this is not used but the logfile is taken from command line
            //string pathforfile = "C:\\Users\\Pragya\\Documents\\Advanced Project - 524";

            //This is a list of all the automation elements 
            List<AutomationElement> names = new List<AutomationElement>();
            bool ourext = false;
            
            try
            {
                //Gets the automation element of the browser
                AutomationElement aeBrowser = AutomationElement.FromHandle(handle);
                Console.WriteLine("Browser Name : " + aeBrowser.Current.Name);
                /* string getURL = this.GetURLfromFirefox(aeBrowser);
                 Console.WriteLine("The URL we got" + getURL);*/
              
                //This traverses all the automation elements
                WalkEnabledElements(aeBrowser, names);


                file = new System.IO.StreamWriter(logfile);
                foreach (AutomationElement ae in names)
                {
                    file.WriteLine();
                    file.WriteLine("Name : " + ae.Current.Name);
                    file.WriteLine("Type : " + ae.Current.AutomationId);
                    file.WriteLine("Class Name : " + ae.Current.ClassName);
                    file.WriteLine("Control Type Name : " + ae.Current.ControlType.ProgrammaticName);
                    file.WriteLine("Is Enabled : " + ae.Current.IsEnabled);
                    file.WriteLine("Native Window Handle : " + ae.Current.NativeWindowHandle);
                    file.WriteLine("Automation ID : " + ae.Current.AutomationId);
                    file.WriteLine("Item Type : " + ae.Current.ItemType);

                    // if the current automation element Name contains our extension Name, set the variable ourext to true;
                    if (ae.Current.Name.Contains(extension))
                    {
                        ourext = true;
                    }
                    file.WriteLine("Bool Variable : " + ourext);

                    //If the current Automation element type is a Button and we are traversing our extension and the button name is either Enable or Disable, we try to either 
                    //enable it if it is currently disabled or do nothing if it enabled
                    if (ae.Current.ControlType.ProgrammaticName == "ControlType.Button" && ourext == true && (ae.Current.Name == "Enable" || ae.Current.Name == "Disable"))
                    {
                        found = true;
                        //if the button name is enabled, it means that the button is right now disabled and we have to click on this button and enable it
                        if (ae.Current.Name == "Enable")
                        {
                            //We call the InvokeControl(ae) function to click the button
                            if (InvokeControl(ae) == true)
                            {
                                Console.WriteLine("Extension " + extension + "Enabled the extension");
                                file.WriteLine("Extension " + extension + " Enabled the extension");
                                return 3;
                            }

                        }
                        else
                        {
                            // if the button name is disabled, that means it is already enabled, so we do nothing
                            Console.WriteLine("Extension " + extension + " is Already Enabled ");
                            file.WriteLine("Extension " + extension + "Already Enabled ");
                            return 4;
                        }
                        //So we have not enabled or disabled out extension, so we are done and we won't do anything now
                        ourext = false;
                    }
                    
                }

                //If we could not find out extension, after traversing the entire list of Automation elements, we just write a message in the  LOG
                if (found == false)
                {
                    file.WriteLine("Could not find the extension : " + extension);
                    return 2;
                }
            }
            catch (ElementNotAvailableException enax)
            {
                Console.WriteLine("Element not Available exception : " + enax.ToString());
                return 1;
            }
            finally
            {
                file.Close();
            }
            return 1;
        }

        ///-------------------------------------------------------------------- 
        /// <summary> 
        /// Obtains an InvokePattern control pattern from a control 
        /// and calls the InvokePattern.Invoke() method on the control. 
        /// </summary> 
        /// <param name="targetControl">
        /// The control of interest. In our case this would be the button, we would click to
        /// enable the extension
        /// </param> 
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/system.windows.automation.invokepattern.invoke(v=VS.85).aspx
        /// </remarks>
        ///-------------------------------------------------------------------- 
        private bool InvokeControl(AutomationElement targetControl)
        {
            Console.WriteLine("In Invoke Control");
            InvokePattern invokePattern = null;

            try
            {
                invokePattern =
                    targetControl.GetCurrentPattern(InvokePattern.Pattern)
                    as InvokePattern;
            }
            catch (ElementNotEnabledException)
            {
                // Object is not enabled 
                return false;
            }
            catch (InvalidOperationException)
            {
                // object doesn't support the InvokePattern control pattern 
                return false;
            }

            invokePattern.Invoke();
            return true;
        }
        /// <summary> 
        /// Walks the UI Automation tree and adds the control type of each enabled control  
        /// element it finds to a TreeView. 
        /// </summary> 
        /// <param name="rootElement">The root of the search on this iteration.</param>
        /// <param name="List of Automation elements">This list stores all the automation elements.</param>
        /// <remarks> 
        /// http://msdn.microsoft.com/en-us/library/ms752090.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-1
        /// This is a recursive function that maps out the structure of the subtree beginning at the 
        /// UI Automation element passed in as rootElement on the first call. This could be, for example, 
        /// an application window. 
        /// CAUTION: Do not pass in AutomationElement.RootElement. Attempting to map out the entire subtree of 
        /// the desktop could take a very long time and even lead to a stack overflow. 
        /// </remarks> 
        private void WalkEnabledElements(AutomationElement rootElement, List<AutomationElement> elementName)
        {
            
            Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
            Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
            TreeWalker walker = new TreeWalker(new AndCondition(condition1, condition2));
            AutomationElement elementNode = walker.GetFirstChild(rootElement);
            while (elementNode != null)
            {
                
                if (add == true)
                {

                    elementName.Add(elementNode);
                }
                if (elementNode.Current.Name.Contains("XTalk "))
                    add = true;
                WalkEnabledElements(elementNode, elementName);
                if (elementNode.Current.Name.Contains("XTalk "))
                    add = true;
                elementNode = walker.GetNextSibling(elementNode);
                
            }
        }


        public string GetURLfromFirefox(AutomationElement rootElement)
        {
            Condition condition1 = new PropertyCondition(AutomationElement.IsContentElementProperty, true);
            TreeWalker walker = new TreeWalker(condition1);
            AutomationElement elementNode = walker.GetFirstChild(rootElement);


            if (elementNode != null)
            {
                Console.WriteLine(elementNode.Current.Name);
            }
            return "null";
        }

        //Constructor to Set the Handle
        public AccessibleCode(IntPtr p_handle)
        {
            handle = p_handle;
        }

    }
}
