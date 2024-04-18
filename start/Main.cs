using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace start
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();
            string appFolder = "VideoPlayer\\";
            string videoPlayer = "intro.exe";
            if (File.Exists(appFolder + videoPlayer))
            {
                ProcessStartInfo _processStartInfo = new ProcessStartInfo();
                _processStartInfo.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, appFolder);
                _processStartInfo.FileName = videoPlayer;
                Process myProcess = Process.Start(_processStartInfo);
            }
            else
            {
                var appLaunch = "frontend.exe";
                if (File.Exists(appLaunch))
                {
                    Process.Start(appLaunch);
                }
                else
                {
                    MessageBox.Show("File not found: " + appLaunch);
                }
            }
            Close();
        }
    }
}
