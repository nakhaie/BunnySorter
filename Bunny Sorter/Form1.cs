using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Bunny_Sorter
{
    public partial class Form1 : Form
    {
        private string _rootPath;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            
        // Set validate names and check file exists to false otherwise windows will
        // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            
        // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";

            if (folderBrowser.ShowDialog() != DialogResult.OK)
                return;
            
            _rootPath = Path.GetDirectoryName(folderBrowser.FileName);
            textBox1.Text = _rootPath;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            _rootPath = textBox1.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _rootPath = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = string.Empty;
            _rootPath   = textBox1.Text;
            
            if(_rootPath == string.Empty)
                return;

            var dirsPath = Directory.GetDirectories(_rootPath);
            
            if (dirsPath.Length < 1)
            {
                MessageBox.Show(@"There is no plan directory in this path!", @"Incorrect Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string errorsDescription = string.Empty;

            Dictionary<string, int> layersList = new Dictionary<string, int>();
            
            foreach (var dir in dirsPath)
            {
                string[] allFilesPaths = Directory.GetFiles(dir, $"{dir.Split('\\').Last()}.*");
                
                if (allFilesPaths.Length < 1)
                {
                    errorsDescription += $"{dir} (Empty!)\n";
                }
                else
                {
                    label2.Text += $"{dir.Split('\\').Last()}\n";
                    layersList.Clear();
                    
                    foreach (var filesPath in allFilesPaths)
                    {
                        string planType = filesPath.Split('\\').Last();
                        string[] planTypeSplit = planType.Split('.');
                        
                        if (planTypeSplit.Length < 4)
                        {
                            errorsDescription += $"{dir} (No Types!)\n";
                            break;
                        }

                        planType = planTypeSplit[1];
                        
                        if (layersList.ContainsKey(planType))
                        {
                            layersList[planType]++;
                        }
                        else
                        {
                            layersList.Add(planType,1);
                        }

                        string planTypeDir = textBox2.Text;
                        
                        planTypeDir = planTypeDir == String.Empty ? $@"{dir}\{planType}" : $@"{dir}\{textBox2.Text}\{planType}";

                        if (!Directory.Exists(planTypeDir))
                        {
                            Directory.CreateDirectory(planTypeDir);
                        }

                        if (planTypeSplit[2] != "0000")
                        {
                            planTypeDir = $"{planTypeDir}\\{filesPath.Split('\\').Last()}";

                            File.Move(filesPath, planTypeDir);
                        }
                    }

                    foreach (var key in layersList.Keys)
                    {
                        label2.Text     += $"   {key} = {layersList[key]}\n";
                    }
 
                }
            }

            if (errorsDescription != string.Empty)
            {
                errorsDescription = errorsDescription.Insert(0, "There is no plan file in this path! \n");
                MessageBox.Show(errorsDescription, @"Incorrect Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}