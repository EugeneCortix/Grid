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
        public List<Element> elements;
        public Graphics g;
        Bitmap bmp = new Bitmap(480, 380);

        public int xval, yval;
        public int xcrsh = 4; //X crushing for every element
        public int ycrsh = 4; //Y crushing for every element

        public Form1()
        {
            fieldPoints = new List<Point>();
            addx = new List<int>();
            addy = new List<int>();
            node = new List<Point>();
            elements = new List<Element>();
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
            bmp.SetPixel(dot[0].X, dot[0].Y, Color.FromArgb(255, 0, 0, 0));
            for (int i = 1; i < int.Parse(datas[0]); i++)
            {
                g.DrawLine(fieldPen, dot[i-1], dot[i]);
                bmp.SetPixel(dot[i].X, dot[i].Y, Color.FromArgb(255, 0, 0, 0));
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
            Color c;
            Color border = Color.FromArgb(255,0, 0, 0);
            for (int i = 0; i < xval; i++)
            {
                for (int j = 0; j < yval; j++)
                {
                    c = bmp.GetPixel(addx[i], addy[j]);
                    // Check border
                    if (c == border)
                    {if((bmp.GetPixel(addx[i], addy[j] -5) !=border || bmp.GetPixel(addx[i], addy[j] + 5) != border) &&
                          (bmp.GetPixel(addx[i] -5, addy[j]) != border || bmp.GetPixel(addx[i]+5, addy[j]) != border))
                        node.Add(new Point(addx[i], addy[j]));
                    }
                }
            }
            /*g.DrawLine(addGridPen, y0, cross);
                    g.DrawLine(addGridPen, cross, x0);*/
                    // Side nodes
                    for (int i = 0; i<addx.Count; i++)
            {
                if(bmp.GetPixel(addx[i], addy[0]) == border)
                {
                    Point p = new Point(addx[i], addy[0]);
                    if (!(node.Contains(p))) node.Add(p);
                }
            }
                    // Border nodes
            int prev = 0;
            int thaty = 0;
            for (int j = 0; j < node.Count; j++)
                    if (node[j].X == addx[0]) prev++;

            for (int i = 1; i < addx.Count; i++)
            { int cur = 0;
                for (int j = 0; j < node.Count; j++)
                    if (node[j].X == addx[i]) cur++;
                if (cur > prev)
                {
                    
                    for (int l = 0; l < addy.Count; l++)
                    {
                        Point pprev = new Point(addx[i - 1], addy[l]);
                        Point pcur = new Point(addx[i], addy[l]);
                        if(node.Contains(pcur) && !(node.Contains(pprev)))
                        {
                            thaty = addy[l];
                        }
                    }
                    prev = cur;
                }
            }
            // Add that nodes
            for(int i = 0; i< addx.Count; i++)
                if (!(node.Contains(new Point(addx[i], thaty)))) node.Add(new Point(addx[i], thaty));
            // Make the elements List
            cross();
            // Sort elements to know where's which one
            Comparer nc = new Comparer();
            elements.Sort(nc);
            // Draw elements
            drel();
            pictureBox1.Image = bmp;
            button3.Enabled = true;
        }
        private void drel()
        {
            Pen addGridPen;
            addGridPen = new Pen(Brushes.Gray, 1);
            addGridPen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
            for (int i = 0; i < elements.Count; i++)
            {
                g.DrawLine(addGridPen, elements[i].p1, elements[i].p2);
                g.DrawLine(addGridPen, elements[i].p2, elements[i].p4);
                g.DrawLine(addGridPen, elements[i].p4, elements[i].p3);
                g.DrawLine(addGridPen, elements[i].p3, elements[i].p1);
            }
        }
        private void cross()
        {
            for(int i = 0; i< node.Count - 1; i++)
            {
                Point point1 = new Point();
                Point point2 = new Point();
                Point point3 = new Point();
                Point point4 = new Point();
                // Take a point
                point1 = node[i];
                // Comparing characteristics
                int dxmin = addx[xval - 1] - addx[0];
                int dymin = addy[0] - addy[yval - 1];
                double diamin = Math.Pow(Math.Pow(dxmin, 2) + Math.Pow(dymin, 2),
                                                                             0.5);

                // Look for other points
                for (int j = 0; j < node.Count; j++)
                {
                    Point pt = new Point();
                   
                    // Not the same Point
                    if(j != i)
                    {
                        pt = node[j];
                        // Vertical closest which is upper
                        if(pt.X == point1.X)
                        {
                            int dy = point1.Y - pt.Y;
                            if(dy>0 && dy < dymin) { dymin = dy; point2 = pt; }
                        }
                        // Horizontal closest which is righter
                        if (pt.Y == point1.Y)
                        {
                            int dx = pt.X - point1.X;
                            if (dx > 0 && dx < dxmin) { dxmin = dx; point3 = pt; }
                        }
                        // Diagonal closest which is righter and upper
                        if(pt.Y != point1.Y && pt.X != point1.X)
                        {
                            int dx = pt.X - point1.X;
                            int dy = point1.Y - pt.Y;
                            if (dy > 0 && dx > 0)
                            {
                                double dia = Math.Pow(Math.Pow(dx, 2) + Math.Pow(dy, 2),
                                                                                     0.5);
                                if(dia < diamin) { diamin = dia; point4 = pt; }
                            }
                        }
                    }
                }
                // Make an element
                //Without zero-points
                if (point2.X != 0 && point2.Y != 0 && point3.X != 0 && point3.Y != 0 && point4.X != 0 && point4.Y != 0)
                elements.Add(new Element() { p1 = point1, p2 = point2, p3 = point3, p4 = point4 });
            }
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
            for (int i = 0; i < yval; i++)
            {
                addy.Add(int.Parse(y[i]));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            List<int> isfilled = new List<int>();
            string filename = openFileDialog1.FileName;
            string[] fill = System.IO.File.ReadAllLines(filename);
            Pen gridPen = new Pen(Brushes.Red, 1);
           /* for (int i = 0; i < fill.Length; i++)
            {
                isfilled.Add(Convert.ToInt(fill[i]));
            }*/
            for (int i = 0; i < elements.Count; i++)
            {
                if (fill[i] == "1")
                {
                    // Horizontal crushing
                    for (int j =0; j<xcrsh; j++)
                    {
                        Point pu = new Point(); // Up point of the seg
                        Point pd = new Point(); // Down point
                        int ddx = Convert.ToInt32(Math.Round(Convert.ToDouble(
                            (elements[i].p3.X - elements[i].p1.X)/xcrsh*j)));
                        int dux = Convert.ToInt32(Math.Round(Convert.ToDouble(
                            (elements[i].p4.X - elements[i].p2.X) / xcrsh * j))); 
                        pd.X = elements[i].p1.X+ddx;
                        pu.X = elements[i].p2.X+dux;

                        int duy = 0;
                        if (elements[i].p4.Y - elements[i].p2.Y != 0)
                        {
                            duy= Convert.ToInt32(Math.Round(Convert.ToDouble(
                            (pu.X - elements[i].p2.X) * (elements[i].p4.Y - elements[i].p2.Y) / (elements[i].p4.X - elements[i].p2.X))));
                        }
                        int ddy = 0;
                        if(elements[i].p3.Y - elements[i].p1.Y != 0)
                        {
                            ddy = Convert.ToInt32(Math.Round(Convert.ToDouble(
                             (pd.X - elements[i].p1.X) * (elements[i].p3.Y - elements[i].p1.Y) / (elements[i].p3.X - elements[i].p1.X))));
                        }
                        pd.Y = elements[i].p1.Y + ddy;
                        pu.Y = elements[i].p2.Y + duy;
                        g.DrawLine(gridPen, pu, pd);

                    }
                    // Vertical crushing
                    for (int j = 0; j < ycrsh; j++)
                    {
                        Point pl = new Point(); // Left point
                        Point pr = new Point(); // Right point
                        int dl = Convert.ToInt32(Math.Round(Convert.ToDouble(
                            (elements[i].p1.Y - elements[i].p2.Y)/ycrsh*j)));
                        int dr = Convert.ToInt32(Math.Round(Convert.ToDouble(
                            (elements[i].p3.Y - elements[i].p4.Y) / ycrsh * j)));
                        pl = elements[i].p2;
                        pr = elements[i].p4;
                        pl.Y += dl;
                        pr.Y += dr;
                        g.DrawLine(gridPen, pl, pr);

                    }
                }
                pictureBox1.Image = bmp;
                saveNode();
                saveElements();
            }
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
            ComparerP nc = new ComparerP();
            node.Sort(nc);
            string nodes = "№\tX\tY\n";
            string filename = "..\\...\\...\\node.txt";
            for(int i = 0; i < node.Count; i++)
            {
                nodes +=i.ToString()+'\t'+ node[i].X.ToString() + '\t' + node[i].Y.ToString() + '\n';
            }
            System.IO.File.WriteAllText(filename, nodes);
        }

        private void saveElements()
        {
            string el="";
            string filename = "..\\...\\...\\elements.txt";
            for (int i = 0; i < elements.Count; i++)
            {
                el += i.ToString()+'\t' +'\n';
                el+=PrintNum(elements[i].p1) + '\t'+
                   PrintNum(elements[i].p2) + '\t' +
                   PrintNum(elements[i].p3) + '\t'+
                   PrintNum(elements[i].p4) + '\n' +'\n';
            }
            System.IO.File.WriteAllText(filename, el);
        }
        private string PrintNum(Point p)
        {
            int n = -1;
            for (int i = 0; i < node.Count; i++)
            {
                if (node[i] == p) n = i;
            }
            return n.ToString();
        }
    }
    // The struct of an element
    /*
     p2-----------p4
     |             |
     |             |
     |             |
     |             |
     p1-----------p3
         
         */
    public class Element
    {
        public Point p1 { get; set; }
        public Point p2 { get; set; }
        public Point p3 { get; set; }
        public Point p4 { get; set; }

    }
    // Comparation for the sorting func
    public class Comparer : IComparer<Element>
    {
        public int Compare(Element x, Element y)
        {
            Point px = new Point();
            px = x.p1;
            Point py = new Point();
            py = y.p1;
            // X are equal
            if (px.X == py.X)
            {
                return px.Y.CompareTo(py.Y);
            }
            else
            {
                return px.X.CompareTo(py.X);
            }
        }
    }

    public class ComparerP : IComparer<Point>
    {
        public int Compare(Point p, Point pp)
        {
            if (pp.X == p.X) return p.Y.CompareTo(pp.Y);
            else return p.X.CompareTo(pp.X);
        }
    }


}
