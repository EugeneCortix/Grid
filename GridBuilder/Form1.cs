/* Lab 1
 * Var 21
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*using System.Windows.Media;
using System.Windows.Point;
using Pen = System.Drawing.Pen;
using Color = System.Drawing.Color;
using Brushes = System.Drawing.Brushes;*/

namespace GridBuilder
{
    public partial class Form1 : Form
    {
        public List<Point> fieldPoints;
        public List<Point> node;
        public List<int> addx;
        public List<int> addy;
        public List<float> gridx;
        public List<float> gridy;
        public Graphics g;
        Bitmap bmp = new Bitmap(480, 380);

        public int xval, yval;
        public int xcrsh = 9; //X crushing
        public int ycrsh = 9; //Y crushing

        public Form1()
        {
            fieldPoints = new List<Point>();
            addx = new List<int>();
            addy = new List<int>();
            node = new List<Point>();
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawField(fieldPoints);
        }

        // Drawning func
        private void drawField(List<Point> dot)
        {
            
            g = Graphics.FromImage(bmp);
            Pen fieldPen;
            fieldPen = new Pen(Brushes.Black, 4);
            g.Clear(Color.White);
            string[] datas = getField();
            if (datas == null) return;
            for (int i = 1; i < int.Parse(datas[0]); i++)
            {
                g.DrawLine(fieldPen, dot[i-1], dot[i]);
            }
            pictureBox1.Image = bmp;
            button2.Enabled = true;
        }

        //Reading func
        private string[] getField()
        {
            string[] datas;
            string[] empty = null;
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return empty;
            string filename = openFileDialog1.FileName;
            string fileText = System.IO.File.ReadAllText(filename);
            
            datas = fileText.Split('\t', '\n');
            for(int i = 1; i < int.Parse(datas[0]) + 1; i++)
            {
                string[] xy = datas[i].Split(' ');
                fieldPoints.Add(new Point(int.Parse(xy[0]), int.Parse(xy[1])));
            }
            return datas;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pen addGridPen;
            addGridPen = new Pen(Brushes.Gray, 1);
            addGridPen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
            getAddGrid();
            for(int i = 0; i < yval; i++)
            {
                for (int j = 0; j < xval; j++)
                {
                    Point y0 = new Point(addx[0], addy[j]);
                    Point x0 = new Point(addx[i], addy[0]);
                    Point cross = new Point(addx[i], addy[j]);
                    g.DrawLine(addGridPen, y0, cross);
                    g.DrawLine(addGridPen, cross, x0);
                } 
            }
            pictureBox1.Image = bmp;
            button3.Enabled = true;
        }

        private void getAddGrid()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            string[] vals = System.IO.File.ReadAllLines(filename);
            xval = int.Parse(vals[0]);
            yval = int.Parse(vals[2]);
            string[] x = vals[1].Split('\t');
            for (int i = 0; i < xval; i++)
            {
                addx.Add(int.Parse(x[i]));
            }
            string[] y = vals[3].Split('\t');
            for (int i = 0; i < xval; i++)
            {
                addy.Add(int.Parse(y[i]));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Vertical
            Pen gridPen = new Pen(Brushes.Red, 1);
                for(int j = 0; j<=xcrsh; j++)
                {
                //int xt = addx[i-1] + (addx[i] - addx[i-1]) / xcrsh * j;
                double xtd = 1.5 + addx[0] + (addx[xval - 1] - addx[0] - 4.5 ) / xcrsh * j;
                int xt = Convert.ToInt32(Math.Round(xtd));
                Point y0 = new Point(xt, addy[0]);
                    int yt = findy(xt);
                    Point ym = new Point(xt, yt);
                    g.DrawLine(gridPen, y0, ym);
                    node.Add(y0);
                    node.Add(ym);
                }

            // Horizontal

                for (int j = 0; j <= ycrsh; j++)
                {
               // int yt = addy[yval-1]+(addy[0] - addy[yval - 1]) * j / ycrsh;
                double ytd = 1.5 + addy[yval-1]+(addy[0] - addy[yval - 1] -4.5) * j / ycrsh;
                int yt = Convert.ToInt32(Math.Round(ytd));
                    drawHor(yt, gridPen);
                }

            pictureBox1.Image = bmp;
            saveNode();
        }

        private void drawHor(int y, Pen pen)
        {
            // Colors for the search 
            Color c = Color.FromArgb(255, 0, 0);
            Color border = Color.FromArgb(0, 0, 0);

            List<int> xp = new List<int>();
            // Width of the paintBox is 480

            xp.Add(0);
            while( c != border)
            {
                c = bmp.GetPixel(xp[0], y);
                xp[0]++;
            }
            
            for( int i = 1; i < xval; i++)
            {
                int xwd = addx[i] - 3;
                int id = 0;
                for (int h = 1; h < 7; h++)
                {
                    c = bmp.GetPixel(xwd+h, y);
                    if (c == border) id = 1;
                }
                if (id == 1) xp.Add(addx[i]);
            }

            for (int m = 1; m < xp.Count; m += 2)
            {
                Point p2 = new Point(xp[m], y);
                Point p1 = new Point(xp[m - 1], y);
                g.DrawLine(pen, p1, p2);
                node.Add(p1);
                node.Add(p2);
            }
        }

        private int findy(int x)
        {
            Color c = Color.FromArgb(255, 0, 0);
            Color admesh = Color.FromArgb(128, 128, 128);
            Color border = Color.FromArgb(0, 0, 0);
            int res = addy[0] - 5;
            while (c != border) 
            {
                c = bmp.GetPixel(x, res);
                res--;
            }
            return res;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            string coord = "";
            coord+= e.X.ToString();
            coord+= ", ";
            coord+= e.Y.ToString();
            label1.Text = coord;
        }
        // Makes nodes file
        private void saveNode()
        {
            string nodes = "X\tY\n";
            string filename = "..\\...\\...\\node.txt";
            for(int i = 0; i < node.Count; i++)
            {
                nodes += node[i].X.ToString() + '\t' + node[i].Y.ToString() + '\n';
            }
            System.IO.File.WriteAllText(filename, nodes);
        }
    }
 

}
