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
    enum CGButtonType
    {
        CGTypeNothing,
        CGTypeSave,
        CGTypeMove,
        CGTypeAdjust,
        CGTypePoint,
        CGTypeLine,
        CGTypeCircle,
        CGTypeEllipse,
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
        private CGButtonType ghsType;
        private CGMouseState mouseState;
        private Graphics ghs;
        private Point oldPos, curPos, downPos;
        private Pen ghsPen = new Pen(Color.Black, 1);
        private Pen rawPen = new Pen(Color.White, 1);
        private bool canDrawGraphics = false;
        private bool canClearGraphics = false;
        private CGUserCanvas userCanvas;
        private CGUserGraphics curUserGraphics;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OverideLoad(object sender, EventArgs e)
        {
            ghs = this.CreateGraphics();
            userCanvas = new CGUserCanvas(this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        private void ResumeOldStripButton(CGButtonType gt)
        {
            switch (gt)
            {
                case CGButtonType.CGTypeMove:
                    this.buttonMoveGraphics.Checked = false;
                    break;
                case CGButtonType.CGTypeAdjust:
                    this.buttonAdjustGraphics.Checked = false;
                    break;
                case CGButtonType.CGTypePoint:
                    this.buttonDrawPoint.Checked = false;
                    break;
                case CGButtonType.CGTypeLine:
                    this.buttonDrawLine.Checked = false;
                    break;
                case CGButtonType.CGTypeCircle:
                    this.buttonDrawCircle.Checked = false;
                    break;
                case CGButtonType.CGTypeEllipse:
                    this.buttonDrawEllipse.Checked = false;
                    break;
            }
        }

        private void buttonDrawPoint_Click(object sender, EventArgs e)
        {
            if (ghsType == CGButtonType.CGTypePoint)
            {
                this.buttonDrawPoint.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawPoint.Checked = true;
                ghsType = CGButtonType.CGTypePoint;
            }
        }

        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            if(ghsType == CGButtonType.CGTypeLine)
            {
                this.buttonDrawLine.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawLine.Checked = true;
                ghsType = CGButtonType.CGTypeLine;
            }
        }

        private void buttonDrawCircle_Click(object sender, EventArgs e)
        {
            if (ghsType == CGButtonType.CGTypeCircle)
            {
                this.buttonDrawCircle.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawCircle.Checked = true;
                ghsType = CGButtonType.CGTypeCircle;
            }
        }

        private void buttonDrawEllipse_Click(object sender, EventArgs e)
        {
            if (ghsType == CGButtonType.CGTypeEllipse)
            {
                this.buttonDrawEllipse.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonDrawEllipse.Checked = true;
                ghsType = CGButtonType.CGTypeEllipse;
            }
        }

        private void buttonMoveGraphics_Click(object sender, EventArgs e)
        {
            if (ghsType == CGButtonType.CGTypeMove)
            {
                this.buttonMoveGraphics.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonMoveGraphics.Checked = true;
                ghsType = CGButtonType.CGTypeMove;
            }
        }

        private void buttonSaveBitmap_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = ".";
            sfd.Filter = "图片文件(*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                String strFileName = sfd.FileName;
                userCanvas.bmp.Save(strFileName);
            }
        }

        private void buttonAdjustGraphics_Click(object sender, EventArgs e)
        {
            if (ghsType == CGButtonType.CGTypeAdjust)
            {
                this.buttonAdjustGraphics.Checked = false;
                ghsType = CGButtonType.CGTypeNothing;
            }
            else
            {
                ResumeOldStripButton(ghsType);
                this.buttonAdjustGraphics.Checked = true;
                ghsType = CGButtonType.CGTypeAdjust;
            }
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            mouseState = CGMouseState.CGMouseStateUp;
            if (canClearGraphics)
            {
                userCanvas.ClearGraphicsSelected(curUserGraphics);
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
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

            userCanvas.SelectGraphicsByCursor(new Point(e.X, e.Y));
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }

        private void MainWindow_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            ghs.DrawImage(userCanvas.bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
        }


        private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Back:
                    
                    if (userCanvas.isUserGraphicsSelected)
                    {
                        userCanvas.DeleteSelectedGraphics();
                        ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    }
                    break;
            }
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

            // FIXME
            if (userCanvas.isUserGraphicsSelected && mouseState == CGMouseState.CGMouseStateDown)
            {
                if (ghsType == CGButtonType.CGTypeAdjust)
                {
                    userCanvas.AdjustGraphicsByCursor(oldPos, new Point(e.X, e.Y));
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                else
                {
                    userCanvas.MoveSelectedGraphics(e.X - oldPos.X, e.Y - oldPos.Y);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                return;
            }

            switch (ghsType)
            {
                case CGButtonType.CGTypePoint:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsPoint singlePoint = new CGUserGraphicsPoint(curPos);
                    userCanvas.SelectGraphics(singlePoint);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = singlePoint;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGTypeLine:
                    if(canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsLine line = new CGUserGraphicsLine(downPos, curPos);
                    userCanvas.SelectGraphics(line);
                    userCanvas.SetGraphicsSelected(line);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = line;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGTypeCircle:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsCircle circle = new CGUserGraphicsCircle(downPos, curPos);
                    userCanvas.SelectGraphics(circle);
                    userCanvas.SetGraphicsSelected(circle);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = circle;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGTypeEllipse:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse(downPos, curPos);
                    userCanvas.SelectGraphics(ellipse);
                    userCanvas.SetGraphicsSelected(ellipse);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = ellipse;
                    canClearGraphics = true;
                    break;
            }

            
        }
    }
}
