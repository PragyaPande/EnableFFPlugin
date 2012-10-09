using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//Reference : http://www.pinvoke.net/default.aspx/user32.enumwindows
namespace EnablePluginFF
{
    class WindowHandle
    {
        
        //Delegate for EnumWindowProc , this is the delegate for the callback function
        private delegate bool EnumWindowsProc(IntPtr hWnd, ref WindowData data);

        public static List<IntPtr> SearchForWindow(string wndclass)
        {
            WindowData sd = new WindowData 
            { Wndclass = wndclass, 
              hWnd = new List<IntPtr>()
            };

            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        public static List<IntPtr> SearchForWindow(string wndclass,string title)
        {
            WindowData sd = new WindowData
            {
                Wndclass = wndclass,
                windowName = title,
                hWnd = new List<IntPtr>()
            };

            EnumWindows(new EnumWindowsProc(EnumProcwithTitle), ref sd);
            return sd.hWnd;
        }


        //Callback Function
        public static bool EnumProc(IntPtr hWnd, ref WindowData data)
        {
            //Check for className and get list of handles
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString().Equals(data.Wndclass))
            {
                //Add more conditions for differenting between different instances of firefox
                data.hWnd.Add(hWnd);
                return true; //Continue Enumerating and adding
            }
            return true;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        //Callback Function
        public static bool EnumProcwithTitle(IntPtr hWnd, ref WindowData data)
        {
            //Check for className and get list of handles
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString().Equals(data.Wndclass))
            {
                StringBuilder title = new StringBuilder(1024);
                GetWindowText(hWnd, title, title.Capacity);
                if(title.ToString().Contains(data.windowName))
                    data.hWnd.Add(hWnd);
                return true; //Continue Enumerating and adding
            }
            return true;
        }
        /*This is the Search Data Class which contains the 
        1.windowClassName to be searched
        2. The Title
        3. The Handle 
         * This is a public class through which we will get Handle in Main class
        */
        public class WindowData
        {
            public string Wndclass;
            public string windowName;
            public List<IntPtr> hWnd;
        } 

        //Iterate over the Windows
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref WindowData data);

        //Get the ClassName
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    }
}
