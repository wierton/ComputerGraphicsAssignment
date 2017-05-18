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
        CGButtonTypeNothing,
        CGButtonTypeSave,
        CGButtonTypeMove,
        CGButtonTypeAdjust,
        CGButtonTypePoint,
        CGButtonTypeLine,
        CGButtonTypeCircle,
        CGButtonTypeEllipse,
        CGButtonTypePolygon,
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
        private CGButtonType btnType;
        private CGMouseState mouseState;
        private Graphics ghs;
        private Point oldPos, curPos, downPos;
        private Pen ghsPen = new Pen(Color.Black, 1);
        private Pen rawPen = new Pen(Color.White, 1);
        private bool canDrawGraphics = false;
        private bool canClearGraphics = false;
        private CGUserCanvas userCanvas;
        private CGUserGraphics curUserGraphics;
        private bool isPolygonDrawing = false;
        private bool isPolygonDBClicked = false;

        private List<Point> polygonsPointSet = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OverideLoad(object sender, EventArgs e)
        {
            ghs = this.CreateGraphics();
            userCanvas = new CGUserCanvas(this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        private void ResumeOldStripButton(CGButtonType bt)
        {
            switch (bt)
            {
                case CGButtonType.CGButtonTypeMove:
                    this.buttonMoveGraphics.Checked = false;
                    break;
                case CGButtonType.CGButtonTypeAdjust:
                    this.buttonAdjustGraphics.Checked = false;
                    break;
                case CGButtonType.CGButtonTypePoint:
                    this.buttonDrawPoint.Checked = false;
                    break;
                case CGButtonType.CGButtonTypeLine:
                    this.buttonDrawLine.Checked = false;
                    break;
                case CGButtonType.CGButtonTypeCircle:
                    this.buttonDrawCircle.Checked = false;
                    break;
                case CGButtonType.CGButtonTypeEllipse:
                    this.buttonDrawEllipse.Checked = false;
                    break;
                case CGButtonType.CGButtonTypePolygon:
                    this.buttonDrawPolygon.Checked = false;
                    break;
            }
        }

        private void buttonDrawPoint_Click(object sender, EventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePoint)
            {
                this.buttonDrawPoint.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonDrawPoint.Checked = true;
                btnType = CGButtonType.CGButtonTypePoint;
            }
        }

        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            if(btnType == CGButtonType.CGButtonTypeLine)
            {
                this.buttonDrawLine.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonDrawLine.Checked = true;
                btnType = CGButtonType.CGButtonTypeLine;
            }
        }

        private void buttonDrawCircle_Click(object sender, EventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypeCircle)
            {
                this.buttonDrawCircle.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonDrawCircle.Checked = true;
                btnType = CGButtonType.CGButtonTypeCircle;
            }
        }

        private void buttonDrawEllipse_Click(object sender, EventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypeEllipse)
            {
                this.buttonDrawEllipse.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonDrawEllipse.Checked = true;
                btnType = CGButtonType.CGButtonTypeEllipse;
            }
        }
        private void buttonDrawPolygon_Click(object sender, EventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePolygon)
            {
                this.buttonDrawPolygon.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonDrawPolygon.Checked = true;
                btnType = CGButtonType.CGButtonTypePolygon;
            }
        }
        private void buttonMoveGraphics_Click(object sender, EventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypeMove)
            {
                this.buttonMoveGraphics.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonMoveGraphics.Checked = true;
                btnType = CGButtonType.CGButtonTypeMove;
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
            if (btnType == CGButtonType.CGButtonTypeAdjust)
            {
                this.buttonAdjustGraphics.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                this.buttonAdjustGraphics.Checked = true;
                btnType = CGButtonType.CGButtonTypeAdjust;
            }
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePolygon)
            {
                new UserLog("mouse set to twice up");
                mouseState = CGMouseState.CGMouseStateTwiceUp;
                isPolygonDrawing = true;
                downPos.X = e.X;
                downPos.Y = e.Y;
                if (isPolygonDBClicked)
                {
                    isPolygonDrawing = false;
                    isPolygonDBClicked = false;

                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    CGUserGraphicsPolygon newPolygon = new CGUserGraphicsPolygon(polygonsPointSet);
                    userCanvas.SelectGraphics(newPolygon);
                    polygonsPointSet.RemoveAll((u)=> { return true; });
                    newPolygon.edgeLines.ForEach((l)=> { userCanvas.ClearGraphicsSelected(l); });
                    userCanvas.SelectGraphics(newPolygon);
                }
                else
                {
                    polygonsPointSet.Add(new Point(e.X, e.Y));
                    canDrawGraphics = true;
                }
                
            }
            else
            {
                new UserLog("mouse set to up");
                mouseState = CGMouseState.CGMouseStateUp;
                canDrawGraphics = false;
            }

            if (canClearGraphics)
            {
                // clear current graphics in userCanvas
                userCanvas.ClearGraphicsSelected(curUserGraphics);
                // flip userCanvas to ghs
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
            
            canClearGraphics = false;
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            new UserLog("mouse state set to down");
            mouseState = CGMouseState.CGMouseStateDown;

            if (btnType != CGButtonType.CGButtonTypePolygon)
            {
                canDrawGraphics = true;
                canClearGraphics = false;
            }
            downPos.X = e.X;
            downPos.Y = e.Y;

            if (btnType == CGButtonType.CGButtonTypeMove || btnType == CGButtonType.CGButtonTypeAdjust)
            {
                userCanvas.SelectGraphicsByCursor(new Point(e.X, e.Y));
            }
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }
        private void MainWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePolygon) 
            {
                new UserLog("mouse double click");

                int dx = e.X - polygonsPointSet[0].X;
                int dy = e.Y - polygonsPointSet[0].Y;

                if (dx * dx + dy * dy < 4 * 4)
                {
                    mouseState = CGMouseState.CGMouseStateUp;
                    canDrawGraphics = false;
                    isPolygonDBClicked = true;
                    isPolygonDrawing = false;

                    Point firstPos = polygonsPointSet[0];
                    CGUserGraphicsLine line = new CGUserGraphicsLine(downPos, firstPos);
                    userCanvas.SelectGraphics(line);
                    userCanvas.SetGraphicsSelected(line);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
            }
        }

        private void MainWindow_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            ghs.DrawImage(userCanvas.bmp, new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
        }


        private void buttonDrawRotation_Click(object sender, EventArgs e)
        {

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
                new UserLog(String.Format("return from MouseMove handler, {0}, {1}", canDrawGraphics, mouseState));
                return;
            }

            // FIXME
            if (userCanvas.isUserGraphicsSelected && mouseState == CGMouseState.CGMouseStateDown)
            {
                if (btnType == CGButtonType.CGButtonTypeAdjust)
                {
                    userCanvas.AdjustGraphicsByCursor(oldPos, new Point(e.X, e.Y));
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                else if(btnType == CGButtonType.CGButtonTypeMove)
                {
                    userCanvas.MoveSelectedGraphics(e.X - oldPos.X, e.Y - oldPos.Y);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                return;
            }

            switch (btnType)
            {
                case CGButtonType.CGButtonTypePoint:
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
                case CGButtonType.CGButtonTypePolygon:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.ClearGraphics(curUserGraphics);
                    }

                    if (!isPolygonDrawing)
                        break;

                    Point lastPos = polygonsPointSet[polygonsPointSet.Count - 1];
                    CGUserGraphicsLine pline = new CGUserGraphicsLine(lastPos, curPos);
                    userCanvas.SelectGraphics(pline);
                    userCanvas.SetGraphicsSelected(pline);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = pline;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGButtonTypeLine:
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
                case CGButtonType.CGButtonTypeCircle:
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
                case CGButtonType.CGButtonTypeEllipse:
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
