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
        CGButtonTypeColoring,
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
        private Point oldPos, curPos, downPos, upPos;
        private Pen ghsPen = new Pen(Color.Black, 1);
        private Pen rawPen = new Pen(Color.White, 1);
        private bool canUpdateGraphics = false;
        private bool canClearGraphics = false;
        private CGUserCanvas userCanvas;
        private CGUserGraphics curUserGraphics;

        private bool isRightMouseClicked = false;
        private bool isUpdatingGraphicsWhenMouseUp = false;

        CGUserGraphicsLine oldEdgeLineOfPolygon = null;
        private List<CGUserGraphicsLine> polygonEdgeSet = new List<CGUserGraphicsLine>();

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

        private void buttonColoring_Click(object sender, EventArgs e)
        {
            normalButtonClicked(CGButtonType.CGButtonTypeColoring, this.buttonColoring);
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
                log.write("mouse set to up");
                mouseState = CGMouseState.CGMouseStateUp;

                if (isRightMouseClicked)
                {
                    isRightMouseClicked = false;
                }
                else
                {
                    isUpdatingGraphicsWhenMouseUp = true;
                }

                upPos.X = e.X; upPos.Y = e.Y;
                log.write("MainWindow_MouseUp, canUpdateGraphics --> false");
                canUpdateGraphics = true;
                
            }
            else
            {
                log.write("mouse set to up, canUpdateGraphics --> false");
                mouseState = CGMouseState.CGMouseStateUp;
                canUpdateGraphics = false;
            }

            if (canClearGraphics)
            {
                // clear current graphics in userCanvas
                userCanvas.ClearStateOfSelectedGraphics();
                // flip userCanvas to ghs
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
            
            canClearGraphics = false;
        }

        private void MainWindow_LeftMouseDown(object sender, MouseEventArgs e)
        {
            log.write("mouse state set to down");
            mouseState = CGMouseState.CGMouseStateDown;

            if (btnType != CGButtonType.CGButtonTypePolygon)
            {
                log.write(String.Format("canUpdateGraphics:{0} --> true", canUpdateGraphics));
                canUpdateGraphics = true;
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
        private void MainWindow_RightMouseDown(object sender, MouseEventArgs e)
        {
            // note: the order of each event when double clicked
            //       down -> up -> down -> dbclick -> up
            if (btnType == CGButtonType.CGButtonTypePolygon)
            {
                log.write("mouse double click");

                if (polygonEdgeSet.Count == 0)
                    return;

                int dx = e.X - polygonEdgeSet[0].firstPoint.X;
                int dy = e.Y - polygonEdgeSet[0].firstPoint.Y;

                if (dx * dx + dy * dy < 3 * 3)
                {
                    mouseState = CGMouseState.CGMouseStateUp;
                    log.write("canUpdateGraphics --> false");
                    canUpdateGraphics = false;

                    isRightMouseClicked = true;
                    isUpdatingGraphicsWhenMouseUp = false;

                    userCanvas.ClearStateOfSelectedGraphics();

                    CGUserGraphicsPolygon polygon = new CGUserGraphicsPolygon(polygonEdgeSet);
                    userCanvas.AddGraphics(polygon);
                    userCanvas.SetGraphicsSelected(polygon);

                    polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });
                    polygonEdgeSet.RemoveAll((l) => { return true; });

                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
            }
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                MainWindow_LeftMouseDown(sender, e);
            else
                MainWindow_RightMouseDown(sender, e);
        }


        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }


        private void buttonDrawRotation_Click(object sender, EventArgs e)
        {

        }

        private void MainWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (btnType == CGButtonType.CGButtonTypeColoring)
            {
                userCanvas.Coloring(new Point(e.X, e.Y));
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (userCanvas.isUserGraphicsSelected)
                    {
                        userCanvas.DeleteSelectedGraphics();
                        ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    }
                    break;
            }
        }

        // return true if can clear old graphics
        private bool NormalPartOfUpdateGraphics(CGUserGraphics graphics)
        {
            bool ret = false;
            if (canClearGraphics)
            {
                userCanvas.ClearStateOfSelectedGraphics();
                userCanvas.RemoveGraphics(curUserGraphics);
                ret = true;
            }

            userCanvas.AddGraphics(graphics);
            userCanvas.SetGraphicsSelected(graphics);
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            curUserGraphics = graphics;
            canClearGraphics = true;
            return ret;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            oldPos = curPos;
            curPos.X = e.X;
            curPos.Y = e.Y;

            if (!canUpdateGraphics)
            {
                log.write(String.Format("return from MouseMove handler, canUpdateGraphics={0}", canUpdateGraphics));
                return;
            }

            if(mouseState == CGMouseState.CGMouseStateUp && !isUpdatingGraphicsWhenMouseUp)
            {
                log.write(String.Format("return from MouseMove handler, mouseState={0}, pass={1}", mouseState, isUpdatingGraphicsWhenMouseUp));
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

            log.write("mouse move");

            // update graphics if a graphics is drawing
            switch (btnType)
            {
                case CGButtonType.CGButtonTypePoint:
                    NormalPartOfUpdateGraphics(new CGUserGraphicsPoint(curPos));
                    break;
                case CGButtonType.CGButtonTypePolygon:
                    CGUserGraphicsLine pline = new CGUserGraphicsLine(upPos, curPos);
                    if (NormalPartOfUpdateGraphics(pline))
                    {
                        polygonEdgeSet.Remove(oldEdgeLineOfPolygon);
                    }

                    oldEdgeLineOfPolygon = pline;
                    polygonEdgeSet.Add(pline);
                    break;
                case CGButtonType.CGButtonTypeLine:
                    NormalPartOfUpdateGraphics(new CGUserGraphicsLine(downPos, curPos));
                    break;
                case CGButtonType.CGButtonTypeCircle:
                    NormalPartOfUpdateGraphics(new CGUserGraphicsCircle(downPos, curPos));
                    break;
                case CGButtonType.CGButtonTypeEllipse:
                    NormalPartOfUpdateGraphics(new CGUserGraphicsEllipse(downPos, curPos));
                    break;
            }
        }
    }
}
