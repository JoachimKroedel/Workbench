using System;
using System.Windows.Forms;

namespace ConceptisPuzzles
{
    public partial class DialogLoadPuzzle : Form
    {
        public DialogLoadPuzzle()
        {
            InitializeComponent();
        }

        public string InitialDirectory { get; set; }

        public string FileName { get; set; }

        private void DialogLoadPuzzle_Load(object sender, EventArgs e)
        {
            RefreshFiles();
        }

        private void RefreshFiles()
        {
            _cbxFilterState.SelectedIndexChanged -= _cbxFilterState_SelectedIndexChanged;
            _txtDirectory.Text = InitialDirectory;
            _dataGridView.Rows.Clear();
            _cbxFilterState.Items.Clear();
            _cbxFilterState.Items.Add("");
            foreach(string fileName in System.IO.Directory.GetFiles(InitialDirectory))
            {
                if (fileName.EndsWith(".fap"))
                {
                    string fileText = System.IO.File.ReadAllText(fileName);
                    string[] splitRows = fileText.Split(new char[]{'\n'});
                    string[] splitInfo = splitRows[0].Split(new char[] { ';' });
                    if (splitInfo.Length >= 6)
                    {
                        int index = _dataGridView.Rows.Count;
                        _dataGridView.Rows.Add();

                        _dataGridView.Rows[index].Cells["_colFileName"].Value = fileName;
                        _dataGridView.Rows[index].Cells["_colSize"].Value = splitInfo[0] + "x" + splitInfo[1];
                        _dataGridView.Rows[index].Cells["_colDate"].Value = splitInfo[2];
                        _dataGridView.Rows[index].Cells["_colIndex"].Value = splitInfo[3];
                        _dataGridView.Rows[index].Cells["_colLevel"].Value = splitInfo[4];
                        _dataGridView.Rows[index].Cells["_colState"].Value = splitInfo[5];
                        if (!_cbxFilterState.Items.Contains(splitInfo[5]))
                        {
                            _cbxFilterState.Items.Add(splitInfo[5]);
                        }
                        if (splitInfo.Length >= 7)
                        {
                            _dataGridView.Rows[index].Cells["_colSaveDate"].Value = splitInfo[6];
                        }
                    }
                }
            }
            _cbxFilterState.SelectedIndex = 0;
            _cbxFilterState.SelectedIndexChanged += _cbxFilterState_SelectedIndexChanged;
        }

        private void _btnLoad_Click(object sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count > 0)
            {
                FileName = _dataGridView.Rows[_dataGridView.SelectedRows[0].Index].Cells["_colFileName"].Value as string;
            }
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void _dataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _btnLoad_Click(sender, e);
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void _cbxFilterState_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_cbxFilterState.Text))
            {
                RefreshFiles();
                return;
            }
            int index = 0;
            while(index < _dataGridView.Rows.Count)
            {
                if (_cbxFilterState.Text.Equals(_dataGridView.Rows[index].Cells["_colState"].Value as string))
                {
                    index++;
                }
                else
                {
                    _dataGridView.Rows.RemoveAt(index);
                }
            }
            _dataGridView.Refresh();
        }

        private void _btnChangeDirectory_Click(object sender, EventArgs e)
        {
            _folderBrowserDialog.SelectedPath = InitialDirectory;
            if (_folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                InitialDirectory = _folderBrowserDialog.SelectedPath;
                RefreshFiles();
            }
        }
    }
}
