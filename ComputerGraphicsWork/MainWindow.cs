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
    };

    public partial class MainWindow : Form
    {
        private UserLog log = new UserLog("log.txt");

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

        private void normalButtonClicked(CGButtonType clickedButtonType, System.Windows.Forms.ToolStripButton clickedButtonObject)
        {
            if (btnType == clickedButtonType)
            {
                clickedButtonObject.Checked = false;
                btnType = CGButtonType.CGButtonTypeNothing;
            }
            else
            {
                ResumeOldStripButton(btnType);
                clickedButtonObject.Checked = true;
                btnType = clickedButtonType;
            }
        }

        private void buttonDrawPoint_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypePoint, this.buttonDrawPoint);
        }

        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypeLine, this.buttonDrawLine);
        }

        private void buttonDrawCircle_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypeCircle, this.buttonDrawCircle);
        }

        private void buttonDrawEllipse_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypeEllipse, this.buttonDrawEllipse);
        }
        private void buttonDrawPolygon_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypePolygon, this.buttonDrawPolygon);
        }

        private void buttonMoveGraphics_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypeMove, this.buttonMoveGraphics);
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
            normalButtonClicked(CGButtonType.CGButtonTypeAdjust, this.buttonAdjustGraphics);
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePolygon)
            {
                log.write("mouse set to twice up");
                mouseState = CGMouseState.CGMouseStateUp;
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
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    CGUserGraphicsPolygon newPolygon = new CGUserGraphicsPolygon(polygonsPointSet);
                    userCanvas.AddGraphics(newPolygon);
                    polygonsPointSet.RemoveAll((u)=> { return true; });
                    newPolygon.edgeLines.ForEach((l)=> { userCanvas.ClearGraphicsSelected(l); });
                    userCanvas.AddGraphics(newPolygon);
                }
                else
                {
                    polygonsPointSet.Add(new Point(e.X, e.Y));
                    log.write("MainWindow_MouseUp, canDrawGraphics --> false");
                    canDrawGraphics = true;
                }
                
            }
            else
            {
                log.write("mouse set to up, canDrawGraphics --> false");
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
            log.write("mouse state set to down");
            mouseState = CGMouseState.CGMouseStateDown;

            if (btnType != CGButtonType.CGButtonTypePolygon)
            {
                log.write(String.Format("canDrawGraphics:{0} --> true", canDrawGraphics));
                canDrawGraphics = true;
                canClearGraphics = false;
            }

            // set downpos
            this.downPos.X = e.X;
            this.downPos.Y = e.Y;

            if (btnType == CGButtonType.CGButtonTypeMove
                || btnType == CGButtonType.CGButtonTypeAdjust)
            {
                userCanvas.SelectGraphicsByCursor(new Point(e.X, e.Y));
            }
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }
        private void MainWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypePolygon) 
            {
                log.write("mouse double click");

                int dx = e.X - polygonsPointSet[0].X;
                int dy = e.Y - polygonsPointSet[0].Y;

                if (dx * dx + dy * dy < 4 * 4)
                {
                    mouseState = CGMouseState.CGMouseStateUp;
                    log.write("canDrawGraphics --> false");
                    canDrawGraphics = false;
                    isPolygonDBClicked = true;
                    isPolygonDrawing = false;

                    Point firstPos = polygonsPointSet[0];
                    CGUserGraphicsLine line = new CGUserGraphicsLine(downPos, firstPos);
                    userCanvas.AddGraphics(line);
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
                log.write(String.Format("return from MouseMove handler, canDrawGraphics={0}, mouseState={1}", canDrawGraphics, mouseState));
                return;
            }

            // move or adjust graphics if a graphics is selected
            // cond: 1. some graphics is selected in userCanvas
            //       2. mouse is down
            if (userCanvas.isUserGraphicsSelected && mouseState == CGMouseState.CGMouseStateDown)
            {
                if (btnType == CGButtonType.CGButtonTypeAdjust)
                {
                    userCanvas.AdjustGraphicsByCursor(oldPos, new Point(e.X, e.Y));
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    log.write("return since isUserGraphicsSelected");
                    return;
                }
                else if(btnType == CGButtonType.CGButtonTypeMove)
                {
                    userCanvas.MoveSelectedGraphics(e.X - oldPos.X, e.Y - oldPos.Y);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    log.write("return since isUserGraphicsSelected");
                    return;
                }
            }

            log.write("try to update graphics");

            // update graphics if a graphics is drawing
            switch (btnType)
            {
                case CGButtonType.CGButtonTypePoint:
                    if (canClearGraphics)
                    {
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    CGUserGraphicsPoint singlePoint = new CGUserGraphicsPoint(curPos);
                    userCanvas.AddGraphics(singlePoint);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = singlePoint;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGButtonTypePolygon:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    if (!isPolygonDrawing)
                        break;

                    Point lastPos = polygonsPointSet[polygonsPointSet.Count - 1];
                    CGUserGraphicsLine pline = new CGUserGraphicsLine(lastPos, curPos);
                    userCanvas.AddGraphics(pline);
                    userCanvas.SetGraphicsSelected(pline);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = pline;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGButtonTypeLine:
                    if(canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    log.write(String.Format("downpos:{0}, newpos:{1}", downPos, curPos));
                    CGUserGraphicsLine line = new CGUserGraphicsLine(downPos, curPos);
                    userCanvas.AddGraphics(line);
                    userCanvas.SetGraphicsSelected(line);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = line;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGButtonTypeCircle:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    CGUserGraphicsCircle circle = new CGUserGraphicsCircle(downPos, curPos);
                    userCanvas.AddGraphics(circle);
                    userCanvas.SetGraphicsSelected(circle);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = circle;
                    canClearGraphics = true;
                    break;
                case CGButtonType.CGButtonTypeEllipse:
                    if (canClearGraphics)
                    {
                        userCanvas.ClearGraphicsSelected(curUserGraphics);
                        userCanvas.RemoveGraphics(curUserGraphics);
                    }

                    CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse(downPos, curPos);
                    userCanvas.AddGraphics(ellipse);
                    userCanvas.SetGraphicsSelected(ellipse);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    curUserGraphics = ellipse;
                    canClearGraphics = true;
                    break;
            }
        }
    }
}
