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
        public Form1()
        {
            fieldPoints = new List<Point>();
            /*fieldPoints.Add(new Point(30, 100));
            fieldPoints.Add(new Point(3,160));
            fieldPoints.Add(new Point(3, 300));
            fieldPoints.Add(new Point(340, 300));
            fieldPoints.Add(new Point(340,120));
            fieldPoints.Add(new Point(240, 150));
            fieldPoints.Add(new Point(240, 180));
            fieldPoints.Add(new Point(100, 180));
            fieldPoints.Add(new Point(100, 100));
            fieldPoints.Add(new Point(30, 100));*/

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drawField(fieldPoints);
        }

        // Drawning func
        private void drawField(List<Point> dot)
        {
            Graphics g = pictureBox1.CreateGraphics();
            Pen fieldPen;
            fieldPen = new Pen(Brushes.Black, 4);
            g.Clear(Color.White);
            string[] datas = getField();
            for (int i = 1; i < int.Parse(datas[0]); i++)
            {
                g.DrawLine(fieldPen, dot[i-1], dot[i]);
            }

        }

        //Reading func
        private string[] getField()
        {
            string[] datas;
            string[] empty = { };
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
        
    }
 

}
