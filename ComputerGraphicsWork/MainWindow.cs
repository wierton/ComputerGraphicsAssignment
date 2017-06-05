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

    enum CGMouseState
    {
        CGMouseStateDown = 0,
        CGMouseStateUp = 1,
    };

    public partial class MainWindow : Form
    {
        private UserLog log = new UserLog("log.txt");

        private ToolStripButton buttonClicked = null;
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

        private CGUserGraphics oldSelectedCurve = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OverideLoad(object sender, EventArgs e)
        {
            ghs = this.CreateGraphics();
            userCanvas = new CGUserCanvas(this.ClientRectangle.Width, this.ClientRectangle.Height);
        }


        private void normalButtonClicked(ToolStripButton newClickedButtonObject)
        {
            if (buttonClicked == newClickedButtonObject)
            {
                buttonClicked.Checked = false;
                buttonClicked = null;
            }
            else
            {
                if(buttonClicked != null)
                    buttonClicked.Checked = false;
                newClickedButtonObject.Checked = true;
                buttonClicked = newClickedButtonObject;
            }
        }

        private void buttonDrawPoint_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawPoint);
        }

        private void buttonDrawLine_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawLine);
        }
        private void buttonDrawRectangle_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawRectangle);
        }

        private void buttonDrawCircle_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawCircle);
        }

        private void buttonDrawEllipse_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawEllipse);
        }
        private void buttonDrawPolygon_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawPolygon);
        }

        private void buttonDrawBezier_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawBezier);
        }

        private void buttonMoveGraphics_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonMoveGraphics);
        }

        private void buttonZoomGraphics_Click(object sender, EventArgs e)
        {
            userCanvas.SetBasePoint(new Point(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2));
            normalButtonClicked(this.buttonZoomGraphics);
        }

        private void buttonRotation_Click(object sender, EventArgs e)
        {
            userCanvas.SetBasePoint(new Point(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2));
            normalButtonClicked(this.buttonRotation);
        }

        private void buttonColoring_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonColoring);
        }

        private void buttonTrim_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonTrim);
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
            normalButtonClicked(this.buttonAdjustGraphics);
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (buttonClicked == this.buttonDrawPolygon
                || buttonClicked == this.buttonDrawBezier)
            {
                // log.write("mouse set to up");
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
                // log.write("MainWindow_MouseUp, canUpdateGraphics --> false");
                canUpdateGraphics = true;
                
            }
            else
            {
                if (buttonClicked == this.buttonTrim)
                {
                    userCanvas.trimAllGraphics(userCanvas.trimArea);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                // log.write("mouse set to up, canUpdateGraphics --> false");
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
            // log.write("mouse state set to down");
            mouseState = CGMouseState.CGMouseStateDown;

            if (buttonClicked != this.buttonDrawPolygon
                && buttonClicked != this.buttonDrawBezier)
            {
                // log.write(String.Format("canUpdateGraphics:{0} --> true", canUpdateGraphics));
                canUpdateGraphics = true;
                canClearGraphics = false;
            }

            // set downpos
            this.downPos.X = e.X;
            this.downPos.Y = e.Y;

            if (buttonClicked == this.buttonMoveGraphics
                || buttonClicked == this.buttonAdjustGraphics
                || buttonClicked == this.buttonRotation
                || buttonClicked == this.buttonZoomGraphics)
            {
                userCanvas.SelectGraphicsByCursor(new Point(e.X, e.Y));
            }
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }
        private void MainWindow_RightMouseDown(object sender, MouseEventArgs e)
        {
            // note: the order of each event when double clicked
            //       down -> up -> down -> dbclick -> up

            if (buttonClicked == this.buttonDrawPolygon)
            {
                // log.write("mouse double click");

                if (polygonEdgeSet.Count == 0)
                    return;

                int dx = e.X - polygonEdgeSet[0].firstPoint.X;
                int dy = e.Y - polygonEdgeSet[0].firstPoint.Y;

                if (dx * dx + dy * dy > 3 * 3)
                {
                    CGUserGraphicsLine pline = new CGUserGraphicsLine(new Point(e.X, e.Y), polygonEdgeSet[0].firstPoint);
                    userCanvas.AddGraphics(pline);
                    polygonEdgeSet.Add(pline);
                }

                mouseState = CGMouseState.CGMouseStateUp;
                // log.write("canUpdateGraphics --> false");
                canUpdateGraphics = false;

                isRightMouseClicked = true;
                isUpdatingGraphicsWhenMouseUp = false;

                userCanvas.ClearStateOfSelectedGraphics();

                polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });

                CGUserGraphicsPolygon polygon = new CGUserGraphicsPolygon(polygonEdgeSet);
                userCanvas.AddGraphics(polygon);
                userCanvas.SetGraphicsSelected(polygon);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);              
                polygonEdgeSet.RemoveAll((l) => { return true; });

            }
            else if(buttonClicked == this.buttonDrawBezier)
            {
                if (polygonEdgeSet.Count <= 1)
                    return;

                if (oldSelectedCurve != null)
                {
                    userCanvas.RemoveGraphics(oldSelectedCurve);
                    oldSelectedCurve = null;
                }

                int dx = e.X - polygonEdgeSet[0].firstPoint.X;
                int dy = e.Y - polygonEdgeSet[0].firstPoint.Y;

                List<Point> downPointSet = new List<Point>();

                foreach(CGUserGraphicsLine l in polygonEdgeSet)
                {
                    downPointSet.Add(l.firstPoint);
                }

                if (dx * dx + dy * dy < 3 * 3)
                {
                    downPointSet.Add(polygonEdgeSet[0].firstPoint);
                }
                else
                {
                    downPointSet.Add(polygonEdgeSet[polygonEdgeSet.Count - 1].nextPoint);
                }

                mouseState = CGMouseState.CGMouseStateUp;
                // log.write("canUpdateGraphics --> false");
                canUpdateGraphics = false;

                isRightMouseClicked = true;
                isUpdatingGraphicsWhenMouseUp = false;

                userCanvas.ClearStateOfSelectedGraphics();

                polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });
                polygonEdgeSet.RemoveAll((l) => { return true; });

                CGUserGraphicsBezier curve = new CGUserGraphicsBezier(downPointSet);

                userCanvas.AddGraphics(curve);
                userCanvas.SetGraphicsSelected(curve);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);

                downPointSet.RemoveAll((l) => { return true; });
            }
            else if(buttonClicked == this.buttonRotation
                || buttonClicked == this.buttonZoomGraphics)
            {
                userCanvas.SetBasePoint(new Point(e.X, e.Y));

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
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


        private void MainWindow_MouseClick(object sender, MouseEventArgs e)
        {
            if (buttonClicked == this.buttonColoring)
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
        private bool NormalPartOfUpdateTwoPointGraphics(CGUserGraphics graphics)
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

        private void NormalPartOfUpdateMultPointGraphics()
        {
            CGUserGraphicsLine pline = new CGUserGraphicsLine(upPos, curPos);
            if (NormalPartOfUpdateTwoPointGraphics(pline))
            {
                polygonEdgeSet.Remove(oldEdgeLineOfPolygon);
            }

            oldEdgeLineOfPolygon = pline;
            polygonEdgeSet.Add(pline);
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            oldPos = curPos;
            curPos.X = e.X;
            curPos.Y = e.Y;

            if (buttonClicked == null)
                return;

            if (!canUpdateGraphics)
            {
                // log.write(String.Format("return from MouseMove handler, canUpdateGraphics={0}", canUpdateGraphics));
                return;
            }

            if(mouseState == CGMouseState.CGMouseStateUp && !isUpdatingGraphicsWhenMouseUp)
            {
                // log.write(String.Format("return from MouseMove handler, mouseState={0}, pass={1}", mouseState, isUpdatingGraphicsWhenMouseUp));
                return;
            }

            // move or adjust graphics if a graphics is selected
            // cond: 1. some graphics is selected in userCanvas
            //       2. mouse is down
            if (userCanvas.isUserGraphicsSelected && mouseState == CGMouseState.CGMouseStateDown)
            {
                if (buttonClicked == this.buttonAdjustGraphics)
                {
                    userCanvas.AdjustGraphicsByCursor(oldPos, new Point(e.X, e.Y));
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    // log.write("return since isUserGraphicsSelected");
                    return;
                }
                else if(buttonClicked == this.buttonMoveGraphics)
                {
                    userCanvas.MoveSelectedGraphics(new Point(e.X, e.Y));
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    // log.write("return since isUserGraphicsSelected");
                    return;
                }
                else if(buttonClicked == this.buttonRotation)
                {
                    userCanvas.RotateSelectedGraphics(curPos);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    return;
                }
                else if(buttonClicked == buttonZoomGraphics)
                {
                    userCanvas.ZoomSelectedGraphics(curPos);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    return;
                }
            }

            // log.write("mouse move");

            // update graphics if a graphics is drawing
            switch (buttonClicked.Text)
            {
                case "buttonDrawPoint":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsPoint(curPos));
                    break;
                case "buttonDrawBezier":
                    NormalPartOfUpdateMultPointGraphics();

                    if (polygonEdgeSet.Count >= 2)
                    {
                        List<Point> tpl = new List<Point>();
                        polygonEdgeSet.ForEach((l)=> { tpl.Add(l.firstPoint); });
                        tpl.Add(polygonEdgeSet[polygonEdgeSet.Count - 1].nextPoint);

                        if (oldSelectedCurve != null)
                        {
                            userCanvas.RemoveGraphics(oldSelectedCurve);
                        }
                        oldSelectedCurve = new CGUserGraphicsBezier(tpl);
                        userCanvas.AddGraphics(oldSelectedCurve);
                    }
                    
                    break;
                case "buttonDrawPolygon":
                    NormalPartOfUpdateMultPointGraphics();
                    break;
                case "buttonDrawLine":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsLine(downPos, curPos));
                    break;
                case "buttonDrawRectangle":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsRectangle(downPos, curPos));
                    break;
                case "buttonDrawCircle":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsCircle(downPos, curPos));
                    break;
                case "buttonDrawEllipse":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsEllipse(downPos, curPos));
                    break;
                case "buttonTrim":
                    userCanvas.adjustTrimArea(downPos, curPos);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                    break;
            }
        }
    }
}
