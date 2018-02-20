using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            cmbTypes.Text = "ALL";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                using (var fileBrowser = new OpenFileDialog())
                {
                    if (fileBrowser.ShowDialog() == DialogResult.OK)
                    {
                        tbFilePath.Text = fileBrowser.FileName;
                        LoadFile(fileBrowser.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadFile(string fileName)
        {
            if (Directory.Exists(fileName))
            {
                var files = new DirectoryInfo(fileName).GetFiles();

                FillFilesDropdown(files);
            }
            else if (File.Exists(fileName))
            {
                cmbFiles.DataSource = null;

                FillFilesDropdown(new[] { new FileInfo(fileName) });

                FillGrid(fileName);
            }
        }

        private void FillGrid(string fileName)
        {
            try
            {
                grd.DataSource = null;
                lblRecordCount.Text = "Reading...";
                this.Enabled = false;

                var records = LogReader.LoadFile(fileName);

                if (records?.Any() ?? false)
                {
                    var keyword = tbSearch.Text;

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        if (!chNotIn.Checked)
                        {
                            records = records.Where(m =>
                                                m.Message?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Date.ToString("dd-MM-yyyy HH:mm:ss").IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Payload?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Url?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.StackTrace?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1).ToList(); 
                        }
                        else
                        {
                            records = records.Where(m =>
                                                !(m.Message?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Date.ToString("dd-MM-yyyy HH:mm:ss").IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Payload?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.Url?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                                                m.StackTrace?.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase) > -1)).ToList();
                        }
                    }

                    grd.DataSource = records;

                    LayoutGrid();
                }
            }
            catch (Exception ex)
            {
                lblRecordCount.Text = string.Empty;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Enabled = true;
            }
        }

        private void LayoutGrid()
        {
            var sChar = grd.RowCount == 1 ? string.Empty : "s";

            lblRecordCount.Text = $"{grd.RowCount} row{sChar}";

            if (grd.Columns.Contains("Message"))
            {
                grd.Columns["Message"].Width = 420;
            }
            if (grd.Columns.Contains("Date"))
            {
                grd.Columns["Date"].Width = 130;
            }
            if (grd.Columns.Contains("Payload"))
            {
                grd.Columns["Payload"].Width = 200;
            }
            if (grd.Columns.Contains("StackTrace"))
            {
                grd.Columns["StackTrace"].Width = 200;
            }
            if (grd.Columns.Contains("Url"))
            {
                grd.Columns["Url"].Width = 250;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (filePaths != null && filePaths.Length > 0)
                    {
                        tbFilePath.Text = filePaths[0];
                        LoadFile(filePaths[0]);
                    }
                }
            }
            catch { }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            }
            catch { }
        }

        private void tbFilePath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadFile(tbFilePath.Text);
            }
        }

        private void cmbTypes_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var config = Config.Get();

            if (config != null)
            {
                tbFilePath.Text = config.Filename;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbFilePath.Text))
            {
                Config.Save(new Config { Filename = tbFilePath.Text.Trim() });
            }
        }

        private void cmbFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbFiles.Text))
            {
                FillGrid(cmbFiles.SelectedValue as string);
            }
        }

        private void FillFilesDropdown(IEnumerable<FileInfo> files)
        {
            cmbFiles.DataSource = null;

            if (files?.Any() ?? false)
            {
                var selectedFiles = files
                    .Where(m => m.Extension.IndexOf("txt", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                           m.Extension.IndexOf("log", StringComparison.InvariantCultureIgnoreCase) > -1)
                    .OrderByDescending(m => m.LastWriteTime)
                    .ToArray();

                cmbFiles.ValueMember = "FullName";
                cmbFiles.DisplayMember = "Name";
                cmbFiles.DataSource = selectedFiles;
            }
        }

        private void cmbMsg_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGrid(cmbFiles.SelectedValue as string);
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FillGrid(cmbFiles.SelectedValue as string);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                tbSearch.Focus();
            }
        }

        private void chNotIn_CheckedChanged(object sender, EventArgs e)
        {
            FillGrid(cmbFiles.SelectedValue as string);
        }
    }
}
