using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security;

namespace InjectedDLL
{
    public class InjectDLL
    {

        public bool DLLMain()
        {
            // Find the target window handle.
            IntPtr hTargetWnd = NativeMethod.FindWindow(null, "Injector");
            if (hTargetWnd == IntPtr.Zero)
            {
                System.IO.File.WriteAllText(@"DLLlog.txt", "Unable to find the Injector window");
            }

            // Prepare the COPYDATASTRUCT struct with the data to be sent.
            MyStruct myStruct;

            myStruct.Number = 1;
            myStruct.Message = "Send Data Example";

            // Marshal the managed struct to a native block of memory.
            int myStructSize = Marshal.SizeOf(myStruct);
            IntPtr pMyStruct = Marshal.AllocHGlobal(myStructSize);
            try
            {
                Marshal.StructureToPtr(myStruct, pMyStruct, true);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.cbData = myStructSize;
                cds.lpData = pMyStruct;

                // Send the COPYDATASTRUCT struct through the WM_COPYDATA message to 
                // the receiving window. (The application must use SendMessage, 
                // instead of PostMessage to send WM_COPYDATA because the receiving 
                // application must accept while it is guaranteed to be valid.)                
                NativeMethod.SendMessage(hTargetWnd, WM_COPYDATA, hTargetWnd, ref cds);

                int result = Marshal.GetLastWin32Error();
                if (result != 0)
                {
                    System.IO.File.WriteAllText(@"DLLlog.txt", "SendMessage(WM_COPYDATA) failed w/err 0x{0:" + result.ToString() + "}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMyStruct);
            }
            return true;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MyStruct
        {
            public int Number;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Message;
        }


        #region Native API Signatures and Types

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another 
        /// application
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;


        /// <summary>
        /// The COPYDATASTRUCT structure contains data to be passed to another 
        /// application by the WM_COPYDATA message. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // Specifies data to be passed
            public int cbData;          // Specifies the data size in bytes
            public IntPtr lpData;       // Pointer to data to be passed
        }


        /// <summary>
        /// The class exposes Windows APIs to be used in this code sample.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary>
            /// Sends the specified message to a window or windows. The SendMessage 
            /// function calls the window procedure for the specified window and does 
            /// not return until the window procedure has processed the message. 
            /// </summary>
            /// <param name="hWnd">
            /// Handle to the window whose window procedure will receive the message.
            /// </param>
            /// <param name="Msg">Specifies the message to be sent.</param>
            /// <param name="wParam">
            /// Specifies additional message-specific information.
            /// </param>
            /// <param name="lParam">
            /// Specifies additional message-specific information.
            /// </param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
                IntPtr wParam, ref COPYDATASTRUCT lParam);


            /// <summary>
            /// The FindWindow function retrieves a handle to the top-level window 
            /// whose class name and window name match the specified strings. This 
            /// function does not search child windows. This function does not 
            /// perform a case-sensitive search.
            /// </summary>
            /// <param name="lpClassName">Class name</param>
            /// <param name="lpWindowName">Window caption</param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        #endregion
    }
}

