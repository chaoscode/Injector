using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!Directory.Exists("DLLs"))
            {
                Directory.CreateDirectory("DLLs");
            }
            FillProcessList();
            FillDLLList();
        }

        public void FillProcessList()
        {
            ProcessList.Items.Clear();
            Process[] processlist = Process.GetProcesses();
            foreach(Process theprocess in processlist){
                ProcessList.Items.Add(theprocess.ProcessName + " - " + theprocess.Id.ToString());                
            }
        }

        public void FillDLLList()
        {
            var fileList = Directory
                .EnumerateFiles("DLLs", "*.dll", SearchOption.AllDirectories)
                .Select(Path.GetFullPath); // <-- note you can shorten the lambda
            DLLListBox.DataSource = fileList.ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillProcessList();
        }

        private void ProcessList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string spliter = ProcessList.Text;
            int postion = spliter.IndexOf("-");
            ProcessIDBox.Text = spliter.Remove(0, postion + 2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FillDLLList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            injector injectdll = new injector();
            injectdll.InjectByProcessID(DLLListBox.Text,Convert.ToInt32(ProcessIDBox.Text));
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                // Get the COPYDATASTRUCT struct from lParam. 
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                // If the size matches 
                if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))
                {
                    // Marshal the data from the unmanaged memory block to a  
                    // MyStruct managed struct. 
                    MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData,
                        typeof(MyStruct));

                    // Display the MyStruct data members. 
                    OutPut.AppendText("Number: " + myStruct.Number.ToString() + Environment.NewLine + "Message: " + myStruct.Message);                   
                }
            }

            base.WndProc(ref m);
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
        /// application. 
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

        #endregion 

        private void OutPut_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
