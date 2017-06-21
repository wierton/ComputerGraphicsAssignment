using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
        private void buttonDrawBStyleCurve_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonDrawBStyleCurve);

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

        private void buttonInternalColoring_Click(object sender, EventArgs e)
        {
            userCanvas.InternalColoring();
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
        }

        private void buttonTrim_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonTrim);
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = ".";
            ofd.Filter = "图元文件(*.cg)|*.cg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                String strFileName = ofd.FileName;

                userCanvas.LoadFromCGFile(strFileName);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
        }

        private void buttonSaveBitmap_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = ".";
            sfd.Filter = "图元文件(*.cg)|*.cg|图片文件(*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                String strFileName = sfd.FileName;

                if (strFileName.EndsWith(".cg"))
                {
                    userCanvas.SaveToCGFile(strFileName);
                }
                else
                {
                    userCanvas.bmp.Save(strFileName);
                }
            }
        }


        private void buttonAdjustGraphics_Click(object sender, EventArgs e)
        {
            normalButtonClicked(this.buttonAdjustGraphics);
        }

        private void commandHolder_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void MainWindow_LeftMouseUp(object sender, MouseEventArgs e)
        {
            if (buttonClicked == this.buttonDrawPolygon
                || buttonClicked == this.buttonDrawBezier
                || buttonClicked == this.buttonDrawBStyleCurve)
            {
                mouseState = CGMouseState.CGMouseStateUp;

                isUpdatingGraphicsWhenMouseUp = true;

                upPos.X = e.X; upPos.Y = e.Y;

                canUpdateGraphics = true;
            }
            else
            {
                if (buttonClicked == this.buttonTrim)
                {
                    userCanvas.trimSelectedGraphics(userCanvas.trimArea);
                    ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
                }
                // log.write("mouse set to up, canUpdateGraphics --> false");
                mouseState = CGMouseState.CGMouseStateUp;
                canUpdateGraphics = false;
            }

            if (canClearGraphics)
            {
                // clear current graphics in userCanvas
                // userCanvas.ClearStateOfSelectedGraphics();
                // flip userCanvas to ghs
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }

            canClearGraphics = false;
        }

        private void MainWindow_RightMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                MainWindow_LeftMouseUp(sender, e);
            else
                MainWindow_RightMouseUp(sender, e);
        }


        private void MainWindow_LeftMouseDown(object sender, MouseEventArgs e)
        {
            // log.write("mouse state set to down");
            mouseState = CGMouseState.CGMouseStateDown;

            if (buttonClicked != this.buttonDrawPolygon
                && buttonClicked != this.buttonDrawBezier
                && buttonClicked != this.buttonDrawBStyleCurve)
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
                || buttonClicked == this.buttonZoomGraphics
                || buttonClicked == null)
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

                isUpdatingGraphicsWhenMouseUp = false;

                userCanvas.ClearStateOfSelectedGraphics();

                polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });

                CGUserGraphicsPolygon polygon = new CGUserGraphicsPolygon(polygonEdgeSet);
                userCanvas.AddGraphics(polygon);
                userCanvas.SetGraphicsSelected(polygon);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);              
                polygonEdgeSet.Clear();

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

                isUpdatingGraphicsWhenMouseUp = false;

                userCanvas.ClearStateOfSelectedGraphics();

                polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });
                polygonEdgeSet.Clear();

                CGUserGraphicsBezier curve = new CGUserGraphicsBezier(downPointSet);

                userCanvas.AddGraphics(curve);
                userCanvas.SetGraphicsSelected(curve);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);

                downPointSet.Clear();
            }
            else if (buttonClicked == this.buttonDrawBStyleCurve)
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

                foreach (CGUserGraphicsLine l in polygonEdgeSet)
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

                isUpdatingGraphicsWhenMouseUp = false;

                userCanvas.ClearStateOfSelectedGraphics();

                polygonEdgeSet.ForEach((l) => { userCanvas.RemoveGraphics(l); });
                polygonEdgeSet.Clear();

                CGUserGraphicsBStyleCurve curve = new CGUserGraphicsBStyleCurve(downPointSet);

                userCanvas.AddGraphics(curve);
                userCanvas.SetGraphicsSelected(curve);

                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);

                downPointSet.Clear();
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

        private void buttonDrawCube_Click(object sender, EventArgs e)
        {
            Wpf3D.MainWindow wpfwindow = new Wpf3D.MainWindow();
            wpfwindow.ShowDialog();

            /*
            Point a = new Point(301, 178);
            Point b = new Point(237, 196);
            Point c = new Point(315, 280);
            Point d = new Point(166, 253);

            CGUserGraphics g = new CGUserGraphics();
            MessageBox.Show(String.Format("{0}", g.IsSegmentsIntersect(a, b, c, d)));

            a = new Point(0, 0);
            b = new Point(2, 2);
            c = new Point(1, 0);
            d = new Point(3, 2);
            MessageBox.Show(String.Format("{0}", g.IsSegmentsIntersect(a, b, c, d)));


            
            CGUserGraphicsPolygon polygon = new CGUserGraphicsPolygon(new List<Point>()
            {
                new Point(100, 100),
                new Point(140, 140),
                new Point(100, 180),
            }
            );

            Rectangle rect = new Rectangle(120, 100, 60, 41);

            polygon.TransformTrim(rect);
            userCanvas.AddGraphics(polygon);
            ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            */
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

        private void NormalPartOfUpdateMultPointGraphics(CGUserGraphicsLine pline)
        {
            if (NormalPartOfUpdateTwoPointGraphics(pline))
            {
                polygonEdgeSet.Remove(oldEdgeLineOfPolygon);
            }

            oldEdgeLineOfPolygon = pline;
            polygonEdgeSet.Add(pline);
        }

        private delegate CGUserGraphics UpdateCurveOperation(ref List<Point> lps);
        private void NormalPartOfUpdateMultPointCurve(UpdateCurveOperation ucof)
        {
            if (polygonEdgeSet.Count >= 2)
            {
                List<Point> tpl = new List<Point>();
                polygonEdgeSet.ForEach((l) => { tpl.Add(l.firstPoint); });
                tpl.Add(curPos);

                if (oldSelectedCurve != null)
                {
                    userCanvas.RemoveGraphics(oldSelectedCurve);
                }
                oldSelectedCurve = ucof(ref tpl);
                userCanvas.AddGraphics(oldSelectedCurve);
                ghs.DrawImage(userCanvas.bmp, this.ClientRectangle);
            }
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            oldPos = curPos;
            curPos.X = e.X;
            curPos.Y = e.Y;

            //this.toolStripLabel.Text = String.Format("({0}, {1})", e.X, e.Y);

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


            // update graphics if a graphics is drawing
            switch (buttonClicked.Text)
            {
                case "buttonDrawPoint":
                    NormalPartOfUpdateTwoPointGraphics(new CGUserGraphicsPoint(curPos));
                    break;
                case "buttonDrawBezier":
                    NormalPartOfUpdateMultPointGraphics(new CGUserGraphicsDottedLine(upPos, curPos));
                    NormalPartOfUpdateMultPointCurve((ref List<Point> lps)=> new CGUserGraphicsBezier(lps));
                    break;
                case "buttonDrawBStyleCurve":
                    NormalPartOfUpdateMultPointGraphics(new CGUserGraphicsDottedLine(upPos, curPos));
                    NormalPartOfUpdateMultPointCurve((ref List<Point> lps) => new CGUserGraphicsBStyleCurve(lps));
                    break;
                case "buttonDrawPolygon":
                    NormalPartOfUpdateMultPointGraphics(new CGUserGraphicsLine(upPos, curPos));
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
