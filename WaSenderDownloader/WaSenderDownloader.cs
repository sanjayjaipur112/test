using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace WaSenderDownloader
{
    public partial class WaSenderDownloader : Form
    {
        private string downloadUrl = "https://example.com/WaSender.zip"; // Replace with your actual download URL
        private string installPath;
        private string zipPath;

        public WaSenderDownloader()
        {
            InitializeComponent();
            installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WaSender");
            zipPath = Path.Combine(Path.GetTempPath(), "WaSender.zip");
        }

        private void WaSenderDownloader_Load(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            
            // Start the download process
            StartDownload();
        }

        private void StartDownload()
        {
            statusLabel.Text = "Downloading WaSender...";
            
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(downloadUrl), zipPath);
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error downloading file: " + e.Error.Message, "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            statusLabel.Text = "Installing WaSender...";
            progressBar.Value = 0;
            
            // Extract the ZIP file
            ExtractFiles();
        }

        private void ExtractFiles()
        {
            try
            {
                // Create the installation directory if it doesn't exist
                if (!Directory.Exists(installPath))
                {
                    Directory.CreateDirectory(installPath);
                }

                // Extract the ZIP file
                ZipFile.ExtractToDirectory(zipPath, installPath);
                
                // Create desktop shortcut
                CreateShortcut();
                
                // Clean up the ZIP file
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                statusLabel.Text = "Installation complete!";
                progressBar.Value = 100;
                
                // Launch the application
                LaunchApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error installing WaSender: " + ex.Message, "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateShortcut()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string shortcutPath = Path.Combine(desktopPath, "WaSender.lnk");
            string targetPath = Path.Combine(installPath, "WASender.exe");

            // Create a shortcut using Windows Script Host
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = installPath;
            shortcut.Description = "WaSender Application";
            shortcut.IconLocation = targetPath + ",0";
            shortcut.Save();
        }

        private void LaunchApplication()
        {
            string exePath = Path.Combine(installPath, "WASender.exe");
            
            if (File.Exists(exePath))
            {
                Process.Start(exePath);
                
                // Close the downloader after a short delay
                Timer timer = new Timer();
                timer.Interval = 2000; // 2 seconds
                timer.Tick += (s, e) => 
                {
                    timer.Stop();
                    Application.Exit();
                };
                timer.Start();
            }
            else
            {
                MessageBox.Show("WaSender executable not found. Installation may be incomplete.", "Launch Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 200);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(460, 23);
            this.progressBar.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 180);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(89, 13);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "Initializing...";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(460, 150);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // WaSenderDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 241);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WaSenderDownloader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaSender Installer";
            this.Load += new System.EventHandler(this.WaSenderDownloader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.PictureBox pictureBox;
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WaSenderDownloader());
        }
    }
}
