//#define EXCEPTION 
//To throw exceptions
#define PROC
//To use 2 different functions
//#define TEST
//For testing
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;

namespace EnablePluginFF
{

    public class EnablePlugin
    {
        //Class and WindowName for firefox
        string className = "MozillaWindowClass";
        string windowName = "Mozilla Firefox";

        //boolean variable which is set to true if firefox exists
        bool FirfoxExists = false;
        string FFpathExe;
        string addonURL = " about:addons ";

  
        //Reference : http://www.codeproject.com/Articles/27672/Programmatically-detecting-browser-cache-size-for
        private void getFFExepath()
        {
            string finalPathName, InstallLocation;

            RegistryKey finalPath;

            //path where registry is
            string initPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            //Opening Registery
            RegistryKey theKeyHKLMFFE = Registry.LocalMachine.OpenSubKey(initPath);

            //Getting all Registry Subkey names
            string[] pathVals = theKeyHKLMFFE.GetSubKeyNames();

            //Closing Registry
            theKeyHKLMFFE.Close();

            InstallLocation = "Not Installed";

            //Loop all the names and try and find firefox
            foreach (string soft in pathVals)
            {
                finalPathName = initPath + "\\" + soft;
                finalPath = Registry.LocalMachine.OpenSubKey(finalPathName);
                string[] subValueNames = finalPath.GetValueNames();
                foreach (string subVals in subValueNames)
                {
                    if (subVals == "DisplayName")
                    {
                        if (finalPath.GetValue("DisplayName") != null &&
                            finalPath.GetValue("DisplayName").ToString().
                                                 ToUpper().Trim().Contains("FIREFOX"))
                        {
                            InstallLocation = finalPath.GetValue("InstallLocation").ToString();
                            FirfoxExists = true;
                            break;
                        }
                    }
                }
            }


            if (FirfoxExists == true)
            {
                FFpathExe = InstallLocation;
                Console.WriteLine("Found Firefox at the location : " + FFpathExe);
            }
            else
            {
                FFpathExe = "FireFox is not installed on the system.";
                Console.WriteLine(FFpathExe);
            }
            
        }



        public IntPtr getHandle()
        {

            List<IntPtr> handles;
#if (!PROC)
            {
                handles = WindowHandle.SearchForWindow(className);
            }
#else
            {
                handles = WindowHandle.SearchForWindow(className, windowName);
            }
#endif

            Console.WriteLine("The number of Handles found : " + handles.Count);

            /* Start Firefox and return the handle of this newly started firefox
            */
            if (handles.Count == 0)
                return startFF();

            /* There is only 1 instance, so just return the Handle*/
            else if (handles.Count == 1)
                return handles.ElementAt(0);

            else //(handles.Count > 1)
            {
                //Handle this condition and make sure we are selecting firefox
                IntPtr h = getFFHandle(handles);

                //If we still did not get Firefox Start it after checking the Caption
                if (h == IntPtr.Zero)
                    return startFF();
                else
                    return h;
            }

        }

        //Checking that we are selecting firefox by using title, as well - the caption of FF web pages contains Mozilla Firefox
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);



        private IntPtr getFFHandle(List<IntPtr> handles)
        {
            int i = 0;
            IntPtr h = IntPtr.Zero;
            foreach (IntPtr handle in handles)
            {
                StringBuilder sb = new StringBuilder(1024);
                GetWindowText(handle, sb, sb.Capacity);
                Console.WriteLine("The Caption of handle {0} is {1}", handle, sb.ToString());
                if (sb.ToString().Contains(windowName))
                {
                    h = handle;
                    i++;
                }
            }
            if (i > 1)
            {
                Console.WriteLine("There are still more than 1 instances of firefox handle, we are selecting the last instance");
            }
            return h;
        }//end getFFHandle from a list of handles


        //Start FireFox and open the addons web page, throws an Exception if Firefox does not exist on the system
        //Reference : http://stackoverflow.com/questions/2494451/how-to-obtain-firefox-newly-created-window-handler
        private IntPtr startFF()
        {
            IntPtr hwnd = IntPtr.Zero;
#if(TEST)
            FFpathExe = "C:\\Program Files (x86)\\Mozilla Firefox\\firefo.exe";
#endif
            
            //Call to this function sets the class variables FireFoxExists & FFPathExe if FF exists
            getFFExepath();

            //Hard Coded this - Is this also supposed to be read from the registry??
            string FFexe = "\\firefox.exe";

            System.Diagnostics.Process browserProc = new System.Diagnostics.Process();
            if (FirfoxExists == false)
            {
                Exception ex = new Exception(FFpathExe);
                throw ex;
            }

            browserProc.StartInfo.FileName = FFpathExe+FFexe;
            browserProc.StartInfo.Arguments = addonURL;

            try
            {
                browserProc.Start();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Console.WriteLine("FF is not installed at the specified location or it is not installed in the system.");
                Console.WriteLine(ex.Message);
                return IntPtr.Zero;

#if(EXCEPTION)
                {
                    throw ex;
                }
#endif
            }//end catch
            do
            {
                Thread.Sleep(100);
                browserProc.Refresh();
            } while (browserProc.MainWindowHandle == IntPtr.Zero && !browserProc.HasExited);

            if (!browserProc.HasExited)
                hwnd = browserProc.MainWindowHandle;
            return hwnd;
        }// end startFF

        //For closing the FireFox Window
        //Reference : http://www.codeproject.com/Articles/22257/Find-and-Close-the-Window-using-Win-API


        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public bool closeWindow(IntPtr handle)
        {
            // close the window using API    
            int iHandle = handle.ToInt32();
            SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
            return false;
        }//end closeWindow
    }
}
