using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using CSScriptLibrary;

namespace BytesEditor
{
    public partial class BytesEditor : Form
    {
        System.Data.DataSet _data_set;
        System.Data.DataTable _active_table;
        dynamic _script = null;

        string _BytesPath = string.Empty;
        string _DLLPath = string.Empty;
        string _DefaultExcel = "InitializeTable";
        string _refAssemblies = string.Empty;
        string _bytes_file = string.Empty;
        string _DefaultLanguage = string.Empty;

        public BytesEditor()
        {
            InitializeComponent();
        }
        private void BytesEditor_Load(object sender, EventArgs e)
        {
            try
            {
                Init();
            }catch(System.Exception)
            {
                ResetPath();
            }
        }
        void Init()
        {
            var settings = Properties.Settings.Default;
            if(string.IsNullOrEmpty(settings.DLLPath))
            {
                throw new System.Exception(settings.DLLPath);
            }
            if(settings.WindowLeft > 0)
                this.Left = settings.WindowLeft;
            if(settings.WindowTop > 0)
                this.Top = settings.WindowTop;
            if (settings.WindowLeft > 100)
                this.Width = settings.WindowWidth;
            if (settings.WindowHeight > 100)
                this.Height = settings.WindowHeight;
            this.WindowState = settings.WindowState;

            this._DefaultLanguage = settings.DefaultLanguage;
            this._BytesPath = $"{System.Environment.CurrentDirectory}/{settings.BytesPath}";
            this._DLLPath = $"{System.Environment.CurrentDirectory}/{settings.DLLPath}";
            this._DefaultExcel = settings.DefaultExcel;

            string execute_path = (System.IO.Path.DirectorySeparatorChar == '/' ? "/" : string.Empty) + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BytesEditor)).CodeBase.Replace("file:///", String.Empty));
            string service_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            _DLLPath = System.IO.Path.Combine( execute_path,  settings.DLLPath);
            _BytesPath = System.IO.Path.Combine( execute_path, settings.BytesPath);

            if (string.IsNullOrEmpty(_DefaultLanguage))
            {
                _DefaultLanguage = "eng";
            }

            {
                cmbLanguage.Items.Clear();
                string[] languages = System.IO.Directory.GetDirectories(_BytesPath);

                foreach (var language in languages)
                {
                    string lang = System.IO.Path.GetFileName(language);
                    int index = cmbLanguage.Items.Add(lang);
                    if(_DefaultLanguage == lang)
                    {
                        cmbLanguage.SelectedIndex = index;
                    }
                }
            }

            {
                cmbBytes.Items.Clear();
                string[] dll_files = System.IO.Directory.GetFiles(_DLLPath);
                foreach (var dll_file in dll_files)
                    cmbBytes.Items.Add(System.IO.Path.GetFileNameWithoutExtension(dll_file));
            }
            if(cmbBytes.Items.Count > 0)
            {
                bool exist = false;
                foreach(var item in cmbBytes.Items)
                {
                    if (item.Equals(_DefaultExcel))
                        exist = true;
                }
                if (exist == false)
                {
                    _DefaultExcel = cmbBytes.Items[0].ToString();
                }
            }
            cmbBytes.SelectedText = _DefaultExcel;


            _refAssemblies = $@"{_DLLPath}/{_DefaultExcel}.dll";
            _bytes_file = $@"{_BytesPath}/{_DefaultLanguage}/{_DefaultExcel}.bytes";
            lblMsg.Text = "";
            this.Text = $"BytesEditor language:{_DefaultLanguage} bytes:{_DefaultExcel}";
        }

        private void btnDataLoad_Click(object sender, EventArgs e)
        {
            using (var fs = System.IO.File.OpenRead(_refAssemblies))
            {
                long uncompressedLen = fs.Length;
                byte[] uncompressed = new byte[uncompressedLen];
                fs.Read(uncompressed, 0, (int)uncompressedLen);

                var assem = System.Reflection.Assembly.Load(uncompressed);
                var type = assem.GetType("ScriptLibrary.Script");
                _script = Activator.CreateInstance(type);
                _data_set = _script.ExtractFromBytes(_bytes_file);

                treeView.Nodes.Clear();
                System.Windows.Forms.TreeNode firstNode = null;
                foreach ( System.Data.DataTable table in _data_set.Tables)
                {
                    System.Windows.Forms.TreeNode node = new TreeNode(table.TableName);
                    treeView.Nodes.Add(node);
                    if (firstNode == null)
                        firstNode = node;
                }
                treeView.SelectedNode = firstNode;
                _active_table = _data_set.Tables[0];
                dataGridView.DataSource = _data_set.Tables[0];
                for (int i = dataGridView.Columns.Count - 1; i >= 0; i--)
                {
                    if (i != 0)
                        dataGridView.Columns[i].Frozen = false;
                    else if (i == 0)
                        dataGridView.Columns[i].Frozen = true;
                }
            }
            lblMsg.Text = DateTime.Now + " Load Complete.";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_script == null)
                return;
            _script.SaveFromDataSet(_data_set, $@"{_BytesPath}/{_DefaultLanguage}");
            lblMsg.Text = DateTime.Now + " Save Complete.";
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _active_table = _data_set.Tables[treeView.SelectedNode.Text];
            dataGridView.DataSource = _active_table;
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _DefaultLanguage = cmbLanguage.Items[cmbLanguage.SelectedIndex].ToString();
            _refAssemblies = $@"{_DLLPath}/{_DefaultExcel}.dll";
            _bytes_file = $@"{_BytesPath}/{_DefaultLanguage}/{_DefaultExcel}.bytes";
            dataGridView.DataSource = null;
            treeView.Nodes.Clear();
            this.Text = $"BytesEditor language:{_DefaultLanguage} bytes:{_DefaultExcel}";
            Properties.Settings.Default.DefaultLanguage = this._DefaultLanguage;
            Properties.Settings.Default.Save();
        }

        private void cmbBytes_SelectedIndexChanged(object sender, EventArgs e)
        {
            _DefaultExcel = cmbBytes.Items[cmbBytes.SelectedIndex].ToString();
            _refAssemblies = $@"{_DLLPath}/{_DefaultExcel}.dll";
            _bytes_file = $@"{_BytesPath}/{_DefaultLanguage}/{_DefaultExcel}.bytes";
            dataGridView.DataSource = null;
            treeView.Nodes.Clear();
            this.Text = $"BytesEditor language:{_DefaultLanguage} bytes:{_DefaultExcel}";
            string execute_path = (System.IO.Path.DirectorySeparatorChar == '/' ? "/" : string.Empty) + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BytesEditor)).CodeBase.Replace("file:///", String.Empty)) + "/";
            string service_name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Properties.Settings.Default.DefaultExcel = this._DefaultExcel;
            Properties.Settings.Default.Save();
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var ht = dataGridView.HitTest(e.X, e.Y);
                if ( (ht.RowIndex <= dataGridView.RowCount ) && ht.Type == DataGridViewHitTestType.RowHeader)
                {
                    // This positions the menu at the mouse's location.
                    RowGridMenuStrip.Show(MousePosition);
                    RowGridMenuStrip.Tag = ht;
                    ColumnGridMenuStrip.Tag = null;
                }
                if ((ht.RowIndex <= dataGridView.RowCount) && ht.Type == DataGridViewHitTestType.ColumnHeader)
                {
                    // This positions the menu at the mouse's location.
                    ColumnGridMenuStrip.Show(MousePosition);
                    ColumnGridMenuStrip.Tag = ht;
                    RowGridMenuStrip.Tag = null;
                }
            }
        }

        private void deleteStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RowGridMenuStrip.Tag != null)
            {
                try
                {
                    DataGridView.HitTestInfo ht = RowGridMenuStrip.Tag as DataGridView.HitTestInfo;
                    dataGridView.Rows.RemoveAt(ht.RowIndex);
                    lblMsg.Text = DateTime.Now + " Removed Row.";
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                    MessageBox.Show(lblMsg.Text);
                }
                finally
                {
                    RowGridMenuStrip.Tag = null;
                }
            }
        }


        private void lockStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ColumnGridMenuStrip.Tag != null)
            {
                try
                {
                    DataGridView.HitTestInfo ht = ColumnGridMenuStrip.Tag as DataGridView.HitTestInfo;
                    for( int i = dataGridView.Columns.Count-1; i >= 0;i--)
                    {
                        if(i > ht.ColumnIndex)
                            dataGridView.Columns[i].Frozen = false;
                        else if(i == ht.ColumnIndex)
                            dataGridView.Columns[i].Frozen = true;
                    }
                }
                catch (Exception ex)
                {
                    lblMsg.Text = ex.ToString();
                    MessageBox.Show(lblMsg.Text);
                }
                finally
                {
                    RowGridMenuStrip.Tag = null;
                }
            }
        }

        private void BytesEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private void btnResetPath_Click(object sender, EventArgs e)
        {
            ResetPath();
        }
        void ResetPath()
        {
            string execute_path = (System.IO.Path.DirectorySeparatorChar == '/' ? "/" : string.Empty) + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BytesEditor)).CodeBase.Replace("file:///", String.Empty)) + "/";
            {
                System.Windows.Forms.FolderBrowserDialog folderFialog = new System.Windows.Forms.FolderBrowserDialog();
                folderFialog.SelectedPath = execute_path;
                folderFialog.Description = "Select Bytes Files Path";
                folderFialog.ShowNewFolderButton = false;
                if (folderFialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                Properties.Settings.Default.BytesPath = folderFialog.SelectedPath.Replace(execute_path, string.Empty);
            }
            {
                System.Windows.Forms.FolderBrowserDialog folderFialog = new System.Windows.Forms.FolderBrowserDialog();
                folderFialog.SelectedPath = execute_path;
                folderFialog.Description = "Select Bytes DLL Path";
                folderFialog.ShowNewFolderButton = false;
                if (folderFialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                Properties.Settings.Default.DLLPath = folderFialog.SelectedPath.Replace(execute_path, string.Empty);
            }

            Properties.Settings.Default.Save();

            Init();
        }

        private void BytesEditor_MaximumSizeChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowState = FormWindowState.Maximized;
            Properties.Settings.Default.Save();
        }

        private void BytesEditor_MinimumSizeChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowState = FormWindowState.Minimized;
            Properties.Settings.Default.Save();
        }

        private void BytesEditor_ResizeEnd(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowState = this.WindowState;
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.Save();
        }
    }
}
