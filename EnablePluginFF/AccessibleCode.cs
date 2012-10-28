using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accessibility;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;

namespace EnablePluginFF
{
    public class AccessibleCode
    {
        IntPtr handle;
        IAccessible iAccessible;//interface: Accessibility namespace
        private const int CHILDID_SELF = 0;
        private const int SELFLAG_TAKESELECTION = 0x2;
        private bool add = true;
        //Obj ID
        internal enum OBJID : uint
        {
            WINDOW = 0x00000000,
            SYSMENU = 0xFFFFFFFF,
            TITLEBAR = 0xFFFFFFFE,
            MENU = 0xFFFFFFFD,
            CLIENT = 0xFFFFFFFC,
            VSCROLL = 0xFFFFFFFB,
            HSCROLL = 0xFFFFFFFA,
            SIZEGRIP = 0xFFFFFFF9,
            CARET = 0xFFFFFFF8,
            CURSOR = 0xFFFFFFF7,
            ALERT = 0xFFFFFFF6,
            SOUND = 0xFFFFFFF5,
            CHILDID_SELF = 0,
            SELFLAG_TAKEFOCUS = 0x01
        }

       
        private string name()
        {
            return iAccessible.get_accName(0);
        }

        private void getChild(IAccessible iAccessible,bool done)
        {
            if (iAccessible == null)
            {
                Console.WriteLine(" The iAccessible object is null");
                return;
            }
            string accWindowName = iAccessible.get_accName(0);
            string accWindowVal = iAccessible.get_accValue(0);

            IAccessible[] childs = new IAccessible[iAccessible.accChildCount];

            int obtained = 0;

            AccessibleChildren(iAccessible, 0, iAccessible.accChildCount - 1, childs, out obtained);
            int i = 0;
            foreach (IAccessible child in childs)
            {
                if (child != null && child.GetType().IsCOMObject)
                {
                    Console.WriteLine("The value of i : " + i);
                    i++;
                    if (child == null)
                    {
                        Console.WriteLine("Child is NULL");
                        continue;
                    }
                    string cname = child.get_accName(0);
                    string cvalue = child.get_accValue(0);
                    string cdesc = child.get_accDescription(0);
                    int crole = child.get_accRole(0);
                    

                    if (cname != null && cname.Trim() != "")
                        Console.WriteLine("Name is : " + cname);
                    else
                        Console.WriteLine("Name is : null");
                    if (cvalue != null && cvalue.Trim() != "")
                        Console.WriteLine("Value is : " + cvalue);
                    else
                        Console.WriteLine("Value is : null");

                    if (cdesc != null && cdesc.Trim() != "")
                        Console.WriteLine("Description is : " + cdesc);
                    else
                        Console.WriteLine("Description is : null");
                    if (crole != null)
                        Console.WriteLine("Role is : " + crole);
                    else
                        Console.WriteLine("Role is : null");
                    if (cname!=null && cname.Contains("Firebug"))
                    {
                        Console.WriteLine("Firebug");

                    }
                    getChild(child,true);
                }
                
            }
        }

        public bool doAccessibleHandle()
        {
            System.IO.StreamWriter file = null;
            bool found = false;
            string pathforfile = "C:\\Users\\Pragya\\Documents\\Advanced Project - 524";
            List<AutomationElement> names = new List<AutomationElement>();
            bool ourext = false;
            const string ext = "XTalk 2.36 (disabled) An Extension for HearSay";
            try
            {
                AutomationElement aeBrowser = AutomationElement.FromHandle(handle);
                Console.WriteLine("Browser Name : " + aeBrowser.Current.Name);
                /* string getURL = this.GetURLfromFirefox(aeBrowser);
                 Console.WriteLine("The URL we got" + getURL);*/
              
                WalkEnabledElements(aeBrowser, names);
                file = new System.IO.StreamWriter(pathforfile + "\\test.txt");
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
                    if (ae.Current.Name.Contains(ext))
                    {
                        ourext = true;
                    }
                    file.WriteLine("Bool Variable : " + ourext);
                    if (ae.Current.ControlType.ProgrammaticName == "ControlType.Button" && ourext == true && (ae.Current.Name == "Enable" || ae.Current.Name == "Disable"))
                    {
                        found = true;
                        if (ae.Current.Name == "Enable")
                        {
                            if (InvokeControl(ae) == true)
                                file.WriteLine(" Enabling the extension");

                        }
                        else
                        {
                            file.WriteLine("Already Enabled ");
                        }
                        ourext = false;
                        //return true;
                    }
                    
                }
                if (found == false)
                {
                    file.WriteLine("Could not find the extension");
                   // return false;
                }
            }
            catch (ElementNotAvailableException enax)
            {
                Console.WriteLine("Element not Available exception : " + enax.ToString());
            }
            finally
            {
                file.Close();
            }
            return false;
        }

        ///-------------------------------------------------------------------- 
        /// <summary> 
        /// Obtains an InvokePattern control pattern from a control 
        /// and calls the InvokePattern.Invoke() method on the control. 
        /// </summary> 
        /// <param name="targetControl">
        /// The control of interest. 
        /// </param> 
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
        /// <param name="treeNode">The node in the TreeView for this iteration.</param>
        /// <remarks> 
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
        public bool checkHandle()
        {
            
           
            Guid guid = new Guid("{618736E0-3C3D-11CF-810C-00AA00389B71}");
            object obj = null;
            int retVal = AccessibleObjectFromWindow(handle, (uint)OBJID.WINDOW, ref guid, ref obj);
            iAccessible = (IAccessible)obj;
            
            //The AccWindowName returned is Add-ons Manager - Mozilla Firefox
            //There is a special child id called CHILDID_SELF (this constant equals 0) that, when used with a function like get_accChild, returns the element itself rather than a child.

            string accWindowName = iAccessible.get_accName(0);
            string accWindowVal = iAccessible.get_accValue(0);
            
            Console.WriteLine("IAccessible Name : " + accWindowName);
            Console.WriteLine("IAccessible value : " + accWindowVal);
            Console.WriteLine("IAccessible Role is : " + iAccessible.get_accRole(0));

            Console.WriteLine("IAccessible Type: " + iAccessible.GetType());
            Console.WriteLine("IAccessible Focus is: " + iAccessible.accFocus);
            Console.WriteLine("IAccessible Selection is " + iAccessible.get_accState());
            //iAccessible.accSelect((int)OBJID.SELFLAG_TAKEFOCUS, 0);
            if (!accWindowName.Contains("Mozilla Firefox"))
                return false;

            getChild(iAccessible,false);

            //End of for window
            Console.WriteLine("End of checkHandle");
            iAccessible = null;
            return false;
        }

        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(
             IntPtr hwnd,
             uint id,
             ref Guid iid,
             [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        [DllImport("oleacc.dll")]
        public static extern uint AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);


        public void getNavigator()
        {
        }

        public AccessibleCode(IntPtr p_handle)
        {
            handle = p_handle;
        }

    }
}
