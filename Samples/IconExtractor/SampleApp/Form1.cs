using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using WinCopies.GUI.Drawing;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        IconExtractor m_iconExtractor = null;
        int m_iconIndex = 0;

        public Form1() => InitializeComponent();

        private void ClearAllIcons()
        {
            foreach (object item in lvwIcons.Items)

                ((IconListViewItem)item).Bitmap.Dispose();

            lvwIcons.Items.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => ClearAllIcons();

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            if (iconPickerDialog.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = iconPickerDialog.FileName;
                m_iconIndex = iconPickerDialog.IconIndex;

                Icon icon ;
                Icon[] splitIcons ;
                try
                {
                    if (Path.GetExtension(iconPickerDialog.FileName).ToLower() == ".ico")
                    {
                        m_iconExtractor = null;
                        icon = new Icon(iconPickerDialog.FileName);
                    }

                    else
                    {
                        m_iconExtractor = new IconExtractor(fileName);
                        icon = m_iconExtractor.GetIcon(m_iconIndex);
                    }

                    splitIcons = IconUtil.Split(icon);
                }

                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                txtFileName.Text = $"{fileName}, #{m_iconIndex}, {splitIcons.Length} variations";

                // Update icons.

                Icon = icon;
                icon.Dispose();

                lvwIcons.BeginUpdate();
                ClearAllIcons();

                foreach (Icon i in splitIcons)
                {
                    var item = new IconListViewItem();
                    Size size = i.Size;
                    item.BitCount = IconUtil.GetBitCount(i);
                    item.Bitmap = IconUtil.ToBitmap(i);
                    item.ToolTipText = $"{size.Width}x{size.Height}, {item.BitCount} bits";
                    i.Dispose();

                    _ = lvwIcons.Items.Add(item);
                }

                lvwIcons.EndUpdate();

                btnSaveAsIco.Enabled = (m_iconExtractor != null);
            }
        }

        private void cbShowChecker_CheckedChanged(object sender, EventArgs e) => lvwIcons.BackgroundImage = cbShowChecker.Checked ? Properties.Resources.Checker : null;

        private void btnSaveAsIco_Click(object sender, EventArgs e)
        {
            if (saveIcoDialog.ShowDialog(this) == DialogResult.OK)

                using (FileStream fs = File.OpenWrite(saveIcoDialog.FileName))

                    m_iconExtractor.Save(m_iconIndex, fs);
        }

        private void btnSaveAsPng_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                int count = lvwIcons.Items.Count;

                for (int i = 0; i < count; ++i)
                {
                    var item = (IconListViewItem)lvwIcons.Items[i];

                    string fileName = $"{item.Bitmap.Width}x{item.Bitmap.Height}, {item.BitCount} bits.png";

                    fileName = Path.Combine(folderBrowserDialog.SelectedPath, fileName);

                    item.Bitmap.Save(fileName);
                }
            }
        }
    }
}
