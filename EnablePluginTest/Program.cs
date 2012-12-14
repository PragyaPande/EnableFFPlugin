//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnablePluginFF;
using System.Runtime.InteropServices;



namespace EnablePluginTest
{

    class Program
    {


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(" The Program should be run as program Name Extension name logfilepath");

            }
            else
            {

                string extension = args[0];
                string logfile = args[1];

                //To Test this, set function getFFExepath() in EnablePlugin.cs to public
                //testFFinstallation();

                testGetHandle(extension, logfile);
            }

        }



        //Function to test the function which finds the FF installation folder
        private static void testFFinstallation()
        {
            EnablePlugin obj = new EnablePlugin();

            //To Test this, set function getFFExepath() in EnablePlugin.cs to public and uncomment the below line
            //obj.getFFExepath();

            Console.ReadLine();
        }

        //This functions has the Actual Algorithm 
        private static void testGetHandle(string extension, string logfile)
        {

            EnablePlugin obj = new EnablePlugin();

            IntPtr handle;

            /*The actual algorithm is here.*/
            {
                ///Get the handle for FF : getHandle() function
                /// 1. If there is no instance of FF running on the system, it runs FF and returns the handle of that instance.
                /// 2. If there are more than 1 instance of FF running on the system, then it returns the IntPtr.Zero.
                /// 3. If there is firefox Browser and Thunderbird both running, then it returns the handle of FF
                /// 4. if there are more than 1 instance of FF browser and Thunderbird running, then it returns the IntPtr.Zero
                /// 5. if FF is not running and we are not able to run it(i.e. it is not installed), it throws an exception saying that FF not installed
                handle = obj.getHandle();


                /*This is to log if we get handle zero even after all the checks are done*/
                if (handle != IntPtr.Zero)
                {
                    Console.WriteLine("Found Handle for Firefox");
                    AccessibleCode acobj = new AccessibleCode(handle);

                    //SET THE EXTENSION NAME HERE, now we are setting it through the command line
                    //const string ext = "XTalk 2.36 (disabled) An Extension for HearSay"

                    int return_code = acobj.doAccessibleHandle(extension, logfile);

                    //Close the window
                    obj.closeWindow(handle);
                    Console.WriteLine("Return Code : " + return_code); 
                    //Console.WriteLine("Done");

                }
                else
                {

                    Console.WriteLine("Try Again After Closing Firefox.");
                }
            }
        }
    }
}

