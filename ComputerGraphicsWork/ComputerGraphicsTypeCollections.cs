using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGraphicsWork
{
    public class Distance
    {
        public static double CalcDistance(Point x, Point y)
        {
            int dx = x.X - y.X;
            int dy = x.Y - y.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    public class Theta
    {
        private bool isThetaValid;
        public double sinTheta { get; }
        public double cosTheta { get; }
        public Theta(Point basePos, Point oldPos, Point newPos)
        {
            Point oldVec = new Point(oldPos.X - basePos.X, oldPos.Y - basePos.Y);
            Point newVec = new Point(newPos.X - basePos.X, newPos.Y - basePos.Y);

            if (oldVec.X == 0 && oldVec.Y == 0 || newVec.X == 0 && newVec.Y == 0)
                isThetaValid = false;
            else
                isThetaValid = true;

            double oldVecSquareMod = oldVec.X * oldVec.X + oldVec.Y * oldVec.Y;
            double newVecSquareMod = newVec.X * newVec.X + newVec.Y * newVec.Y;
            double baseMod = Math.Sqrt(oldVecSquareMod * newVecSquareMod);

            double dotVec = oldVec.X * newVec.X + oldVec.Y * newVec.Y;
            cosTheta = dotVec / baseMod;

            double timesVec = oldVec.X * newVec.Y - oldVec.Y * newVec.X;
            sinTheta = timesVec / baseMod;

/*            if (Math.Abs(sinTheta * sinTheta + cosTheta * cosTheta - 1) > 0.0000000001)
                MessageBox.Show(string.Format("invalid theta: sin={0}, cos={1}, z={2}",
                    sinTheta, cosTheta, sinTheta * sinTheta + cosTheta * cosTheta));*/
        }

        public Point RotatePoint(Point p, Point basePos)
        {
            if (!isThetaValid)
                return p;

            int dx = p.X - basePos.X;
            int dy = p.Y - basePos.Y;
            return new Point(
                (int)(basePos.X + dx * cosTheta - dy * sinTheta + 0.5),
                (int)(basePos.Y + dx * sinTheta + dy * cosTheta + 0.5)
            );
        }

        public Point RotateVector(Point p)
        {
            if (!isThetaValid)
                return p;

            return new Point(
                (int)(p.X * cosTheta - p.Y * sinTheta + 0.5),
                (int)(p.X * sinTheta + p.Y * cosTheta + 0.5)
            );
        }
    }

    public class CGUserGraphics
    {
        public List<Point> pointsSet { get; } = new List<Point>();

        public List<Point> keyPoints = new List<Point>();

        public virtual void CalculatePointsSet()
        {
            // do nothing
        }

        public void UpdatePointsSet()
        {
            pointsSet.RemoveAll((p) => { return true; });
            CalculatePointsSet();
        }

        public virtual bool IsCursorNearby(Point cursorPos)
        {
            foreach(Point p in pointsSet)
            {
                if (Distance.CalcDistance(p, cursorPos) < 4)
                    return true;
            }
            return false;
        }

        public virtual List<Point> CalculateTagPoints()
        {
            return keyPoints;
        }

        public virtual CGUserGraphics TransformRotation(Point basePos, Point oldPos, Point newPos)
        {
            Theta theta = new Theta(basePos, oldPos, newPos);

            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                keyPoints[i] = theta.RotatePoint(keyPoints[i], basePos);
            }
            UpdatePointsSet();
            return this;
        }

        public virtual CGUserGraphics TransformMove(int dx, int dy)
        {
            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                keyPoints[i] = new Point(keyPoints[i].X + dx, keyPoints[i].Y + dy);
            }
            UpdatePointsSet();
            return this;
        }

        public virtual CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            // do nothing
            return this;
        }

        public Point CalculateMiddlePoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        public CGUserGraphics Copy()
        {
            CGUserGraphics graphics = null;
            switch (this.GetType().ToString())
            {
                case "ComputerGraphicsWork.CGUserGraphicsPoint":
                    graphics = new CGUserGraphicsPoint(this.keyPoints[0]);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsLine":
                    graphics = new CGUserGraphicsLine(this.keyPoints[0], this.keyPoints[1]);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsCircle":
                    graphics = new CGUserGraphicsLine(this.keyPoints[0], this.keyPoints[1]);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsEllipse":
                    CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse(this.keyPoints[0], this.keyPoints[1]);
                    ellipse.keyPoints.RemoveAll((p) => { return true; });
                    this.keyPoints.ForEach((p) => { ellipse.keyPoints.Add(p); });
                    ellipse.UpdatePointsSet();
                    graphics = ellipse;
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsPolygon":
                    graphics = new CGUserGraphicsPolygon(this.keyPoints);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("undocumented graphics:{0}", this.GetType().ToString()));
                    break;
            }
            return graphics;
        }
    }

    public class CGUserGraphicsPoint : CGUserGraphics
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public CGUserGraphicsPoint(Point start)
        {
            keyPoints.Add(start);
            CalculatePointsSet();
        }

        public override void CalculatePointsSet()
        {
            X = keyPoints[0].X;
            Y = keyPoints[0].Y;

            pointsSet.RemoveAll((p)=> { return true; });
            pointsSet.Add(keyPoints[0]);
        }

        public override bool IsCursorNearby(Point cursorPos)
        {
            Point point = keyPoints[0];
            int dx = Math.Abs(point.X - cursorPos.X);
            int dy = Math.Abs(point.Y - cursorPos.Y);
            return (dx <= 4 && dy <= 4);
        }
    }

    public class CGUserGraphicsLine : CGUserGraphics
    {
        public Point firstPoint { get; set; }
        public Point nextPoint { get; set; }
        public CGUserGraphicsLine(Point start, Point end)
        {
            keyPoints.Add(start);
            keyPoints.Add(end);
            CalculatePointsSet();
        }
        public override void CalculatePointsSet()
        {
            firstPoint = keyPoints[0];
            nextPoint = keyPoints[1];

            Point start = firstPoint;
            Point end = nextPoint;

            // swap two points
            if (end.X < start.X)
            {
                Point temp = start;
                start = end;
                end = temp;
            }

            int dx = end.X - start.X;
            int dy = end.Y - start.Y;

            pointsSet.Add(start);
            if (dy > 0)
            {
                if (Math.Abs(dx) >= Math.Abs(dy))
                {
                    int p = 2 * dy - dx;
                    for (int i = 0; i < dx; i++)
                    {
                        if (p > 0)
                        {
                            start.X++;
                            start.Y++;
                            pointsSet.Add(start);
                            int ddx = end.X - start.X;
                            int ddy = end.Y - start.Y;
                            p = p + 2 * (ddy - ddx);
                        }
                        else
                        {
                            start.X++;
                            pointsSet.Add(start);
                            int ddy = end.Y - start.Y;
                            p = p + 2 * ddy;
                        }
                    }
                }
                else
                {
                    int p = 2 * dx - dy;
                    for (int i = 0; i < Math.Abs(dy); i++)
                    {
                        if (p > 0)
                        {
                            start.Y++;
                            start.X++;
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
            else
            {
                if (Math.Abs(dx) >= Math.Abs(dy))
                {
                    int p = 2 * dy + dx;
                    for (int i = 0; i < dx; i++)
                    {
                        if (p < 0)
                        {
                            start.X++;
                            start.Y--;
                            pointsSet.Add(start);
                            int ddx = end.X - start.X;
                            int ddy = end.Y - start.Y;
                            p = p + 2 * (ddy + ddx);
                        }
                        else
                        {
                            start.X++;
                            pointsSet.Add(start);
                            int ddy = end.Y - start.Y;
                            p = p + 2 * ddy;
                        }
                    }
                }
                else
                {
                    Point temp = start;
                    start = end;
                    end = temp;

                    dx = end.X - start.X;
                    dy = end.Y - start.Y;

                    int p = 2 * dx + dy;
                    for (int i = 0; i < dy; i++)
                    {
                        if (p < 0)
                        {
                            start.Y++;
                            start.X--;
                            pointsSet.Add(start);
                            int ddy = end.Y - start.Y;
                            int ddx = end.X - start.X;
                            p = p + 2 * (ddx + ddy);
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

        public override bool IsCursorNearby(Point cursorPos)
        {
            int dx = firstPoint.X - nextPoint.X;
            int dy = nextPoint.Y - firstPoint.Y;

            int maxX = Math.Max(firstPoint.X, nextPoint.X);
            int minX = Math.Min(firstPoint.X, nextPoint.X);

            int maxY = Math.Max(firstPoint.Y, nextPoint.Y);
            int minY = Math.Min(firstPoint.Y, nextPoint.Y);

            bool isInX = minX - 4 <= cursorPos.X && cursorPos.X <= maxX + 4;
            bool isInY = minY - 4 <= cursorPos.Y && cursorPos.Y <= maxY + 4;


            double baseDist = Math.Sqrt(dx * dx + dy * dy);
            double c = nextPoint.X * firstPoint.Y - firstPoint.X * nextPoint.Y;

            double d = Math.Abs(cursorPos.X * dy + cursorPos.Y * dx + c) / baseDist;

            if (d < 4 && isInX && isInY)
                return true;
            else
                return false;
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            if(Distance.CalcDistance(keyPoints[0], oldPos) < 4)
            {
                keyPoints[0] = newPos;
                UpdatePointsSet();
            }
            else if(Distance.CalcDistance(keyPoints[0], oldPos) < 4)
            {
                keyPoints[1] = newPos;
                UpdatePointsSet();
            }

            return this;
        }
    }
    public class CGUserGraphicsCircle : CGUserGraphics
    {
        int radius { get; set; }
        // keyPoints[0]: centerPoint, keyPoints[1]: edgePoint
        public CGUserGraphicsCircle(Point center, Point edge)
        {
            keyPoints.Add(center);
            keyPoints.Add(edge);
            CalculatePointsSet();
        }
        public override List<Point> CalculateTagPoints()
        {
            Point center = keyPoints[0];
            Point edge = keyPoints[1];

            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;

            return new List<Point>()
            {
                new Point(center.X + dx, center.Y + dy),
                new Point(center.X - dx, center.Y - dy),
                new Point(center.X - dy, center.Y + dx),
                new Point(center.X + dy, center.Y - dx),
            };
        }

        public override void CalculatePointsSet()
        {
            Point center = keyPoints[0];
            Point edge = keyPoints[1];

            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;
            radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));

            List<Point> baseSet = new List<Point>();

            Point fp = new Point(0, radius);
            baseSet.Add(fp);
            int p = 1 - radius;
            while (fp.X <= fp.Y)
            {
                if (p < 0)
                {
                    fp.X++;
                    p = p + 2 * fp.X + 1;
                    baseSet.Add(fp);
                }
                else
                {
                    fp.X++;
                    fp.Y--;
                    p = p + 2 * fp.X - 2 * fp.Y + 1;
                    baseSet.Add(fp);
                }
            }

            for (int i = baseSet.Count - 1; i >= 0; i--)
            {
                baseSet.Add(new Point(baseSet[i].Y, baseSet[i].X));
            }

            for (int i = baseSet.Count - 1; i >= 0; i--)
                baseSet.Add(new Point(-baseSet[i].X, baseSet[i].Y));

            for (int i = baseSet.Count - 1; i >= 0; i--)
                baseSet.Add(new Point(baseSet[i].X, -baseSet[i].Y));

            baseSet.ForEach((u) => { pointsSet.Add(new Point(u.X + center.X, u.Y + center.Y)); });
        }
        public override bool IsCursorNearby(Point cursorPos)
        {
            Point center = keyPoints[0];
            int dx = cursorPos.X - center.X;
            int dy = cursorPos.Y - center.Y;

            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (Math.Abs(radius - dist) < 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            int dx = newPos.X - oldPos.X;
            int dy = newPos.Y - oldPos.Y;
            if (Math.Abs(Distance.CalcDistance(keyPoints[0], oldPos) - radius) < 4)
            {
                int radius = (int)Distance.CalcDistance(keyPoints[0], newPos);
                keyPoints[1] = new Point(keyPoints[0].X + radius, keyPoints[0].Y);

                pointsSet.RemoveAll((p) => { return true; });
                CalculatePointsSet();
            }
            return this;
        }
    }
    public class CGUserGraphicsEllipse : CGUserGraphics
    {
        /* p[0]          O           p[2]
         * 
         * O             O            O
         * 
         *               O           p[1]
         * */
        int xRadius, yRadius;
        public CGUserGraphicsEllipse(Point p0, Point p1)
        {
            keyPoints.Add(p0);
            keyPoints.Add(p1);
            keyPoints.Add(new Point(p1.X, p0.Y));
            CalculatePointsSet();
        }

        public override void CalculatePointsSet()
        {
            Point center = CalculateMiddlePoint(keyPoints[0], keyPoints[1]);
            if (keyPoints[2].X == keyPoints[1].X && keyPoints[2].Y == keyPoints[0].Y)
            {
                CalculateNormalEllipse(center, keyPoints[1]);
            }
            else
            {
                Point upPoint = CalculateMiddlePoint(keyPoints[0], keyPoints[2]);
                Point rightPoint = CalculateMiddlePoint(keyPoints[2], keyPoints[1]);

                yRadius = (int)Distance.CalcDistance(upPoint, center);
                xRadius = (int)Distance.CalcDistance(rightPoint, center);

                Point fixedUpPoint = new Point(center.X, center.Y + yRadius);
                Point fixedEdgePoint = new Point(center.X + xRadius, center.Y + yRadius);

                CalculateNormalEllipse(center, fixedEdgePoint);

                Theta theta = new Theta(center, fixedUpPoint, upPoint);

                for(int i = pointsSet.Count - 1; i >= 0; i--)
                {
                    pointsSet[i] = theta.RotatePoint(pointsSet[i], center);
                }
            }
        }
        public void CalculateNormalEllipse(Point center, Point edge)
        {
            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;

            xRadius = Math.Abs(dx);
            yRadius = Math.Abs(dy);

            int xRadiusSquare = xRadius * xRadius;
            int yRadiusSquare = yRadius * yRadius;

            int twiceXRadiusSquare = 2 * xRadiusSquare;
            int twiceYRadiusSquare = 2 * yRadiusSquare;

            int px = 0;
            int py = twiceXRadiusSquare * yRadius;

            List<Point> baseSet = new List<Point>();

            Point fp = new Point(0, yRadius);
            baseSet.Add(fp);
            int p = yRadiusSquare - xRadiusSquare * yRadius + xRadiusSquare / 4;
            while (px < py)
            {
                if (p < 0)
                {
                    fp.X++;
                    px += twiceYRadiusSquare;
                    p += yRadiusSquare + px;
                    baseSet.Add(fp);
                }
                else
                {
                    fp.X++;
                    fp.Y--;
                    px += twiceYRadiusSquare;
                    py -= twiceXRadiusSquare;
                    p += yRadiusSquare + px - py;
                    baseSet.Add(fp);
                }
            }

            p = yRadiusSquare * (fp.X * fp.X + fp.X) + xRadiusSquare * (fp.Y - 1) * (fp.Y - 1) - xRadiusSquare * yRadiusSquare;

            while (fp.Y >= 0)
            {
                if (p > 0)
                {
                    fp.Y--;
                    py -= twiceXRadiusSquare;
                    p += xRadiusSquare - py;
                    baseSet.Add(fp);
                }
                else
                {
                    fp.X++;
                    fp.Y--;
                    py -= twiceXRadiusSquare;
                    px += twiceYRadiusSquare;
                    p += xRadiusSquare + px - py;
                    baseSet.Add(fp);
                }
            }

            for (int i = baseSet.Count - 1; i >= 0; i--)
                baseSet.Add(new Point(-baseSet[i].X, baseSet[i].Y));

            for (int i = baseSet.Count - 1; i >= 0; i--)
                baseSet.Add(new Point(baseSet[i].X, -baseSet[i].Y));

            baseSet.ForEach((u) => { pointsSet.Add(new Point(u.X + center.X, u.Y + center.Y)); });
        }
        public override bool IsCursorNearby(Point cursorPos)
        {
            foreach(Point p in pointsSet)
            {
                if (Distance.CalcDistance(cursorPos, p) < 4.0)
                    return true;
            }
            return false;
        }
        public override List<Point> CalculateTagPoints()
        {
            int twicePx = keyPoints[1].X + keyPoints[0].X;
            int twicePy = keyPoints[1].Y + keyPoints[0].Y;

            Point p3 = new Point(twicePx - keyPoints[2].X, twicePy - keyPoints[2].Y);

            return new List<Point>() {
                CalculateMiddlePoint(keyPoints[0], keyPoints[2]),
                CalculateMiddlePoint(keyPoints[2], keyPoints[1]),
                CalculateMiddlePoint(keyPoints[1], p3),
                CalculateMiddlePoint(p3, keyPoints[0]),
            };
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {/*
            if (Distance.CalcDistance(new Point(centerPoint.X, centerPoint.Y + yRadius), oldPos) < 8
             || Distance.CalcDistance(new Point(centerPoint.X, centerPoint.Y - yRadius), oldPos) < 8)
            {
                CGUserGraphicsEllipse newUserGraphics = new CGUserGraphicsEllipse(centerPoint, new Point(edgePoint.X, newPos.Y));
                return newUserGraphics;
            }
            else if (Distance.CalcDistance(new Point(centerPoint.X + xRadius, centerPoint.Y), oldPos) < 8
                  || Distance.CalcDistance(new Point(centerPoint.X - xRadius, centerPoint.Y), oldPos) < 8)
            {
                CGUserGraphicsEllipse newUserGraphics = new CGUserGraphicsEllipse(centerPoint, new Point(newPos.X, edgePoint.Y));
                return newUserGraphics;
            }
            else
            {
                return this;
            }*/
            return this;
        }
    }

    public class CGUserGraphicsPolygon : CGUserGraphics
    {
        public List<CGUserGraphicsLine> edgeLines { get; } = new List<CGUserGraphicsLine>();

        public CGUserGraphicsPolygon(List<Point> inEndPoints)
        {
            inEndPoints.ForEach((u) => { keyPoints.Add(u); });
            CalculatePointsSet();
        }

        public CGUserGraphicsPolygon(List<CGUserGraphicsLine> inEdgeLines)
        {
            inEdgeLines.ForEach((l) => { edgeLines.Add(l); });
            inEdgeLines.ForEach((l) => { keyPoints.Add(l.firstPoint); });
            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });
        }

        public override void CalculatePointsSet()
        {
            edgeLines.RemoveAll((l) => { return true; });

            for (int i = 0; i < keyPoints.Count - 1; i++)
            {
                edgeLines.Add(new CGUserGraphicsLine(keyPoints[i], keyPoints[i + 1]));
            }
            edgeLines.Add(new CGUserGraphicsLine(keyPoints[keyPoints.Count - 1], keyPoints[0]));

            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });
        }

        public override bool IsCursorNearby(Point cursorPos)
        {
            foreach(CGUserGraphicsLine line in edgeLines)
            {
                if (line.IsCursorNearby(cursorPos))
                    return true;
            }
            return false;
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                if (Distance.CalcDistance(oldPos, keyPoints[i]) < 3)
                {
                    int dx = newPos.X - oldPos.X;
                    int dy = newPos.Y - oldPos.Y;
                    Point newPoint = new Point(keyPoints[i].X + dx, keyPoints[i].Y + dy);
                    keyPoints[i] = newPoint;
                    int nextI = (i + 1) % keyPoints.Count;
                    int prevI = (i + keyPoints.Count - 1) % keyPoints.Count;
                    edgeLines[i] = new CGUserGraphicsLine(keyPoints[i], keyPoints[nextI]);
                    edgeLines[prevI] = new CGUserGraphicsLine(keyPoints[prevI], keyPoints[i]);
                }
            }

            pointsSet.RemoveAll((p) => { return true; });
            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });

            return this;
        }
    }

    public class CGUserGraphicsBlock : CGUserGraphics
    {
        public override bool IsCursorNearby(Point cursorPos)
        {
            return base.IsCursorNearby(cursorPos);
        }
    }

    public class CGUserGraphicsTinyRectangle : CGUserGraphics
    {
        const int edge = 2;

        public CGUserGraphicsTinyRectangle(Point center)
        {
            for (int i = center.X - edge; i <= center.X + edge; i++)
                pointsSet.Add(new Point(i, center.Y - edge));

            for (int i = center.X - edge; i <= center.X + edge; i++)
                pointsSet.Add(new Point(i, center.Y + edge));

            for (int i = center.Y - edge; i <= center.Y + edge; i++)
                pointsSet.Add(new Point(center.X - edge, i));

            for (int i = center.Y - edge; i <= center.Y + edge; i++)
                pointsSet.Add(new Point(center.X + edge, i));
        }
    }

    public class CGUserCanvas
    {
        private int canvasWidth, canvasHeight;
        public Bitmap bmp { get; }
        public bool isUserGraphicsSelected { get; set; }

        public CGUserGraphics selectedUserGraphics;

        public CGUserGraphics GraphicsJustCleared = null;
        private int[,] refCount;
        private List<CGUserGraphics> userGraphicsSet = new List<CGUserGraphics>();

        public CGUserGraphicsPoint basePoint { get; set; }
        public CGUserCanvas(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
            bmp = new Bitmap(width, height);
            refCount = new int[width, height];

            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    bmp.SetPixel(i, j, Color.White);
                }
            }

            basePoint = new CGUserGraphicsPoint(new Point(width / 2, height / 2));
        }

        public void SetBasePoint(Point p)
        {
            if (basePoint != null)
                UnsetBasePoint();

            basePoint = new CGUserGraphicsPoint(p);
            this.AddGraphics(basePoint);
            this.SetGraphicsSelected(basePoint);
        }

        public void UnsetBasePoint()
        {
            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(basePoint);
            basePoint = null;
        }

        private void SetPixel(Point pos)
        {
            if (pos.X < 0 || pos.X >= canvasWidth || pos.Y < 0 || pos.Y >= canvasHeight)
                return;

            bmp.SetPixel(pos.X, pos.Y, Color.Black);
            refCount[pos.X, pos.Y]++;
        }

        private void ClearPixel(Point pos)
        {
            if (pos.X < 0 || pos.X >= canvasWidth || pos.Y < 0 || pos.Y >= canvasHeight)
                return;

            refCount[pos.X, pos.Y]--;
            if (refCount[pos.X, pos.Y] <= 0)
            {
                bmp.SetPixel(pos.X, pos.Y, Color.White);
            }
        }

        private Point ClipPoint(Point pos)
        {
            if (pos.X < 0) pos.X = 0;
            if (pos.X >= canvasWidth) pos.X = canvasWidth - 1;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.Y >= canvasHeight) pos.Y = canvasHeight - 1;
            return pos;
        }

        // draw graphics but didn't add to graphics set
        private void DrawGraphics(CGUserGraphics userGraphics)
        {
            foreach (Point point in userGraphics.pointsSet)
            {
                SetPixel(point);
            }
        }

        // undraw graphics but didn't remove from graphics set
        private void UndrawGraphics(CGUserGraphics userGraphics)
        {
            // MessageBox.Show(userGraphics.ToString());
            foreach (Point point in userGraphics.pointsSet)
            {
                ClearPixel(point);
            }
        }

        // select graphics into set and draw it
        public void AddGraphics(CGUserGraphics userGraphics)
        {
            DrawGraphics(userGraphics);
            userGraphicsSet.Add(userGraphics);
        }

        // remove graphics from set and undraw it 
        public void RemoveGraphics(CGUserGraphics userGraphics)
        {
            UndrawGraphics(userGraphics);
            userGraphicsSet.Remove(userGraphics);
        }

        public void Coloring(Point st)
        {
            CGUserGraphicsBlock block = new CGUserGraphicsBlock();
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(st);
            while(q.Count > 0)
            {
                Point center = q.Dequeue();
                block.pointsSet.Add(center);
                block.keyPoints.Add(center);
                List<Point> neighs = new List<Point>() {
                    new Point(center.X + 1, center.Y),
                    new Point(center.X - 1, center.Y),
                    new Point(center.X, center.Y + 1),
                    new Point(center.X, center.Y - 1)
                    };

                neighs.ForEach((u)=> {
                    if (0 <= u.X && u.X < canvasWidth
                        && 0 <= u.Y && u.Y < canvasHeight
                        && refCount[u.X, u.Y] <= 0)
                    {
                        q.Enqueue(u);
                        SetPixel(u);
                    }
                });
            }

            userGraphicsSet.Add(block);
        }

        public void SetGraphicsSelected(CGUserGraphics userGraphics)
        {
            this.ClearStateOfSelectedGraphics();

            selectedUserGraphics = userGraphics;
            isUserGraphicsSelected = true;

            foreach (Point p in userGraphics.CalculateTagPoints())
            {
                this.DrawGraphics(new CGUserGraphicsTinyRectangle(p));
            }
        }

        public void ClearStateOfSelectedGraphics()
        {
            if (!isUserGraphicsSelected)
                return;

            foreach (Point p in selectedUserGraphics.CalculateTagPoints())
            {
                this.UndrawGraphics(new CGUserGraphicsTinyRectangle(p));
            }
            GraphicsJustCleared = selectedUserGraphics;
            selectedUserGraphics = null;
            isUserGraphicsSelected = false;
        }

        //return true if select success, return false otherwise
        public bool SelectGraphicsByCursor(Point cursorPos)
        {
            foreach (CGUserGraphics iterUserGraphics in userGraphicsSet)
            {
                if (iterUserGraphics.IsCursorNearby(cursorPos))
                {
                    this.SetGraphicsSelected(iterUserGraphics);
                    return true;
                }
            }

            // no one graphics selected: clear old selected graphics
            this.ClearStateOfSelectedGraphics();
            return false;
        }

        public void DeleteSelectedGraphics()
        {
            if (!isUserGraphicsSelected)
                return;

            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(GraphicsJustCleared);
        }

        private void UpdateSelectedGraphics(CGUserGraphics newUserGraphics)
        {
            this.AddGraphics(newUserGraphics);
            this.SetGraphicsSelected(newUserGraphics);

            selectedUserGraphics = newUserGraphics;
        }

        public void MoveSelectedGraphics(int dx, int dy)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint)
                return;

            DeleteSelectedGraphics();

            CGUserGraphics newUserGraphics = GraphicsJustCleared.TransformMove(dx, dy);

            UpdateSelectedGraphics(newUserGraphics);
        }

        public void RotateSelectedGraphics(Point oldPos, Point newPos)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint
                || basePoint == null)
                return;

            DeleteSelectedGraphics();

            CGUserGraphics newUserGraphics = GraphicsJustCleared.TransformRotation(new Point(basePoint.X, basePoint.Y), oldPos, newPos);

            UpdateSelectedGraphics(newUserGraphics);
        }


        public void AdjustGraphicsByCursor(Point oldPos, Point newPos)
        {
            if (!isUserGraphicsSelected)
                return;

            CGUserGraphics oldGraphics = selectedUserGraphics;

            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(oldGraphics);


            CGUserGraphics newGraphics = oldGraphics.TransformAdjust(oldPos, newPos);

            this.AddGraphics(newGraphics);
            this.SetGraphicsSelected(newGraphics);

            selectedUserGraphics = newGraphics;
        }
    }
}
