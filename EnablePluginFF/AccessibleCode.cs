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
