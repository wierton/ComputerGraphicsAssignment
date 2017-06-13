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
    public class Line
    {
        Point a, b;

        public Line(Point x, Point y) { a = x; b = y; }
    }
    public class Vector
    {
        public double X, Y;

        public Vector(Point p) { X = p.X; Y = p.Y; }
        public Vector(double x, double y) { X = x; Y = y; }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }
        public static Vector operator +(int off, Vector b)
        {
            return new Vector(off + b.X, off + b.Y);
        }
        public static Vector operator +(Vector a, int off)
        {
            return new Vector(a.X + off, a.Y + off);
        }
        public static Vector operator -(int off, Vector b)
        {
            return new Vector(off - b.X, off - b.Y);
        }
        public static Vector operator -(Vector a, int off)
        {
            return new Vector(a.X - off, a.Y - off);
        }
        public static Vector operator *(double factor, Vector b)
        {
            return new Vector(factor * b.X, factor * b.Y);
        }
        public static Vector operator *(Vector a, double factor)
        {
            return new Vector(a.X * factor, a.Y * factor);
        }
        public static Vector operator /(double factor, Vector b)
        {
            return new Vector(factor / b.X, factor / b.Y);
        }
        public static Vector operator /(Vector a, double factor)
        {
            return new Vector(a.X / factor, a.Y / factor);
        }
    }

    public class Distance
    {
        public static double CalcDistance(Point x, Point y)
        {
            int dx = x.X - y.X;
            int dy = x.Y - y.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static double CalcVecDistance(Vector x, Vector y)
        {
            double dx = x.X - y.X;
            double dy = x.Y - y.Y;
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

        public Theta(Point start, Point end)
        {
            Point vec = new Point(end.X - start.X, end.Y - start.Y);
            Theta theta = new Theta(vec);
            sinTheta = theta.sinTheta;
            cosTheta = theta.cosTheta;
        }

        public Theta(Point vec)
        {
            // old vec = (1, 0), new vec = vector

            if (vec.X == 0 && vec.Y == 0)
                isThetaValid = false;
            else
                isThetaValid = true;

            double vecMod = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);

            cosTheta = vec.X / vecMod;

            sinTheta = vec.Y / vecMod;
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

    public class ZoomFactor
    {
        double sx, sy;
        bool isFactorValid = true;

        public ZoomFactor(Point basePos, Point oldPos, Point newPos)
        {
            Point oldVec = new Point(oldPos.X - basePos.X, oldPos.Y - basePos.Y);
            Point newVec = new Point(newPos.X - basePos.X, newPos.Y - basePos.Y);

            if (oldVec.X == 0 || oldVec.Y == 0)
            {
                isFactorValid = false;
                return;
            }

            sx = newVec.X / (double)oldVec.X;
            sy = newVec.Y / (double)oldVec.Y;
        }

        public Point ZoomPoint(Point p, Point basePos)
        {
            if (isFactorValid == false)
                return p;

            return new Point(
                (int)(basePos.X + sx * (p.X - basePos.X)),
                (int)(basePos.Y + sy * (p.Y - basePos.Y))
                );
        }
    }

    public class CGUserGraphics
    {
        public List<Point> pointsSet { get; } = new List<Point>();

        public List<Point> keyPoints = new List<Point>();

        public virtual void CalculatePointsSet()
        {
            keyPoints.ForEach((p) => { pointsSet.Add(p); });
        }

        public void UpdatePointsSet()
        {
            pointsSet.RemoveAll(p => true);
            CalculatePointsSet();
        }

        public virtual bool IsCursorNearby(Point cursorPos)
        {
            foreach (Point p in pointsSet)
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

        public virtual CGUserGraphics TransformZoom(Point basePos, Point oldPos, Point newPos)
        {
            ZoomFactor zf = new ZoomFactor(basePos, oldPos, newPos);
            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                keyPoints[i] = zf.ZoomPoint(keyPoints[i], basePos);
            }
            UpdatePointsSet();
            return this;
        }

        public virtual List<CGUserGraphics> TransformTrim(Rectangle rect)
        {
            return new List<CGUserGraphics>() { this };
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
                case "ComputerGraphicsWork.CGUserGraphicsPointset":
                    graphics = new CGUserGraphicsPointset(this.keyPoints);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsLine":
                    graphics = new CGUserGraphicsLine(this.keyPoints[0], this.keyPoints[1]);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsRectangle":
                    graphics = new CGUserGraphicsRectangle(this.keyPoints);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsCircle":
                    graphics = new CGUserGraphicsCircle(this.keyPoints[0], this.keyPoints[1]);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsEllipse":
                    CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse(this.keyPoints[0], this.keyPoints[1]);
                    ellipse.keyPoints.RemoveAll(p => true);
                    this.keyPoints.ForEach((p) => { ellipse.keyPoints.Add(p); });
                    ellipse.UpdatePointsSet();
                    graphics = ellipse;
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsPolygon":
                    graphics = new CGUserGraphicsPolygon(this.keyPoints);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsBezier":
                    graphics = new CGUserGraphicsBezier(this.keyPoints);
                    break;
                case "ComputerGraphicsWork.CGUserGraphicsBlock":
                    graphics = new CGUserGraphicsBlock(this.keyPoints);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("undocumented graphics:{0}", this.GetType().ToString()));
                    break;
            }
            return graphics;
        }

        public bool ClipTest(float p, float q, ref float u1, ref float u2)
        {
            if(p > 0)
            {
                float r = q / p;
                if (u1 > r)
                    return false;
                else if (u2 > r)
                    u2 = r;
            }
            else if(p < 0)
            {
                float r = q / p;
                if (u2 < r)
                    return false;
                else if (u1 < r)
                    u1 = r;
            }
            else
            {
                if (q < 0)
                    return false;
            }
            return true;
        }

        public Point[] ClipLine(Point st, Point ed, Rectangle rect)
        {
            int x = st.X;
            int y = st.Y;

            int dx = ed.X - st.X;
            int dy = ed.Y - st.Y;

            int x_min = rect.X;
            int x_max = rect.X + rect.Width - 1;
            int y_min = rect.Y;
            int y_max = rect.Y + rect.Height - 1;

            int[] p = new int[4];
            int[] q = new int[4];

            p[0] = -dx; q[0] = x - x_min;
            p[1] = +dx; q[1] = x_max - x;
            p[2] = -dy; q[2] = y - y_min;
            p[3] = +dy; q[3] = y_max - y;

            float u1 = 0, u2 = 1;
            
            if (ClipTest(p[0], q[0], ref u1, ref u2)
                && ClipTest(p[1], q[1], ref u1, ref u2)
                && ClipTest(p[2], q[2], ref u1, ref u2)
                && ClipTest(p[3], q[3], ref u1, ref u2))
            {
                // MessageBox.Show(String.Format("{0}, {1}", u1, u2));
                if (u1 > 0.0)
                {
                    st.X = (int)(x + dx * u1 + 0.5);
                    st.Y = (int)(y + dy * u1 + 0.5);
                }

                if(u2 < 1.0)
                {
                    ed.X = (int)(x + dx * u2 + 0.5);
                    ed.Y = (int)(y + dy * u2 + 0.5);
                }
            }
            else
            {
                return new Point[] { };
            }

            return new Point[2] { st, ed };
        }

        public String Format(List<Point> lps)
        {
            String ret = "";
            foreach (Point p in lps)
            {
                ret += String.Format("{0},", p);
            }
            return ret;
        }
    }


    public class CGUserGraphicsPointset : CGUserGraphics
    {
        public CGUserGraphicsPointset(List<Point> lps)
        {
            lps.ForEach((p) => { keyPoints.Add(p); });

            CalculatePointsSet();
        }

        public override void CalculatePointsSet()
        {
            keyPoints.ForEach((p) => { pointsSet.Add(p); });
        }

        public override List<Point> CalculateTagPoints()
        {
            return new List<Point>(){};
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

            pointsSet.RemoveAll(p => true);
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

        public override List<CGUserGraphics> TransformTrim(Rectangle rect)
        {
            Point[] ps = ClipLine(keyPoints[0], keyPoints[1], rect);

            if (ps.Count() > 0)
                return new List<CGUserGraphics>() {
                    new CGUserGraphicsLine(ps[0], ps[1])
                };
            else
                return new List<CGUserGraphics>() { };
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

                pointsSet.RemoveAll(p => true);
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
            edgeLines.RemoveAll(p => true);

            for (int i = 0; i < keyPoints.Count - 1; i++)
            {
                edgeLines.Add(new CGUserGraphicsLine(keyPoints[i], keyPoints[i + 1]));
            }
            edgeLines.Add(new CGUserGraphicsLine(keyPoints.Last(), keyPoints.First()));

            foreach(CGUserGraphicsLine l in edgeLines)
            {
                if(l.pointsSet[0] == l.firstPoint)
                {
                    for (int i = 0; i < l.pointsSet.Count; i++)
                    {
                        pointsSet.Add(l.pointsSet[i]);
                    }
                }
                else
                {
                    for (int i = l.pointsSet.Count - 1; i >= 0; i--)
                    {
                        pointsSet.Add(l.pointsSet[i]);
                    }
                }
            }

            /*
            for(int i = 0; i < pointsSet.Count - 1; i++)
            {
                if (Math.Abs(pointsSet[i].X - pointsSet[i + 1].X) > 1
                    && Math.Abs(pointsSet[i].Y - pointsSet[i + 1].Y) > 1)
                    MessageBox.Show("cnsmdf");
            }
            */
            //edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });
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

            pointsSet.RemoveAll(p => true);
            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });

            return this;
        }

        public bool Contains(Point p)
        {
            int i, j;
            bool c = false;
            for (i = 0, j = keyPoints.Count - 1; i < keyPoints.Count; j = i++)
            {
                if (((keyPoints[i].Y > p.Y) != (keyPoints[j].Y > p.Y)) &&
                    (p.X < (keyPoints[j].X - keyPoints[i].X) * (p.Y - keyPoints[i].Y) / (float)(keyPoints[j].Y - keyPoints[i].Y) + keyPoints[i].X))
                    c = !c;
            }
            return c || pointsSet.Contains(p);
        }

        public bool IsPointOnEdge(Point pos)
        {
            return pointsSet.Contains(pos);
        }

        public bool IsPointOnCorner(Point pos)
        {
            return keyPoints.Contains(pos);
        }


        bool IsAnticlockwise(ref List<Point> lps)
        {
            if (lps.Count < 3)
                return false;

            int n = lps.Count;

            int s = lps[0].Y * (lps[n - 1].X - lps[1].X);

            for (int i = 1; i < n; ++i)
                s += lps[i].Y * (lps[i - 1].X - lps[(i + 1) % n].X);

            return s > 0;
        }

        List<Point> SortPointByAnticlockwise(List<Point> lps)
        {
            lps = new List<Point>(lps);
            if (!IsAnticlockwise(ref lps))
                lps.Reverse();
            return lps;
        }
        public enum PointType
        {
            Normal,
            Enter,
            Leave,
            Used,
        };

        public class Node
        {
            public Point p;
            public PointType pt;
            public Node next;
            public Node prev;
            public Node cross;

            public Node() { }
            public Node(Point inP, PointType inPt) { p = inP; pt = inPt; }
        }

        public class LinkedList
        {
            public Node head, tail;
            public LinkedList() { }
            public void Add(Node node)
            {
                if (tail != null)
                {
                    tail.next = node;
                    node.prev = tail;
                    tail = node;
                }
                else
                {
                    head = tail = node;
                }

                tail.next = head;
                head.prev = tail;
            }

            public void Remove(Node node)
            {
                Node prev = node.prev;
                prev.next = node.next;
            }
        }

        UserLog log = new UserLog("log2.txt");

        public override List<CGUserGraphics> TransformTrim(Rectangle rect)
        {
            if (keyPoints.Count == 0)
            {
                return new List<CGUserGraphics>();
            }
            else if (keyPoints.Count == 1)
            {
                if (rect.Contains(keyPoints[0]))
                    return new List<CGUserGraphics>() { new CGUserGraphicsPoint(keyPoints[0]) };
                else
                    return new List<CGUserGraphics>();
            }
            else if (keyPoints.Count == 2)
            {
                Point[] ps = ClipLine(keyPoints[0], keyPoints[1], rect);
                return new List<CGUserGraphics>() { new CGUserGraphicsLine(ps[0], ps[1]) };
            }

            List<Point> plg = new List<Point>();
            List<PointType> plgt = new List<PointType>();

            List<Point> rtg = new List<Point>();
            List<PointType> rtgt = new List<PointType>();

            List<Point> lps = SortPointByAnticlockwise(keyPoints);
            for(int i = 0, j = lps.Count - 1; i < lps.Count; j = i++)
            {
                // line : j --> i
                plg.Add(lps[j]);
                plgt.Add(PointType.Normal);

                Point[] ps = ClipLine(lps[j], lps[i], rect);

                if(ps.Count() >= 1 && ps[0] != lps[j])
                {
                    plg.Add(ps[0]);
                    if (rect.Contains(lps[j]))
                    {
                        plgt.Add(PointType.Leave);
                    }
                    else
                    {
                        plgt.Add(PointType.Enter);
                    }
                }
                if (ps.Count() >= 2 && ps[1] != lps[i])
                {
                    plg.Add(ps[1]);
                    plgt.Add(PointType.Leave);
                }
            }

            log.write("===============start=================");
            for(int i = 0; i < plg.Count; i++)
            {
                log.write(String.Format("({0}, {1}, {2})", plg[i].X, plg[i].Y, plgt[i]));
            }

            log.write("\n");

            lps = new List<Point>()
            {
                new Point(rect.X, rect.Y),
                new Point(rect.X + rect.Width - 1, rect.Y),
                new Point(rect.X + rect.Width - 1, rect.Y + rect.Height - 1),
                new Point(rect.X, rect.Y + rect.Height - 1),
            };

            for (int i = 0, j = lps.Count - 1; i < lps.Count; j = i++)
            {
                // edge : j --> i
                rtg.Add(lps[j]);
                rtgt.Add(PointType.Normal);

                if(lps[j].X == lps[i].X)
                {
                    List<Point> ps = plg.FindAll((p) => { return p.X == lps[j].X; });
                    ps.Sort((p, q)=> Math.Abs(p.Y - lps[j].Y) - Math.Abs(q.Y - lps[j].Y));

                    foreach(Point p in ps)
                    {
                        rtg.Add(p);
                        rtgt.Add(plgt[plg.IndexOf(p)]);
                    }
                }
                else if(lps[j].Y == lps[i].Y)
                {
                    List<Point> ps = plg.FindAll((p) => { return p.Y == lps[j].Y; });
                    ps.Sort((p, q) => Math.Abs(p.X - lps[j].X) - Math.Abs(q.X - lps[j].X));

                    foreach (Point p in ps)
                    {
                        rtg.Add(p);
                        rtgt.Add(plgt[plg.IndexOf(p)]);
                    }
                }
            }

            for (int i = 0; i < rtg.Count; i++)
            {
                log.write(String.Format("({0}, {1}, {2})", rtg[i].X, rtg[i].Y, rtgt[i]));
            }
            log.write("===============mid=================\n");


            List<CGUserGraphics> graphics = new List<CGUserGraphics>();

            int index = -1;
            do
            {
                try
                {
                    index = plgt.FindIndex(t => t == PointType.Enter);
                }
                catch
                {
                    index = -1;
                }

                if (index == -1)
                    break;


                List<Point> outPoints = new List<Point>();
                // points in stack
                Point st = plg[index];
                Point tp = st;
                // pointer
                lps = plg;
                List<PointType> lpt = plgt;
                // init
                outPoints.Add(st);
                log.write(String.Format("({0}, {1})", st.X, st.Y));
                do
                {
                    outPoints.Add(tp);
                    log.write(String.Format("({0}, {1})", tp.X, tp.Y));

                    int ti = lps.IndexOf(tp);
                    ti = (ti + 1) % lps.Count;
                    tp = lps[ti];

                    if(outPoints.Contains(tp))
                        break;

                    if (lpt[ti] == PointType.Enter)
                    {
                        log.write(String.Format("switch to polygon at ({0}, {1})", lps[ti].X, lps[ti].Y));
                        lps = plg;
                        lpt = plgt;
                    }
                    else if (lpt[ti] == PointType.Leave)
                    {
                        log.write(String.Format("switch to clip edge at ({0}, {1})", lps[ti].X, lps[ti].Y));
                        lps = rtg;
                        lpt = rtgt;
                    }
                    else
                    {
                        log.write(String.Format("keep at {2}:({0}, {1})", lps[ti].X, lps[ti].Y, lpt[ti]));
                    }
                } while (tp != st) ;

                foreach (Point p in outPoints)
                {
                    int ti = plg.IndexOf(p);
                    if (ti > 0)
                    {
                        plgt[ti] = PointType.Used;
                    }

                    ti = rtg.IndexOf(p);
                    if (ti > 0)
                    {
                        rtgt[ti] = PointType.Used;
                    }
                }

                log.write(String.Format("add polygon:{0}", Format(outPoints)));
                graphics.Add(new CGUserGraphicsPolygon(outPoints));
                log.write("===============next=================\n");
            } while (true);

            log.write("===============end=================\n");

            return graphics;
        }
    }

    public class CGUserGraphicsRectangle : CGUserGraphics
    {
        public CGUserGraphicsRectangle(Point st, Point ed)
        {
            int minX = Math.Min(st.X, ed.X);
            int minY = Math.Min(st.Y, ed.Y);
            int maxX = Math.Max(st.X, ed.X);
            int maxY = Math.Max(st.Y, ed.Y);

            keyPoints.Add(new Point(minX, minY));
            keyPoints.Add(new Point(minX, maxY));
            keyPoints.Add(new Point(maxX, minY));

            CalculatePointsSet();
        }

        public CGUserGraphicsRectangle(List<Point> lps)
        {
            lps.ForEach((p)=> { keyPoints.Add(p); });

            CalculatePointsSet();
        }

        public override void CalculatePointsSet()
        {
            CGUserGraphicsLine l;
            l = new CGUserGraphicsLine(keyPoints[0], keyPoints[1]);
            l.pointsSet.ForEach((p)=> { this.pointsSet.Add(p); });

            l = new CGUserGraphicsLine(keyPoints[0], keyPoints[2]);
            l.pointsSet.ForEach((p) => { this.pointsSet.Add(p); });

            Point tp = new Point(keyPoints[1].X + keyPoints[2].X - keyPoints[0].X,
                keyPoints[1].Y + keyPoints[2].Y - keyPoints[0].Y);

            l = new CGUserGraphicsLine(keyPoints[1], tp);
            l.pointsSet.ForEach((p) => { this.pointsSet.Add(p); });

            l = new CGUserGraphicsLine(keyPoints[2], tp);
            l.pointsSet.ForEach((p) => { this.pointsSet.Add(p); });
        }

        public override List<Point> CalculateTagPoints()
        {
            Point tp = new Point(keyPoints[1].X + keyPoints[2].X - keyPoints[0].X,
                keyPoints[1].Y + keyPoints[2].Y - keyPoints[0].Y);
            return new List<Point>(){
                keyPoints[0],
                keyPoints[1],
                keyPoints[2],
                tp
            };
        }

    }

    public class CGUserGraphicsBezier : CGUserGraphics
    {
        public Vector[,] pv;
        public CGUserGraphicsBezier()
        {

        }

        public CGUserGraphicsBezier(List<Point> inEndPoints)
        {
            inEndPoints.ForEach((u) => { keyPoints.Add(u); });
            CalculatePointsSet();
        }

        Vector CalculateBezierPointWithFactor(double factor)
        {
            for (int i = 1; i < keyPoints.Count; i++)
            {
                for (int j = 0; j < keyPoints.Count - i; j++)
                {
                    pv[i, j] = (1 - factor) * pv[i - 1, j] + factor * pv[i - 1, j + 1];
                }
            }
            return pv[keyPoints.Count - 1, 0];
        }

        void CalculateBezierPoint(double stf, Vector st, double edf, Vector ed)
        {

            double tf = (stf + edf) / 2;
            Vector v = CalculateBezierPointWithFactor(tf);
            pointsSet.Add(new Point((int)v.X, (int)v.Y));

            if (Math.Abs((int)st.X - (int)v.X) <= 1
                && Math.Abs((int)st.Y - (int)v.Y) <= 1
                && Math.Abs((int)v.X - (int)ed.X) <= 1
                && Math.Abs((int)v.Y - (int)ed.Y) <= 1)
                return;

            CalculateBezierPoint(stf, st, tf, v);
            CalculateBezierPoint(tf, v, edf, ed);
        }

        public override void CalculatePointsSet()
        {
            pv = new Vector[keyPoints.Count, keyPoints.Count];

            for (int i = 0; i < keyPoints.Count; i++)
            {
                pv[0, i] = new Vector(keyPoints[i]);
            }

            pointsSet.Add(new Point((int)pv[0, 0].X, (int)pv[0, 0].Y));
            pointsSet.Add(new Point((int)pv[0, keyPoints.Count - 1].X, (int)pv[0, keyPoints.Count - 1].Y));
            CalculateBezierPoint(0, pv[0, 0], 1, pv[0, keyPoints.Count - 1]);
        }
    }

    public class CGUserGraphicsBStyleCurve : CGUserGraphics
    {

    }

    public class CGUserGraphicsBlock : CGUserGraphics
    {
        public CGUserGraphicsBlock()
        {

        }
        public CGUserGraphicsBlock(List<Point> inPointSet)
        {
            inPointSet.ForEach((p) => { keyPoints.Add(p); });
            inPointSet.ForEach((p) => { pointsSet.Add(p); });
        }

        public override List<Point> CalculateTagPoints()
        {
            return new List<Point>();
        }
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
        public Bitmap bmp { get; }
        public bool isUserGraphicsSelected { get; set; }

        public CGUserGraphics selectedUserGraphics;

        public CGUserGraphics GraphicsJustCleared = null;
        private int[,] refCount;
        private List<CGUserGraphics> userGraphicsSet = new List<CGUserGraphics>();

        // transform
        Point posOfGraphicsWhenSelected;
        CGUserGraphics rawSelectedGraphics;
        CGUserGraphics transfromSelectedGraphics;

        public Rectangle trimArea { get; set; }
        private Rectangle clientRect;
        private CGUserGraphicsRectangle trimRectangle;

        public CGUserGraphicsPoint basePoint { get; set; }
        public CGUserCanvas(int width, int height)
        {
            clientRect = new Rectangle(0, 0, width, height);

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
            if (!clientRect.Contains(pos))
                return;

            bmp.SetPixel(pos.X, pos.Y, Color.Black);
            refCount[pos.X, pos.Y]++;
        }

        private void ClearPixel(Point pos)
        {
            if (!clientRect.Contains(pos))
                return;

            refCount[pos.X, pos.Y]--;
            if (refCount[pos.X, pos.Y] <= 0)
            {
                bmp.SetPixel(pos.X, pos.Y, Color.White);
            }
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
                    if (0 <= u.X && u.X < clientRect.Width
                        && 0 <= u.Y && u.Y < clientRect.Height
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
                    posOfGraphicsWhenSelected = cursorPos;
                    rawSelectedGraphics = iterUserGraphics;

                    this.SetGraphicsSelected(iterUserGraphics);
                    // this.userGraphicsSet.Remove(iterUserGraphics);
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
            // comment it when test rotation
            this.RemoveGraphics(GraphicsJustCleared);
        }

        private void UpdateSelectedGraphics(CGUserGraphics newUserGraphics)
        {
            this.AddGraphics(newUserGraphics);
            this.SetGraphicsSelected(newUserGraphics);

            selectedUserGraphics = newUserGraphics;
        }

        public void MoveSelectedGraphics(Point newPos)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint)
                return;

            DeleteSelectedGraphics();

            transfromSelectedGraphics = rawSelectedGraphics.Copy();
            transfromSelectedGraphics.TransformMove(newPos.X - posOfGraphicsWhenSelected.X,
                newPos.Y - posOfGraphicsWhenSelected.Y);

            UpdateSelectedGraphics(transfromSelectedGraphics);
        }

        public void ZoomSelectedGraphics(Point newPos)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint)
                return;

            DeleteSelectedGraphics();

            transfromSelectedGraphics = rawSelectedGraphics.Copy();
            transfromSelectedGraphics.TransformZoom(
                new Point(basePoint.X, basePoint.Y),
                posOfGraphicsWhenSelected,
                newPos);

            UpdateSelectedGraphics(transfromSelectedGraphics);
        }

        public void RotateSelectedGraphics(Point newPos)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint
                || basePoint == null)
                return;

            DeleteSelectedGraphics();

            transfromSelectedGraphics = rawSelectedGraphics.Copy();
            transfromSelectedGraphics.TransformRotation(
                new Point(basePoint.X, basePoint.Y),
                posOfGraphicsWhenSelected,
                newPos);

            UpdateSelectedGraphics(transfromSelectedGraphics);
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

        public void trimSelectedGraphics(Rectangle rect)
        {
            if (trimRectangle != null)
            {
                this.UndrawGraphics(trimRectangle);
                trimRectangle = null;
            }

            if (!isUserGraphicsSelected)
                return;

            CGUserGraphics oldGraphics = selectedUserGraphics;
            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(oldGraphics);
            List<CGUserGraphics> lg = oldGraphics.TransformTrim(rect);
            if (lg != null && lg.Count > 0)
            {
                for (int j = 0; j < lg.Count; j++)
                {
                    this.AddGraphics(lg[j]);
                }
            }
        }

        public void redrawAllGraphics()
        {
            Rectangle oldRect = clientRect;
            clientRect = new Rectangle(0, 0, clientRect.Width, clientRect.Height);

            userGraphicsSet.ForEach((g) => { this.UndrawGraphics(g); });

            clientRect = oldRect;
            userGraphicsSet.ForEach((g) => { this.DrawGraphics(g); });

        }

        public void adjustTrimArea(Point downPos, Point curPos)
        {
            int minX = Math.Max(0, Math.Min(downPos.X, curPos.X));
            int minY = Math.Max(0, Math.Min(downPos.Y, curPos.Y));
            int maxX = Math.Min(clientRect.Width - 1, Math.Max(downPos.X, curPos.X));
            int maxY = Math.Min(clientRect.Height - 1, Math.Max(downPos.Y, curPos.Y));


            if (trimRectangle != null)
            {
                this.UndrawGraphics(trimRectangle);
            }

            trimArea = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            trimRectangle = new CGUserGraphicsRectangle(new Point(minX, minY), new Point(maxX, maxY));
            this.DrawGraphics(trimRectangle);
        }
    }
}
