using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Worms2
{
    public partial class MainWindow : Window
    {
        private const string UniqueEventName = "Worms 2 Launcher by Carlmundo";
        string fileExtension = ".wmv";
        string msgFileNotFound = "File not found: ";
        int forceClose = 0;

        public static string folderPrefix = "..\\";

        public MainWindow()
        {
            SingleInstance();
            Process[] Process1 = Process.GetProcessesByName("frontend");
            Process[] Process2 = Process.GetProcessesByName("worms2");
            if (Process1.Length > 0 || Process2.Length > 0)
            {
                MessageBox.Show("Worms 2 is already running.");
                forceClose = 1;
                Close();
            }
            else
            {
                InitializeComponent();
                if (
                    //Windows XP
                    System.Environment.OSVersion.Platform.ToString() == "Win32NT" &
                    System.Environment.OSVersion.Version.Major.ToString() == "5" &
                    System.Environment.OSVersion.Version.Minor.ToString() == "1"
                    )
                {
                    string verWMP = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\MediaPlayer\\PlayerUpgrade", "PlayerVersion", "0").ToString();
                    int indexWMP = verWMP.IndexOf(",");
                    if (indexWMP >= 0)
                    {
                        verWMP = verWMP.Substring(0, indexWMP);
                    }
                    int.TryParse(verWMP, out int verWMPint);
                    if (verWMPint < 11)
                    {
                        MessageBox.Show("Please install Windows Media Player 11 to be able to play the intro videos.");
                        Close();
                    }
                }
                string FileExt = "ext.txt";
                if (File.Exists(FileExt))
                {
                    string txtContents = File.ReadAllText(FileExt).Trim();
                    if (txtContents.Length == 3 || txtContents.Length == 4)
                    {
                        fileExtension = "." + txtContents;
                    }
                }

                string fileIntro = "Intro" + fileExtension;
                if (!File.Exists(folderPrefix + fileIntro))
                {
                    MessageBox.Show(msgFileNotFound + fileIntro);
                    Close();
                }
                else
                {
                    VideoPlayer.MediaEnded += OnMediaEnded;
                    VideoPlayer.MediaFailed += OnMediaEnded;
                    VideoPlayer.Source = new Uri(folderPrefix + fileIntro, UriKind.Relative);
                    VideoPlayer.Play();
                    this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
                }
            }
        }
        private void SingleInstance()
        {
            try
            {
                EventWaitHandle.OpenExisting(UniqueEventName);
                forceClose = 1;
                Close();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                VideoPlayer.Position = TimeSpan.MaxValue;
            }
        }
        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            var currentVideo = VideoPlayer.Source.ToString();
            if (currentVideo == folderPrefix + "Intro" + fileExtension)
            {
                string[] videoList = { "ARMAG", "BANDIT", "BASEBALL", "GRENADE1", "PINGPONG", "TV", "VIDCAM" };
                Random random = new Random();
                var randomVideoIndex = random.Next(0, videoList.Length);
                var randomVideoFile = videoList[randomVideoIndex] + fileExtension;
                if (!File.Exists(folderPrefix + randomVideoFile))
                {
                    MessageBox.Show("File not found: " + randomVideoFile);
                    Close();
                }
                else
                {
                    VideoPlayer.Source = new Uri(folderPrefix + randomVideoFile, UriKind.Relative);
                }
            }
            else
            {
                Close();
            }     
        }
        private void Main_Closing(object sender, CancelEventArgs e)
        {
            if (forceClose != 1)
            {
                var appLaunch = "frontend.exe";
                if (File.Exists(folderPrefix + appLaunch))
                {
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo();
                    _processStartInfo.WorkingDirectory = Path.Combine(Environment.CurrentDirectory, folderPrefix);
                    _processStartInfo.FileName = appLaunch;
                    Process myProcess = Process.Start(_processStartInfo);
                }
                else
                {
                    MessageBox.Show(msgFileNotFound + appLaunch);
                }
            }
                
        }
    }
}
