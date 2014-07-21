using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadCheckCode
{
    public partial class Form1 : Form
    {
        CheckCodeReader ccReader = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Image img = Image.FromFile(@"C:\Users\lbp\Pictures\1621.jpg");
            this.pictureBox1.Image = img;
            ccReader = new CheckCodeReader(img);
            this.trackBar1.Value = ccReader.Threshold;
            this.label1.Text = this.trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.pictureBox2.Image = ccReader.BinaryZaTion();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ccReader.Threshold = this.trackBar1.Value;
            this.label1.Text = this.trackBar1.Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var lstImg = ccReader.CutImg();
            foreach (var item in lstImg)
            {
                this.textBox1.Text += item.ToString();
            }

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox)
            {
                var ctlPic = sender as PictureBox;
                this.textBox1.Text += ccReader.PixlPercent(new Bitmap(ctlPic.Image));
            }
        }
    }
}
