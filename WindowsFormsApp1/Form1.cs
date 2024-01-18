using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private enum DrawingShape { Line, Rectangle, Ellipse }
        private DrawingShape currentShape = DrawingShape.Line;

        private Point startPoint;
        private Color currentColor;
        private List<Shape> shapes = new List<Shape>();
        private Bitmap offScreenBitmap;
        private Graphics offScreenGraphics;
        public Form1()
        {
            InitializeComponent();
            currentColor = Color.Red;
            InitializeOffScreenBuffer();
        }
        private void InitializeOffScreenBuffer()
        {
            offScreenBitmap = new Bitmap(panel1.Width, panel1.Height);
            offScreenGraphics = Graphics.FromImage(offScreenBitmap);
        }
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            // Save the drawn shape to the list
            shapes.Add(CreateShape(startPoint, e.Location));

            // Redraw all shapes on the off-screen buffer
            offScreenGraphics.Clear(panel1.BackColor);
            Pen pen = new Pen(currentColor);
            foreach (Shape shape in shapes)
            {
                shape.Draw(offScreenGraphics, pen);
            }

            panel1.Invalidate(); // Trigger paint event to copy the off-screen buffer to the screen
        }
        private void buttonLine_Click(object sender, EventArgs e)
        {
            currentShape = DrawingShape.Line;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            UpdateColor();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentShape = DrawingShape.Rectangle;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentShape = DrawingShape.Ellipse;
        }

        private void UpdateColor()
        {
            int red = Math.Min(255, Math.Max(0, trackBar1.Value));
            int green = Math.Min(255, Math.Max(0, trackBar2.Value));
            int blue = Math.Min(255, Math.Max(0, trackBar3.Value));

            Console.WriteLine($"Updating color: R={red}, G={green}, B={blue}");

            currentColor = Color.FromArgb(red, green, blue);
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            UpdateColor();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Draw on the off-screen buffer
                offScreenGraphics.Clear(panel1.BackColor);

                Pen previewPen = new Pen(currentColor);

                // Draw all shapes
                foreach (Shape shape in shapes)
                {
                    shape.Draw(offScreenGraphics, previewPen);
                }

                // Draw the preview shape
                switch (currentShape)
                {
                    case DrawingShape.Line:
                        offScreenGraphics.DrawLine(previewPen, startPoint, e.Location);
                        break;
                    case DrawingShape.Rectangle:
                        offScreenGraphics.DrawRectangle(previewPen, Math.Min(startPoint.X, e.X), Math.Min(startPoint.Y, e.Y), Math.Abs(e.X - startPoint.X), Math.Abs(e.Y - startPoint.Y));
                        break;
                    case DrawingShape.Ellipse:
                        offScreenGraphics.DrawEllipse(previewPen, Math.Min(startPoint.X, e.X), Math.Min(startPoint.Y, e.Y), Math.Abs(e.X - startPoint.X), Math.Abs(e.Y - startPoint.Y));
                        break;
                }

                panel1.Invalidate(); // Trigger paint event to redraw the preview
            }
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            UpdateColor();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(offScreenBitmap, 0, 0);
        }

        private abstract class Shape
        {
            public abstract void Draw(Graphics g, Pen pen);
        }

        // Class for a line
        private class Line : Shape
        {
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }

            public override void Draw(Graphics g, Pen pen)
            {
                g.DrawLine(pen, StartPoint, EndPoint);
            }
        }

        // Class for a rectangle
        private class RectangleShape : Shape
        {
            public Rectangle Bounds { get; set; }

            public override void Draw(Graphics g, Pen pen)
            {
                g.DrawRectangle(pen, Bounds);
            }
        }

        // Class for an ellipse
        private class Ellipse : Shape
        {
            public Rectangle Bounds { get; set; }

            public override void Draw(Graphics g, Pen pen)
            {
                g.DrawEllipse(pen, Bounds);
            }
        }

        // Create a shape based on the current shape type
        private Shape CreateShape(Point start, Point end)
        {
            switch (currentShape)
            {
                case DrawingShape.Line:
                    return new Line { StartPoint = start, EndPoint = end };
                case DrawingShape.Rectangle:
                    return new RectangleShape { Bounds = new Rectangle(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) };
                case DrawingShape.Ellipse:
                    return new Ellipse { Bounds = new Rectangle(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y), Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)) };
                default:
                    return null;
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {

        }
    }
}
