using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ComputerGraphicsWork
{
    enum CGGraphicsType
    {
        CGTypeNothing = 0,
        CGTypePoint = 1,
        CGTypeLine = 2,
        CGTypeCircle = 3,
        CGTypeEllipse = 4,
    };

    enum CGMouseState
    {
        CGMouseStateDown = 0,
        CGMouseStateUp = 1,
        CGMouseStateTwiceDown = 2,
        CGMouseStateTwiceUp = 3,
    };

    public partial class MainWindow : Form
    {
        private CGGraphicsType ghsType;
        private CGMouseState mouseState;
        private Graphics ghs;
        private Point oldPos, curPos, downPos;
        private Pen ghsPen = new Pen(Color.Black, 1);
        private Pen rawPen = new Pen(Color.White, 1);
        private bool canDrawGraphics = false;
        private bool canClearGraphics = false;
        private CGUserCanvas userCanvas;
        private CGUserGraphics curUserGraphics;
        private IList<CGUserGraphics> userGraphicsSet = new List<CGUserGraphics>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void MainWindow_OverideLoad(object sender, EventArgs e)
        {
            ghs = this.CreateGraphics();
            userCanvas = new CGUserCanvas(this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        private void ResumeOldStripButton(CGGraphicsType gt)
        {
            switch (gt)
            {
                case CGGraphicsType.CGTypePoint:
                    this.buttonDrawPoint.Checked = false;
                    break;
                case CGGraphicsType.CGTypeLine:
                    this.buttonDrawLine.Checked = false;
                    break;
                case CGGraphicsType.CGTypeCircle:
                    this.buttonDrawCircle.Checked = false;
                    break;
                case CGGraphicsType.CGTypeEllipse:
                    this.buttonDrawEllipse.Checked = false;
                    break;
            }
        }

        private void buttonDrawPoint_Click(object sender, EventArgs e)
        {
            if (ghsType == CGGraphicsType.CGTypePoint)
            {
                this.buttonDrawPoint.Checked = false;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawPoint.Checked = true;
                ghsType = CGGraphicsType.CGTypePoint;
            }
        }

        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            if(ghsType == CGGraphicsType.CGTypeLine)
            {
                this.buttonDrawLine.Checked = false;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawLine.Checked = true;
                ghsType = CGGraphicsType.CGTypeLine;
            }
        }

        private void buttonDrawCircle_Click(object sender, EventArgs e)
        {
            if (ghsType == CGGraphicsType.CGTypeCircle)
            {
                this.buttonDrawCircle.Checked = false;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawCircle.Checked = true;
                ghsType = CGGraphicsType.CGTypeCircle;
            }
        }

        private void buttonDrawEllipse_Click(object sender, EventArgs e)
        {
            if (ghsType == CGGraphicsType.CGTypeEllipse)
            {
                this.buttonDrawEllipse.Checked = false;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawEllipse.Checked = true;
                ghsType = CGGraphicsType.CGTypeEllipse;
            }
        }

        private void buttonDrawNothing_Click(object sender, EventArgs e)
        {
            if (ghsType == CGGraphicsType.CGTypeNothing)
            {
                this.buttonDrawNothing.Checked = false;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawNothing.Checked = true;
                ghsType = CGGraphicsType.CGTypeNothing;
            }
        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouseState = CGMouseState.CGMouseStateUp;
            canDrawGraphics = false;
            canClearGraphics = false;
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            mouseState = CGMouseState.CGMouseStateDown;
            canDrawGraphics = true;
            canClearGraphics = false;
            downPos.X = e.X;
            downPos.Y = e.Y;
        }

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            ghs.DrawImage(userCanvas.bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void wrapDrawCircle(Pen pen, Point center, Point edge)
        {
            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;
            double cx = center.X;
            double cy = center.Y;
            double r = Math.Sqrt((dx * dx + dy * dy));
            ghs.DrawEllipse(pen, new RectangleF((float)(cx - r), (float)(cy - r), (float)(2 * r), (float)(2 * r)));

        }


        private void wrapDrawEllipse(Pen pen, Point center, Point edge)
        {
            int dx = Math.Abs(edge.X - center.X);
            int dy = Math.Abs(edge.Y - center.Y);
            int cx = center.X;
            int cy = center.Y;
            double e = Math.Sqrt((dx * dx + dy * dy) / 2);
            ghs.DrawEllipse(pen, new RectangleF((float)(cx - dx), (float)(cy - dy), (float)(2 * dx), (float)(2 * dy)));

        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            oldPos = curPos;
            curPos.X = e.X;
            curPos.Y = e.Y;

            if (!canDrawGraphics || mouseState == CGMouseState.CGMouseStateUp)
            {
                return;
            }

            switch (ghsType)
            {
                case CGGraphicsType.CGTypePoint:
                    
                    // userCanvas.SetPixel(curPos.X, curPos.Y, Color.Black);
                    // ghs.DrawImage(userCanvas, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
                    break;
                case CGGraphicsType.CGTypeLine:
                    if(canClearGraphics)
                    {
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsLine line = new CGUserGraphicsLine(downPos, curPos);
                    userCanvas.SelectGraphics(line);
                    ghs.DrawImage(userCanvas.bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
                    curUserGraphics = line;
                    // ghs.DrawLine(rawPen, downPos, oldPos);
                    // ghs.DrawLine(ghsPen, downPos, curPos);
                    break;
                case CGGraphicsType.CGTypeCircle:
                    wrapDrawCircle(rawPen, downPos, oldPos);
                    wrapDrawCircle(ghsPen, downPos, curPos);
                    break;
                case CGGraphicsType.CGTypeEllipse:
                    wrapDrawEllipse(rawPen, downPos, oldPos);
                    wrapDrawEllipse(ghsPen, downPos, curPos);
                    break;
            }

            canClearGraphics = true;
        }
    }
}
