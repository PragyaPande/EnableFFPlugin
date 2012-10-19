using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accessibility;
using System.Runtime.InteropServices;


namespace EnablePluginFF
{
    public class AccessibleCode
    {
        IntPtr handle;
        IAccessible iAccessible;//interface: Accessibility namespace
        private const int CHILDID_SELF = 0;
        private const int SELFLAG_TAKESELECTION = 0x2;
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
            string accWindowName = iAccessible.get_accName(0);
            string accWindowVal = iAccessible.get_accValue(0);
           /* if ((accWindowName!=null && !accWindowName.Contains("Add-ons Manager - Mozilla Firefox"))||(accWindowVal!=null && !accWindowVal.Contains("Add-ons Manager - Mozilla Firefox")))
                return ;*/

            /*if ((accWindowName != null && !accWindowName.Contains("Mozilla Firefox Start Page - Mozilla Firefox")) || (accWindowVal != null && !accWindowVal.Contains("Mozilla Firefox Start Page - Mozilla Firefox")))
                return;*/

            IAccessible[] childs = new IAccessible[iAccessible.accChildCount];

            int obtained = 0;

            AccessibleChildren(iAccessible, 0, iAccessible.accChildCount - 1, childs, out obtained);

            foreach (IAccessible child in childs)
            {
                if (child != null && child.GetType().IsCOMObject)
                {
                    //Console.Write("Printing the children : ");
                    string cname = ((IAccessible)child).get_accName(0);
                    string cvalue = ((IAccessible)child).get_accValue(0);

                    if(cname != null && cname.Trim() != "")
                        Console.WriteLine("Name is : " + cname);
                    if (cvalue != null && cvalue.Trim() != "")
                        Console.WriteLine("Value is : " + cvalue);
                    if (cname != null && cname.Contains("Add-ons Manager - Mozilla Firefox"))
                    {
                        codeforAddon(child);
                    }
                    getChild(((IAccessible) child),true);
                }
                
            }
        }

        private void codeforAddon(IAccessible obj)
        {
            if (obj == null)
                return;
            Console.WriteLine("Code for Addon");
            IAccessible[] childs = new IAccessible[obj.accChildCount];

            int obtained = 0;

            AccessibleChildren(obj, 0, obj.accChildCount - 1, childs, out obtained);

            foreach (IAccessible child in childs)
            {
                if (child == null)
                    continue;
                string cname = ((IAccessible)child).get_accName(0);
                string cvalue = ((IAccessible)child).get_accValue(0);
                
                string cdesc = child.get_accDescription(0);

                if (cname != null && cname.Trim() != "")
                    Console.WriteLine("Name is : " + cname);
                if (cvalue != null && cvalue.Trim() != "")
                    Console.WriteLine("Value is : " + cvalue);
                if (cdesc != null && cdesc.Trim() != "")
                    Console.WriteLine("Value is : " + cdesc);
            }
            Console.WriteLine("Addon Done");

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

            /*//For TitleBar
            Object[] childs = new Object[iAccessible.accChildCount];
            
            int obtained = 0;

            AccessibleChildren(iAccessible, 0, iAccessible.accChildCount - 1, childs, out obtained);
            
            
            for (int i = 0; i < obtained; i++)
            {
                Console.WriteLine(iAccessible.get_accName(i + 1));
                
            }
             if(!accWindowVal.Contains("Add-ons Manager - Mozilla Firefox"))
                return false;
             */
            //End of for TitleBar
            //For Window
            /*if (!accWindowName.Contains("Add-ons Manager - Mozilla Firefox"))
                return false;*/
            Console.WriteLine(iAccessible.GetType());
            Console.WriteLine("Focus is: " + iAccessible.accFocus);
            Console.WriteLine("Selection is " + iAccessible.get_accState());
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
