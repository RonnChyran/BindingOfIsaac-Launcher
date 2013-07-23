﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;


namespace Installer
{
    public partial class Installer : Form
    {
        FolderBrowserDialog dialog = new FolderBrowserDialog();

        public Installer()
        {
            InitializeComponent();

            //Initialize FolderBrowserDialog with default Steam path
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Steam\steamapps\common\the binding of isaac";
            dialog.Description = "Select where Binding of Isaac: Wrath of the Lamb has been installed";

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Check that all files are present before continuing
            if (!CheckFiles().Equals("good"))
            {
                MessageBox.Show(CheckFiles() + " is missing, installer can not continue");
                this.Close();
            }
            else
            {
                //Ask user if he/she wants to install
                DialogResult confirm = MessageBox.Show("Install The Binding of Isaac Launcher: Revamped?" + Environment.NewLine + "The Binding of Isaac Launcher: Revamped will not install if the Wrath of the Lamb DLC is not installed", "Install The Binding of Isaac Launcher?", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.No)
                {
                    this.Close();
                }

                statusLabel.Text = "Binding of Isaac Launcher: Revamped will install to";
                filePath.Text = dialog.SelectedPath;

            }



        }

        private string CheckFiles()
        {
            //Check for xdelta patch util
 
            if (!File.Exists(Path.Combine(Application.StartupPath, "xdelta.exe")));
            {
                return "xdelta.exe";
            }
            //Check for patch file
            if (!File.Exists(Path.Combine(Application.StartupPath, "patch.xdelta")))
            {
                return "patch.xdelta";
            }
            //Check for launcher file
            if (!File.Exists(Path.Combine(Application.StartupPath, "launcher.exe")))
            {
                return "launcher.exe";
            }
            //Check for AchievementFix
            if (!File.Exists(Path.Combine(Application.StartupPath, "FlashAchievements.exe")))
            {
                return "FlashAchievements.exe";
            }
            //Check for Uninstall file
            if (!File.Exists(Path.Combine(Application.StartupPath, "uninstaller.exe")))
            {
                return "uninstaller.exe";
            }
            //Check for Wrath of the Lamb md5
            if (!File.Exists(Path.Combine(Application.StartupPath, "WotL.md5")))
            {
                return "WotL.md5";
            }
            //check for Vanilla md5
            if (!File.Exists(Path.Combine(Application.StartupPath, "Vanilla.md5")))
            {
                return "Vanilla.md5";
            }
            //If everything is present, we return "good"
            return "good";
        }

        private void installpathbutton_Click(object sender, EventArgs e)
        {
            //Open dialog 
            dialog.ShowDialog();
            filePath.Text = dialog.SelectedPath;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://forums.steampowered.com/forums/showthread.php?t=2216833");
        }

        private void nextbutton_Click(object sender, EventArgs e)
        {

            DialogResult confirm = MessageBox.Show("Launcher will be installed to " + dialog.SelectedPath, "Binding of Isaac Launcher: Revamped", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {

                try
                {

                    installpathbutton.Hide();
                    nextbutton.Hide();
                    filePath.Hide();
                    linkLabel1.Hide();
                    achievementFixBox.Hide();

                    int progBarIncrement;

                    if (achievementFixBox.Checked)
                    {
                        progBarIncrement = 100 / 7;

                    }
                    else
                    {

                        progBarIncrement = 100 / 6;
                    }
                    InstallerProcess installLauncher = new InstallerProcess(dialog.SelectedPath, Application.StartupPath);

                    statusLabel.Text = "Verifying Wrath of The Lamb Executable";
                    statusLabel.Refresh();
                    installLauncher.CheckWrathOfTheLamb();
                    IncrementStatusBar(progBarIncrement);

                    statusLabel.Text = "Creating New Folder Structure";
                    statusLabel.Refresh();
                    installLauncher.CreateFolderStructure();
                    IncrementStatusBar(progBarIncrement);

                    statusLabel.Text = "Patching Wrath of The Lamb Executable to Vanilla";
                    statusLabel.Refresh();
                    installLauncher.PatchWrathOfTheLamb();
                    IncrementStatusBar(progBarIncrement);

                    statusLabel.Text = "Installing Launcher Executable";
                    statusLabel.Refresh();
                    installLauncher.InstallLauncher();
                    IncrementStatusBar(progBarIncrement);

                    if (achievementFixBox.Checked)
                    {
                        statusLabel.Text = "Installing Achievement Fix";
                        statusLabel.Refresh();
                        installLauncher.InstallAchievementFix();
                        IncrementStatusBar(progBarIncrement);
                    }

                    statusLabel.Text = "Copying Dependencies";
                    statusLabel.Refresh();
                    installLauncher.CopyFlashAchievements();
                    IncrementStatusBar(progBarIncrement);

                    statusLabel.Text = "Installing Save File Fixes";
                    statusLabel.Refresh();
                    installLauncher.InstallSavesFix();
                    IncrementStatusBar(progBarIncrement);

                    MessageBox.Show("The Binding of Isaac Launcher has been installed");
                   
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Encountered Exception " + ex.GetType().ToString() + Environment.NewLine + ex.Message + Environment.NewLine
                        + "Verify game cache with Steam and try again." +
                        Environment.NewLine +
                        "If you do not own the Wrath of the Lamb DLC, the Launcher will fail to install."
                        + Environment.NewLine +
                        "If you are re-installing, please uninstall before you attempt installing again"
                        );
                    InstallerProcess.ExceptionCleanup(dialog.SelectedPath);
                }

                finally
                {
                    this.Close();
                }
            }

        }

        private void IncrementStatusBar(int increment)
        {
            progressBar1.Increment(increment);
            progressBar1.Refresh();
            //Sleep for half a second, for aesthetic purposes :)
            System.Threading.Thread.Sleep(500);

        }
    }

}
