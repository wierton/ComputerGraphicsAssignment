using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace ComputerGraphicsWork
{
    public class log
    {
        public void write(String s)
        {
            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate | FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(s);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
    public class CGUserGraphics
    {
        public IList<Point> pointsSet { get; } = new List<Point>();

        public void TagPoints()
        {

        }
    }

    public class CGUserGraphicsPoint : CGUserGraphics
    {
        private Point point;
        public CGUserGraphicsPoint(Point start)
        {
            point = start;
            pointsSet.Add(start);
        }
    }

    public class CGUserGraphicsLine : CGUserGraphics
    {
        private Point startPoint, endPoint;
        public CGUserGraphicsLine(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;

            if (end.X < start.X)
            {
                Point temp = start;
                start = end;
                end = temp;
            }

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;

            pointsSet.Add(start);
            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                int yInc = dy > 0 ? 1 : -1;
                int p = 2 * dy - dx;
                for (int i = 0; i < dx; i++)
                {
                    if (p > 0)
                    {
                        start.X ++;
                        start.Y += yInc;
                        pointsSet.Add(start);
                        int ddx = end.X - start.X;
                        int ddy = end.Y - start.Y;
                        p = p + 2 * (ddy - ddx);
                    }
                    else
                    {
                        start.X ++;
                        pointsSet.Add(start);
                        int ddy = end.Y - start.Y;
                        p = p + 2 * ddy;
                    }
                }
            }
            else
            {
                int xInc = dx > 0 ? 1 : -1;
                int p = 2 * dx - dy;
                for (int i = 0; i < Math.Abs(dy); i++)
                {
                    if (p > 0)
                    {
                        start.Y++;
                        start.X += xInc;
                        pointsSet.Add(start);
                        int ddx = end.X - start.X;
                        int ddy = end.Y - start.Y;
                        p = p + 2 * (ddx - ddy);
                    }
                    else
                    {
                        start.Y++;
                        pointsSet.Add(start);
                        int ddx = end.X - start.X;
                        p = p + 2 * ddx;
                    }
                }

            }
        }

    }
    public class CGUserGraphicsCircle : CGUserGraphics
    {

    }

    public class CGUserCanvas
    {
        private int canvasWidth, canvasHeight;
        public Bitmap bmp { get; }
        private int[,] refCount;
        public CGUserCanvas(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
            bmp = new Bitmap(width, height);
            refCount = new int[width, height];
        }

        private void SetPixel(Point pos)
        {
            bmp.SetPixel(pos.X, pos.Y, Color.Black);
            refCount[pos.X, pos.Y]++;
        }

        private void ClearPixel(Point pos)
        {
            refCount[pos.X, pos.Y]--;
            if(refCount[pos.X, pos.Y] <= 0)
                bmp.SetPixel(pos.X, pos.Y, Color.White);
        }

        private Point ClipPoint(Point pos)
        {
            if (pos.X < 0) pos.X = 0;
            if (pos.X >= canvasWidth) pos.X = canvasWidth - 1;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.Y >= canvasHeight) pos.Y = canvasHeight - 1;
            return pos;
        }


        public void SelectGraphics(CGUserGraphics userGraphics)
        {
            foreach (Point point in userGraphics.pointsSet)
            {
                SetPixel(point);
            }
        }

        public void ClearGraphics(CGUserGraphics userGraphics)
        {
            foreach (Point point in userGraphics.pointsSet)
            {
                ClearPixel(point);
            }
        }

        public void FindGraphics(Point pos)
        {

        }

    }
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.commandHolder = new System.Windows.Forms.ToolStrip();
            this.buttonDrawPoint = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawLine = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawCircle = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawEllipse = new System.Windows.Forms.ToolStripButton();
            this.buttonDrawNothing = new System.Windows.Forms.ToolStripButton();
            this.commandHolder.SuspendLayout();
            this.SuspendLayout();
            // 
            // commandHolder
            // 
            this.commandHolder.Dock = System.Windows.Forms.DockStyle.Left;
            this.commandHolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonDrawPoint,
            this.buttonDrawLine,
            this.buttonDrawCircle,
            this.buttonDrawEllipse,
            this.buttonDrawNothing});
            this.commandHolder.Location = new System.Drawing.Point(0, 0);
            this.commandHolder.Name = "commandHolder";
            this.commandHolder.Size = new System.Drawing.Size(33, 381);
            this.commandHolder.Stretch = true;
            this.commandHolder.TabIndex = 0;
            this.commandHolder.Text = "commandHolder";
            this.commandHolder.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.commandHolder_ItemClicked);
            // 
            // buttonDrawPoint
            // 
            this.buttonDrawPoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawPoint.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawPoint.Image")));
            this.buttonDrawPoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawPoint.Name = "buttonDrawPoint";
            this.buttonDrawPoint.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawPoint.Text = "buttonDrawPoint";
            this.buttonDrawPoint.Click += new System.EventHandler(this.buttonDrawPoint_Click);
            // 
            // buttonDrawLine
            // 
            this.buttonDrawLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawLine.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawLine.Image")));
            this.buttonDrawLine.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawLine.Name = "buttonDrawLine";
            this.buttonDrawLine.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawLine.Text = "buttonDrawLine";
            this.buttonDrawLine.ToolTipText = "buttonDrawLine";
            this.buttonDrawLine.Click += new System.EventHandler(this.buttonDrawLine_Click);
            // 
            // buttonDrawCircle
            // 
            this.buttonDrawCircle.AutoSize = false;
            this.buttonDrawCircle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawCircle.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawCircle.Image")));
            this.buttonDrawCircle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawCircle.Name = "buttonDrawCircle";
            this.buttonDrawCircle.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawCircle.Text = "buttonDrawCircle";
            this.buttonDrawCircle.Click += new System.EventHandler(this.buttonDrawCircle_Click);
            // 
            // buttonDrawEllipse
            // 
            this.buttonDrawEllipse.AutoSize = false;
            this.buttonDrawEllipse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawEllipse.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawEllipse.Image")));
            this.buttonDrawEllipse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawEllipse.Name = "buttonDrawEllipse";
            this.buttonDrawEllipse.Size = new System.Drawing.Size(32, 32);
            this.buttonDrawEllipse.Text = "buttonDrawEllipse";
            this.buttonDrawEllipse.ToolTipText = "buttonDrawEllipse";
            this.buttonDrawEllipse.Click += new System.EventHandler(this.buttonDrawEllipse_Click);
            // 
            // buttonDrawNothing
            // 
            this.buttonDrawNothing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDrawNothing.Image = ((System.Drawing.Image)(resources.GetObject("buttonDrawNothing.Image")));
            this.buttonDrawNothing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDrawNothing.Name = "buttonDrawNothing";
            this.buttonDrawNothing.Size = new System.Drawing.Size(30, 20);
            this.buttonDrawNothing.Text = "buttonDrawNothing";
            this.buttonDrawNothing.Click += new System.EventHandler(this.buttonDrawNothing_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(635, 381);
            this.Controls.Add(this.commandHolder);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_OverideLoad);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWindow_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseUp);
            this.commandHolder.ResumeLayout(false);
            this.commandHolder.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip commandHolder;
        private System.Windows.Forms.ToolStripButton buttonDrawLine;
        private System.Windows.Forms.ToolStripButton buttonDrawCircle;
        private System.Windows.Forms.ToolStripButton buttonDrawEllipse;
        private System.Windows.Forms.ToolStripButton buttonDrawNothing;
        private System.Windows.Forms.ToolStripButton buttonDrawPoint;
    }
}