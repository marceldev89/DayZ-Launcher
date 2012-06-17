using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        static string Version = "0.2";
        
        string ConfigPath;

        string A2Path;
        string A2OAPath;

        bool LoadDayZ;
        bool RunBeta;

        public Form1()
        {
            InitializeComponent();

            this.Text += " v" + Version;

            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            LoadDayZ = false;
            RunBeta = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string value;

            ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\dayzlauncher.xml";

            if (File.Exists(ConfigPath))
            {
                XDocument doc = XDocument.Load(ConfigPath);

                LoadDayZ = Convert.ToBoolean((from xml2 in doc.Descendants("config") select xml2.Element("dayz").Value).FirstOrDefault());
                RunBeta = Convert.ToBoolean((from xml2 in doc.Descendants("config") select xml2.Element("beta").Value).FirstOrDefault());

                if (LoadDayZ)
                {
                    checkBox2.Checked = true;
                }

                if (RunBeta)
                {
                    checkBox1.Checked = true;
                }
            }

            // Find ArmA 2 path
            value = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2", "main", null);

            if (value != null)
            {
                A2Path = value;
            }
            else
            {
                value = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2", "main", null);

                if (value != null)
                {
                    A2Path = value;
                }
                else
                {
                    A2Path = null;
                }
            }

            // Find ArmA 2 OA path
            value = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA", "main", null);

            if (value != null)
            {
                A2OAPath = value;
            }
            else
            {
                value = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA", "main", null);

                if (value != null)
                {
                    A2OAPath = value;
                }
                else
                {
                    A2OAPath = null;
                }
            }

            if (A2Path == null || A2OAPath == null)
            {
                MessageBox.Show("You haven't installed A2:CO properly!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            if (!File.Exists(A2OAPath + "\\@dayz\\addons\\dayz_code.pbo"))
            {
                //MessageBox.Show("You haven't installed DayZ properly!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.Close();
                LoadDayZ = false;
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
                checkBox2.Text += " (Not installed)";
            }

            if (!File.Exists(A2OAPath + "\\Expansion\\beta\\arma2oa.exe"))
            {
                RunBeta = false;
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
                checkBox1.Text += " (Not installed)";
            }
            else
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(A2OAPath + "\\Expansion\\beta\\arma2oa.exe");
                checkBox1.Text += " (" + info.ProductVersion + ")";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if (RunBeta)
            {
                startInfo.FileName = A2OAPath + "\\Expansion\\beta\\arma2oa.exe";
            }
            else
            {
                startInfo.FileName = A2OAPath + "\\arma2oa.exe";
            }

            startInfo.WorkingDirectory = A2OAPath;

            startInfo.Arguments = "\"-mod=" + A2Path + ";Expansion;ca";

            if (RunBeta)
            {
                startInfo.Arguments += ";Expansion\\beta;Expansion\\beta\\Expansion";
            }

            if (LoadDayZ)
            {
                startInfo.Arguments += ";@dayz";
            }

            startInfo.Arguments += "\" -nosplash -world=empty";

            Process.Start(startInfo);
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                RunBeta = true;
            }
            else
            {
                RunBeta = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                LoadDayZ = true;
            }
            else
            {
                LoadDayZ = false;
            }
        }

        void Application_ApplicationExit(object sender, EventArgs e)
        {
            XDocument doc = new XDocument(
                new XElement("config",
                    new XElement("version", Version),
                    new XElement("dayz", LoadDayZ),
                    new XElement("beta", RunBeta)));

            doc.Save(ConfigPath);
        }
    }
}
