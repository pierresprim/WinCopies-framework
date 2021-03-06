﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using WinCopies.GUI.Drawing;

namespace MultiIconTester
{
    public partial class FormIconBrowser : Form
    {
        #region Fields
        private string mFolder = string.Empty;
        private readonly MultiIcon mMultiIcon = new MultiIcon();
        #endregion

        #region Constructors
        public FormIconBrowser()
        {
            InitializeComponent();
            dlgSave.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
        }
        #endregion

        #region Events
        private void FormIconBrowser_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateFiles(Path.GetDirectoryName(Application.ExecutablePath));
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            dlgBrowse.SelectedPath = mFolder;
            dlgBrowse.ShowNewFolderButton = false;
            _ = dlgBrowse.ShowDialog(this);

            if (string.IsNullOrEmpty(dlgBrowse.SelectedPath))

                return;

            PopulateFiles(dlgBrowse.SelectedPath);
        }

        private void lbxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbxFiles.SelectedIndex == -1)

                    return;

                lbxIcons.Items.Clear();
                lbxImages.Items.Clear();
                pbxXORImage.Image = null;
                pbxANDImage.Image = null;
                pbxIcon.Image = null;
                lblWidthValue.Text = null;
                lblHeightValue.Text = null;
                lblColorDepthValue.Text = null;
                lblCompressionValue.Text = null;
                mMultiIcon.SelectedIndex = -1;
                mMultiIcon.Load(Path.Combine(mFolder, (string)lbxFiles.SelectedItem));

                foreach (SingleIcon singleIcon in mMultiIcon)

                    _ = lbxIcons.Items.Add(singleIcon.Name);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbxIcons_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbxIcons.SelectedIndex == -1)

                    return;

                lbxImages.Items.Clear();
                pbxXORImage.Image = null;
                pbxANDImage.Image = null;
                pbxIcon.Image = null;
                lblWidthValue.Text = null;
                lblHeightValue.Text = null;
                lblColorDepthValue.Text = null;
                lblCompressionValue.Text = null;
                mMultiIcon.SelectedIndex = lbxIcons.SelectedIndex;

                foreach (IconImage iconImage in mMultiIcon[lbxIcons.SelectedIndex])
                {
                    _ = lbxImages.Items.Add($"{iconImage.Size.Width}x{iconImage.Size.Height} {GetFriendlyBitDepth(iconImage.PixelFormat)}");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbxImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbxImages.SelectedIndex == -1)

                    return;

                IconImage iconImage = mMultiIcon[lbxIcons.SelectedIndex][lbxImages.SelectedIndex];
                pbxXORImage.Image = iconImage.Image;
                pbxANDImage.Image = iconImage.Mask;
                pbxIcon.Image = iconImage.Icon.ToBitmap();
                lblWidthValue.Text = iconImage.Size.Width.ToString();
                lblHeightValue.Text = iconImage.Size.Height.ToString();
                lblColorDepthValue.Text = iconImage.PixelFormat.ToString();
                lblCompressionValue.Text = iconImage.IconImageFormat.ToString();
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            dlgSave.DefaultExt = "icl";
            dlgSave.Filter = "Windows Icon File (*.ico)|*.ico|Icon Library File (*.icl)|*.icl|DLL Library File " +
                "(*.dll)|*.dll|PNG Image File (*.png)|*.png|BMP Windows File (*.bmp)|*.bmp";

            if (dlgSave.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = dlgSave.FileName.ToLower();

                if (fileName.EndsWith(".ico") && mMultiIcon.SelectedIndex != -1)

                    mMultiIcon.Save(dlgSave.FileName, MultiIconFormat.ICO);

                else if (fileName.EndsWith(".png") && mMultiIcon.SelectedIndex != -1 && lbxImages.SelectedIndex != -1)

                    mMultiIcon[mMultiIcon.SelectedIndex][lbxImages.SelectedIndex].Transparent.Save(dlgSave.FileName, ImageFormat.Png);

                else if (fileName.EndsWith(".bmp") && mMultiIcon.SelectedIndex != -1 && lbxImages.SelectedIndex != -1)

                    mMultiIcon[mMultiIcon.SelectedIndex][lbxImages.SelectedIndex].Transparent.Save(dlgSave.FileName, ImageFormat.Bmp);

                else if (fileName.EndsWith(".icl"))

                    mMultiIcon.Save(dlgSave.FileName, MultiIconFormat.ICL);

                else if (fileName.ToLower().EndsWith(".dll"))

                    mMultiIcon.Save(dlgSave.FileName, MultiIconFormat.DLL);
            }
        }

        private void btnExportAND_Click(object sender, EventArgs e) => ExportBitmap(mMultiIcon[mMultiIcon.SelectedIndex][lbxImages.SelectedIndex].Mask);

        private void btnExportBMP_Click(object sender, EventArgs e) => ExportBitmap(mMultiIcon[mMultiIcon.SelectedIndex][lbxImages.SelectedIndex].Image);

        private void ExportBitmap(in Bitmap bmp)
        {
            dlgSave.DefaultExt = "bmp";
            dlgSave.Filter = "Windows Bitmap File (*.bmp)|*.bmp";

            if (mMultiIcon.SelectedIndex == -1 || lbxImages.SelectedIndex == -1)

                return;

            if (dlgSave.ShowDialog(this) == DialogResult.OK)

                bmp.Save(dlgSave.FileName, ImageFormat.Bmp);
        }
        #endregion

        #region Methods
        private void PopulateFiles(string folder)
        {
            var validFiles = new LinkedList<string>();

            mFolder = folder;

            //            string[] files = Directory.GetFiles(folder, "*.ico, *.icl, *.dll, *.exe, *.ocx, *.cpl, *.src");
            string[] files = Directory.GetFiles(folder);

            foreach (string file in files)
            {
                switch (Path.GetExtension(file).ToLower())
                {
                    case ".ico":
                    case ".icl":
                    case ".dll":
                    case ".exe":
                    case ".ocx":
                    case ".cpl":
                    case ".src":
                        _ = validFiles.AddLast(Path.GetFileName(file));
                        break;
                }
            }
            lbxFiles.Items.Clear();
            lbxFiles.Items.AddRange(validFiles.ToArray());
        }

        private string GetFriendlyBitDepth(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return "1-bit B/W";
                case PixelFormat.Format24bppRgb:
                    return "24-bit True Colors";
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppRgb:
                    return "32-bit Alpha Channel";
                case PixelFormat.Format8bppIndexed:
                    return "8-bit 256 Colors";
                case PixelFormat.Format4bppIndexed:
                    return "4-bit 16 Colors";
            }
            return "Unknown";
        }
        #endregion

        #region Override
        protected override void OnClosing(CancelEventArgs e)
        {
            _ = new Form2().ShowDialog(this);
            base.OnClosing(e);
        }
        #endregion

        #region Test methods
        private void btnCreate_Click(object sender, EventArgs e)
        {
            var multiIcon = new MultiIcon();

            for (int i = 1; i == 7; i++)

                multiIcon.Add($"Icon {i}").Load(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), $"Icon{i}.ico"));

            multiIcon.Save(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "library.icl"), MultiIconFormat.ICL);
        }
        #endregion

        #region Internal Testing
        private void button1_Click(object sender, EventArgs e)
        {
            //MultiIcon mIcon = new MultiIcon();
            //SingleIcon sIcon = mIcon.Add("Icon1");
            //sIcon.CreateFrom("c:\\Pampero.png", IconOutputFormat.FromWin95);
            //sIcon.CreateFrom("C:\\z3.png", IconOutputFormat.FromWin31);
            //sIcon[0].Transparent.Save("c:\\zzzz1.png", ImageFormat.Png);
            //sIcon[1].Transparent.Save("c:\\zzzz2.png", ImageFormat.Png);
            //sIcon[2].Transparent.Save("c:\\zzzz3.png", ImageFormat.Png);
            //sIcon[3].Transparent.Save("c:\\zzzz4.png", ImageFormat.Png);
            //sIcon[4].Transparent.Save("c:\\zzzz5.png", ImageFormat.Png);

            //sIcon.Save("c:\\zzzz1.ico");
            //Bitmap bmp1 = (Bitmap) Bitmap.FromFile("C:\\test3.png");
            //bmp1.Save("c:\\test31.png", ImageFormat.Png);
            //Bitmap bmp2 = new Bitmap(bmp1, 128, 138);
            //bmp2.Save("c:\\test32.png", ImageFormat.Png);

            //IColorQuantizer colorReduction = new EuclideanQuantizer(new OctreeQuantizer(), new FloydSteinbergDithering());
            ////Bitmap bmp = (Bitmap) pbxIcon.Image;
            //Bitmap bmp = (Bitmap) Bitmap.FromFile("c:\\MyKids.png");
            ////DateTime dt1 = DateTime.Now;
            //Bitmap newBmp = colorReduction.Convert(bmp, PixelFormat.Format8bppIndexed);
            //newBmp.Save("c:\\MyKids8.png", ImageFormat.Png);

            //newBmp = colorReduction.Convert(bmp, PixelFormat.Format4bppIndexed);
            //newBmp.Save("c:\\MyKids4.png", ImageFormat.Png);

            //newBmp = colorReduction.Convert(bmp, PixelFormat.Format1bppIndexed);
            //newBmp.Save("c:\\MyKids1.png", ImageFormat.Png);

            ////newBmp = fs.Convert(bmp);
            ////newBmp = fs.Convert(bmp);
            //DateTime dt2 = DateTime.Now;
            //Console.WriteLine("Convert:" + ((TimeSpan) dt2.Subtract(dt1)).TotalMilliseconds);
            //pbxIcon.Image = newBmp;
            //newBmp.Save("c:\\zzz.bmp", ImageFormat.Bmp);
        }
        #endregion
    }
}