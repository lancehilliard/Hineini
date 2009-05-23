using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Hineini.Properties;

namespace Hineini {
    public partial class TagForm : Form {
        public event EventHandler FormClosed;
        public TagForm() {
            InitializeComponent();
            Resize += TagForm_Resize;
            ShowTag();
        }

        private void ShowTag() {
            tagPictureBox.Image = null;
            bool heightIsBigger = Height > Width;
            tagPictureBox.Height = heightIsBigger ? Width : Height;
            tagPictureBox.Width = tagPictureBox.Height;
            Image hineiniTag = GetSizedTag(tagPictureBox.Height);
            tagPictureBox.Image = hineiniTag;
        }

        private static Image GetSizedTag(int size) {
            Image originalTag = Resources.Hineini_Tag;
            Bitmap sizedBitmap = new Bitmap(size, size);
            Graphics graphics = Graphics.FromImage(sizedBitmap);
            Rectangle destinationRectangle = new Rectangle(0, 0, size, size);
            Rectangle sourceRectangle = new Rectangle(0, 0, Resources.Hineini_Tag.Width, Resources.Hineini_Tag.Height);
            graphics.DrawImage(originalTag, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            return sizedBitmap;
        }

        void TagForm_Resize(object sender, EventArgs e) {
            ShowTag();
        }

        private void backMenuItem_Click(object sender, EventArgs e) {
            FormClosed(sender, e);
        }
    }
}