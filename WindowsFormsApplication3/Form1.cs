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
    }
}
