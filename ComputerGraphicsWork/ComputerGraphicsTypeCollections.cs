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
    public class Vector
    {
        public double X, Y;

        public Vector(Point p) { X = p.X; Y = p.Y; }
        public Vector(double x, double y) { X = x; Y = y; }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

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

    public class FixedVector
    {
        public Point st { get; }
        public Point ed { get; }
        public FixedVector() { }
        public FixedVector(Point inSt, Point inEd) { st = inSt; ed = inEd; }

        public bool isInside(Point p)
        {
            int v1_dx = ed.X - st.X;
            int v1_dy = ed.Y - st.Y;

            int v2_dx = p.X - st.X;
            int v2_dy = p.Y - st.Y;

            return (v1_dx * v2_dy - v2_dx * v1_dy) >= 0;
        }

        static UserLog log = new UserLog("log1.txt");

        public Point[] CalculateIntersectPoint(Point c, Point d)
        {
            int denominator = (ed.Y - st.Y) * (d.X - c.X) - (st.X - ed.X) * (c.Y - d.Y);
            if (denominator == 0)
            {
                log.write("denominator is 0");
                return new Point[0];
            }

            int X = ((ed.X - st.X) * (d.X - c.X) * (c.Y - st.Y)
                        + (ed.Y - st.Y) * (d.X - c.X) * st.X
                        - (d.Y - c.Y) * (ed.X - st.X) * c.X) / denominator;
            int Y = -((ed.Y - st.Y) * (d.Y - c.Y) * (c.X - st.X)
                        + (ed.X - st.X) * (d.Y - c.Y) * st.Y
                        - (d.X - c.X) * (ed.Y - st.Y) * c.Y) / denominator;

            if ( 
                 (X - c.X) * (X - d.X) <= 0 && (Y - c.Y) * (Y - d.Y) <= 0
               )
            {
                log.write(String.Format("intersect at ({0}, {1})", X, Y));
                return new Point[] { new Point(X, Y) };
            }

            log.write("intersect point not at line");
            return new Point[0];
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
                            MessageBox.Show(string.FormatPointListToString("invalid theta: sin={0}, cos={1}, z={2}",
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
        public double sx { get; }
        public double sy { get; }
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
        public bool isColored = false;
        public List<Point> pointsSet { get; } = new List<Point>();

        public List<Point> keyPoints = new List<Point>();

        public virtual void CalculatePointsSet()
        {
            keyPoints.ForEach((p) => { pointsSet.Add(p); });
        }

        public void UpdatePointsSet()
        {
            pointsSet.Clear();
            CalculatePointsSet();
        }

        public virtual bool IsCursorNearby(Point cursorPos)
        {
            foreach (Point p in pointsSet)
            {
                if (GetDistance(p, cursorPos) < 4)
                    return true;
            }

            return false;
        }

        public virtual List<Point> CalculateTagPoints()
        {
            return keyPoints;
        }

        public delegate Point TransformOperation(Point p);
        public CGUserGraphics BasicTransformFrame(TransformOperation btf)
        {
            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                keyPoints[i] = btf(keyPoints[i]);
            }
            UpdatePointsSet();

            if (isColored)
            {
                List<Point> lps = this.InternalPoints();
                lps.ForEach(p => { pointsSet.Add(p); });
            }

            return this;
        }

        public virtual CGUserGraphics TransformRotation(Point basePos, Point oldPos, Point newPos)
        {
            Theta theta = new Theta(basePos, oldPos, newPos);
            return BasicTransformFrame((p) => theta.RotatePoint(p, basePos));
        }

        public virtual CGUserGraphics TransformZoom(Point basePos, Point oldPos, Point newPos)
        {
            ZoomFactor zf = new ZoomFactor(basePos, oldPos, newPos);
            return BasicTransformFrame((p) => zf.ZoomPoint(p, basePos));
        }

        public virtual CGUserGraphics TransformMove(int dx, int dy)
        {
            return BasicTransformFrame((p) => new Point(p.X + dx, p.Y + dy));
        }

        public virtual CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            for (int i = keyPoints.Count - 1; i >= 0; i--)
            {
                if (GetDistance(oldPos, keyPoints[i]) < 3)
                {
                    int dx = newPos.X - oldPos.X;
                    int dy = newPos.Y - oldPos.Y;
                    Point newPoint = new Point(keyPoints[i].X + dx, keyPoints[i].Y + dy);
                    keyPoints[i] = newPoint;
                    break;
                }
            }

            UpdatePointsSet();

            if (isColored)
            {
                List<Point> lps = this.InternalPoints();
                lps.ForEach(p => { pointsSet.Add(p); });
            }

            return this;
        }

        public virtual List<CGUserGraphics> TransformTrim(Rectangle rect)
        {
            return new List<CGUserGraphics>() { this };
        }
        

        public virtual List<Point> InternalPoints()
        {
            return new List<Point>() { };
        }

        public CGUserGraphics InternalColoring()
        {
            isColored = true;
            List<Point> lps = InternalPoints();
            foreach(Point p in lps)
            {
                pointsSet.Add(p);
            }
            return this;
        }

        public CGUserGraphics Copy()
        {
            Type t = this.GetType();
            CGUserGraphics g = (CGUserGraphics)Activator.CreateInstance(t);

            this.keyPoints.ForEach(p => { g.keyPoints.Add(p); });

            g.CalculatePointsSet();
            g.isColored = this.isColored;

            return g;
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

        // other functions
        public Point GetMiddlePoint(Point a, Point b)
        {
            return new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        }

        public double GetDistance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
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
                // MessageBox.Show(String.FormatPointListToString("{0}, {1}", u1, u2));
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

        public Point[] CalculateCrossPointBetweenLineAndRectangle(Point st, Point ed, Rectangle rect)
        {
            Point[] ps = ClipLine(st, ed, rect);

            List<Point> ret = new List<Point>();

            if (ps.Count() >= 1 && ps[0] != st)
            {
                ret.Add(ps[0]);
            }
            if (ps.Count() >= 2 && ps[1] != ed)
            {
                ret.Add(ps[1]);
            }

            return ret.ToArray();
        }

        int get_area(Point a0, Point a1, Point a2)
        { 
            //求有向面积  
            return a0.X * a1.Y + a2.X * a0.Y + a1.X * a2.Y - a2.X * a1.Y - a0.X * a2.Y - a1.X * a0.Y;
        }

        public bool IsSegmentsIntersect(Point st1, Point ed1, Point st2, Point ed2)
        {
            int s1 = get_area(st1, ed1, st2);
            int s2 = get_area(st1, ed1, ed2);
            int s3 = get_area(st2, ed2, st1);
            int s4 = get_area(st2, ed2, ed1);
            if (s1 * s2 <= 0 && s3 * s4 <= 0)
                return true;
            else
                return false;
        }


        public String FormatPointListToString(List<Point> lps)
        {
            String ret = "";
            foreach (Point p in lps)
            {
                ret += String.Format("{0},", p);
            }
            return ret;
        }
    }


    public class CGUserGraphicsPoint : CGUserGraphics
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;

        public CGUserGraphicsPoint() { }
        public CGUserGraphicsPoint(Point start)
        {
            keyPoints.Add(start);
            CalculatePointsSet();
        }

        public override void CalculatePointsSet()
        {
            X = keyPoints[0].X;
            Y = keyPoints[0].Y;
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
        public CGUserGraphicsLine() { }
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

        public CGUserGraphicsCircle() { }
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

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            int dx = newPos.X - oldPos.X;
            int dy = newPos.Y - oldPos.Y;
            if (Math.Abs(GetDistance(keyPoints[0], oldPos) - radius) < 4)
            {
                int radius = (int)GetDistance(keyPoints[0], newPos);
                keyPoints[1] = new Point(keyPoints[0].X + radius, keyPoints[0].Y);

                pointsSet.Clear();
                CalculatePointsSet();
            }
            return this;
        }

        public override List<Point> InternalPoints()
        {
            Point center = keyPoints[0];
            Point edge = keyPoints[1];

            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;
            int squareRadius = dx * dx + dy * dy;
            radius = (int)Math.Sqrt((double)(squareRadius));

            List<Point> lps = new List<Point>();

            for(int i = center.X - radius; i <= center.X + radius; i++)
            {
                for(int j = center.Y - radius; j <= center.Y + radius; j++)
                {
                    int squareDist = (i - center.X) * (i - center.X) + (j - center.Y) * (j - center.Y);
                    if (squareDist < squareRadius)
                        lps.Add(new Point(i, j));
                }
            }

            return lps;
        }

        public override CGUserGraphics TransformZoom(Point basePos, Point oldPos, Point newPos)
        {
            ZoomFactor zf = new ZoomFactor(basePos, oldPos, newPos);



            if(Math.Abs(zf.sx -  zf.sy) < 1e-6)
            {
                return base.TransformZoom(basePos, oldPos, newPos);
            }
            else
            {
                int dx = keyPoints[1].X - keyPoints[0].X;
                int dy = keyPoints[1].Y - keyPoints[0].Y;

                int d = (int)(Math.Sqrt(dx * dx + dy * dy) + 0.5);

                CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse();
                ellipse.keyPoints.Add(new Point(keyPoints[0].X - d, keyPoints[0].Y - d));
                ellipse.keyPoints.Add(new Point(keyPoints[0].X + d, keyPoints[0].Y + d));
                ellipse.keyPoints.Add(new Point(keyPoints[0].X + d, keyPoints[0].Y - d));

                ellipse.CalculatePointsSet();

                return ellipse;
            }
        }
    }
    public class CGUserGraphicsEllipse : CGUserGraphics
    {
        public CGUserGraphicsEllipse() { }
        /* p[0]          O           p[2]
         * 
         * O             O            O
         * 
         *               O           p[1]
         * */
        public CGUserGraphicsEllipse(Point p0, Point p1)
        {
            keyPoints.Add(p0);
            keyPoints.Add(p1);
            keyPoints.Add(new Point(p1.X, p0.Y));
            CalculatePointsSet();
        }

        public List<Point> CalculateNormalInternalPoints(Point center, Point edge)
        {
            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;

            int xRadius = Math.Abs(dx);
            int yRadius = Math.Abs(dy);

            int xRadiusSquare = xRadius * xRadius;
            int yRadiusSquare = yRadius * yRadius;

            int squareXYRadius = xRadiusSquare * yRadiusSquare;

            List<Point> lps = new List<Point>();

            for (int i = center.X - xRadius; i <= center.X + xRadius; i++)
            {
                for (int j = center.Y - yRadius; j <= center.Y + yRadius; j++)
                {
                    int squareDist = yRadiusSquare * (i - center.X) * (i - center.X) + xRadiusSquare * (j - center.Y) * (j - center.Y);
                    if (squareDist < squareXYRadius)
                        lps.Add(new Point(i, j));
                }
            }

            return lps;
        }

        public override List<Point> InternalPoints()
        {
            Point center = GetMiddlePoint(keyPoints[0], keyPoints[1]);
            if (keyPoints[2].X == keyPoints[1].X && keyPoints[2].Y == keyPoints[0].Y)
            {
                return CalculateNormalInternalPoints(center, keyPoints[1]);
            }
            else
            {
                Point upPoint = GetMiddlePoint(keyPoints[0], keyPoints[2]);
                Point rightPoint = GetMiddlePoint(keyPoints[2], keyPoints[1]);

                int yRadius = (int)GetDistance(upPoint, center);
                int xRadius = (int)GetDistance(rightPoint, center);

                Point fixedUpPoint = new Point(center.X, center.Y + yRadius);
                Point fixedEdgePoint = new Point(center.X + xRadius, center.Y + yRadius);

                List<Point> lps = CalculateNormalInternalPoints(center, fixedEdgePoint);

                Theta theta = new Theta(center, fixedUpPoint, upPoint);

                for (int i = lps.Count - 1; i >= 0; i--)
                {
                    lps[i] = theta.RotatePoint(lps[i], center);
                }
                return lps;
            }
        }

        public override void CalculatePointsSet()
        {
            Point center = GetMiddlePoint(keyPoints[0], keyPoints[1]);
            if (keyPoints[2].X == keyPoints[1].X && keyPoints[2].Y == keyPoints[0].Y)
            {
                CalculateNormalEllipse(center, keyPoints[1]);
            }
            else
            {
                Point upPoint = GetMiddlePoint(keyPoints[0], keyPoints[2]);
                Point rightPoint = GetMiddlePoint(keyPoints[2], keyPoints[1]);

                int yRadius = (int)GetDistance(upPoint, center);
                int xRadius = (int)GetDistance(rightPoint, center);

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

        // @param(1) : center
        // @param(2) : edge
        public void CalculateNormalEllipse(Point center, Point edge)
        {
            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;

            int xRadius = Math.Abs(dx);
            int yRadius = Math.Abs(dy);

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
                if (GetDistance(cursorPos, p) < 4.0)
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
                GetMiddlePoint(keyPoints[0], keyPoints[2]),
                GetMiddlePoint(keyPoints[2], keyPoints[1]),
                GetMiddlePoint(keyPoints[1], p3),
                GetMiddlePoint(p3, keyPoints[0]),
            };
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            return this;
        }
    }

    public class CGUserGraphicsPolygon : CGUserGraphics
    {
        public List<CGUserGraphicsLine> edgeLines { get; } = new List<CGUserGraphicsLine>();

        public CGUserGraphicsPolygon() { }

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
            edgeLines.Clear();

            if (keyPoints.Count == 0)
                return;


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

        bool IsAnyTwoLineCross()
        {
            for (int j = 2; j < edgeLines.Count - 1; j++)
            {
                if (IsSegmentsIntersect(edgeLines[0].firstPoint, edgeLines[0].nextPoint,
                    edgeLines[j].firstPoint, edgeLines[j].nextPoint))
                {
                    log.write(String.Format("cross at {8}:{9} ({0}, {1})-({2}, {3})  ^  ({4}, {5})-({6}, {7})",
                        edgeLines[0].firstPoint.X, edgeLines[0].firstPoint.Y,
                        edgeLines[0].nextPoint.X, edgeLines[0].nextPoint.Y,
                        edgeLines[j].firstPoint.X, edgeLines[j].firstPoint.Y,
                        edgeLines[j].nextPoint.X, edgeLines[j].nextPoint.Y,
                        0, j
                    ));
                    return true;
                }
            }

            for (int i = 1; i < edgeLines.Count; i++)
            {
                for(int j = i + 2; j < edgeLines.Count; j++)
                {
                    if(IsSegmentsIntersect(edgeLines[i].firstPoint, edgeLines[i].nextPoint,
                        edgeLines[j].firstPoint, edgeLines[j].nextPoint))
                    {
                        log.write(String.Format("cross at {8}:{9} ({0}, {1})-({2}, {3})  ^  ({4}, {5})-({6}, {7})",
                            edgeLines[i].firstPoint.X, edgeLines[i].firstPoint.Y,
                            edgeLines[i].nextPoint.X, edgeLines[i].nextPoint.Y,
                            edgeLines[j].firstPoint.X, edgeLines[j].firstPoint.Y,
                            edgeLines[j].nextPoint.X, edgeLines[j].nextPoint.Y,
                            i, j
                        ));
                        return true;
                    }
                }
            }
            return false;
        }

        List<Point> SortPointByAnticlockwise(List<Point> lps)
        {
            lps = new List<Point>(lps);
            if (!IsAnticlockwise(ref lps))
                lps.Reverse();
            return lps;
        }

        // trim polygon
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


        static UserLog log = new UserLog("log2.txt");

        public override List<CGUserGraphics> TransformTrim(Rectangle rect)
        {
            // corner cases
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


            if (IsAnyTwoLineCross())
                return TransformTrimBySutherlandHodgman(rect);
            else
                return TransformTrimByWeilerrAtherton(rect);
        }

        public List<CGUserGraphics> TransformTrimBySutherlandHodgman(Rectangle rect)
        {
            Point[] edgeVertexs = new Point[]
            {
                new Point(rect.X, rect.Y),
                new Point(rect.X + rect.Width - 1, rect.Y),
                new Point(rect.X + rect.Width - 1, rect.Y + rect.Height - 1),
                new Point(rect.X, rect.Y + rect.Height - 1),
            };

            FixedVector[] edges = new FixedVector[] {
                new FixedVector(edgeVertexs[0], edgeVertexs[1]),
                new FixedVector(edgeVertexs[1], edgeVertexs[2]),
                new FixedVector(edgeVertexs[2], edgeVertexs[3]),
                new FixedVector(edgeVertexs[3], edgeVertexs[0]),
            };

            List<Point> outPoints = new List<Point>(keyPoints);
            log.write("SutherlandHodgman start");

            foreach (var edge in edges)
            {
                log.write("\n=====new iteration=====");
                log.write(String.Format("edge: ({0}, {1}) -> ({2}, {3})", edge.st.X, edge.st.Y, edge.ed.X, edge.ed.Y));
                List<Point> lps = outPoints;
                log.write(String.Format("lps:{0}: {1}", lps.Count, FormatPointListToString(lps)));
                outPoints = new List<Point>();

                bool isPrevVertexInRect = edge.isInside(lps.Last());

                for(int j = 0, i = lps.Count - 1; j < lps.Count; i = j++)
                {
                    log.write(String.Format("---({0} -> {1})---", i, j));
                    log.write(String.Format("i point: ({0}, {1})", lps[i].X, lps[i].Y));
                    log.write(String.Format("j point: ({0}, {1})", lps[j].X, lps[j].Y));
                    if (edge.isInside(lps[j]))
                    {
                        log.write("point j is inside");
                        // j in
                        if (!isPrevVertexInRect)
                        {
                            // i not in, j in
                            Point[] ps = edge.CalculateIntersectPoint(lps[i], lps[j]);
                            log.write(String.Format("i not in, j in, intersect points:{0}", ps.Count()));
                            foreach (Point p in ps)
                            {
                                log.write(String.Format("add point: ({0}, {1})", p.X, p.Y));
                                outPoints.Add(p);
                            }
                        }

                        log.write(String.Format("add point: ({0}, {1})", lps[j].X, lps[j].Y));

                        outPoints.Add(lps[j]);

                        isPrevVertexInRect = true;
                    }
                    else
                    {
                        log.write("point j is not inside");

                        // j not in
                        // two cases are same
                        Point[] ps = edge.CalculateIntersectPoint(lps[i], lps[j]);
                        foreach (Point p in ps)
                        {
                            log.write(String.Format("add point: ({0}, {1})", p.X, p.Y));
                            outPoints.Add(p);
                        }

                        isPrevVertexInRect = false;
                    }

                }

                if (outPoints.Count == 0)
                    break;
            }

            return new List<CGUserGraphics>() { new CGUserGraphicsPolygon(outPoints) };
        }

        public List<CGUserGraphics> TransformTrimByWeilerrAtherton(Rectangle rect)
        {
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

                log.write(String.Format("add polygon:{0}", FormatPointListToString(outPoints)));
                graphics.Add(new CGUserGraphicsPolygon(outPoints));
                log.write("===============next=================\n");
            } while (true);

            log.write("===============end=================\n");

            return graphics;
        }

        public enum CrossPointType
        {
            Normal,
            Vertex,
        }

        class CrossPoint
        {
            public Point p;
            public int edy { get; }

            int probe;
            int dx;
            int dy;
            int inc;

            public delegate void CalculateNextPointFunction();
            public CalculateNextPointFunction CalculateNextPoint;

            public CrossPoint(Point st, Point ed)
            {
                p = st;
                probe = 0;
                dx = ed.X - st.X;
                dy = ed.Y - st.Y;

                edy = ed.Y;

                if(dx > 0)
                {
                    inc = 1;
                }
                else if(dx < 0)
                {
                    dx = -dx;
                    inc = -1;
                }
                else
                {
                    inc = 0;
                }

                if (dy > dx)
                    CalculateNextPoint = CalculateNextPointByX;
                else
                {
                    probe = -2 * dx;
                    CalculateNextPoint = CalculateNextPointByY;
                }
            }

            public void CalculateNextPointByY()
            {
                while (true)
                {
                    probe += 2 * dy;
                    p.X += inc;
                    if (probe >= dx)
                    {
                        p.Y ++;
                        probe -= 2 * dx;
                        break;
                    }
                }
            }


            public void CalculateNextPointByX()
            {
                probe += 2 * dx;
                if (probe >= dy)
                {
                    p.X += inc;
                    probe -= 2 * dy;
                }

                p.Y++;
            }
        }

        class PointPair
        {
            public bool isEnd = false;
            public Point v;
            public Point next;
            public PointPair(Point inV, Point inNext) { v = inV; next = inNext; }
            public PointPair(Point inV) { v = inV; isEnd = true; }
        }
        class NodeList
        {
            public List<PointPair> vertex = new List<PointPair>();
        }

        public override List<Point> InternalPoints()
        {
            if (keyPoints.Count < 3)
                return new List<Point>() { };

            int maxY = -2147483647;
            int minY = 2147483647;

            Point topVertex = new Point();
            foreach(Point p in keyPoints)
            {
                if (p.Y < minY)
                {
                    topVertex = p;
                    minY = p.Y;
                }

                if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            log.write(String.Format("minY:{0}, maxY:{1}", minY, maxY));

            NodeList[] lpa = new NodeList[maxY - minY + 1];

            for(int i = 0; i < lpa.Count(); i++)
            {
                lpa[i] = new NodeList();
            }

            log.write("init left nodelist");
            for(int i = 0, prevI = keyPoints.Count - 1; i < keyPoints.Count; prevI = i++)
            {
                bool isEnd = true;
                int curY = keyPoints[i].Y;
                int nextI = (i + 1) % keyPoints.Count;

                if(keyPoints[prevI].Y > curY)
                {
                    log.write(String.Format("add point pair into lpa, ({0}, {1}) -> ({2}, {3})", 
                        keyPoints[i].X, keyPoints[i].Y, keyPoints[prevI].X, keyPoints[prevI].Y));
                    lpa[curY - minY].vertex.Add(new PointPair(keyPoints[i], keyPoints[prevI]));
                    isEnd = false;
                }

                if (keyPoints[nextI].Y > curY)
                {
                    log.write(String.Format("add point pair into lpa, ({0}, {1}) -> ({2}, {3})",
                        keyPoints[i].X, keyPoints[i].Y, keyPoints[nextI].X, keyPoints[nextI].Y));
                    lpa[curY - minY].vertex.Add(new PointPair(keyPoints[i], keyPoints[nextI]));
                    isEnd = false;
                }

                if(isEnd)
                {
                    log.write(String.Format("add end point into lpa, ({0}, {1})",
                        keyPoints[i].X, keyPoints[i].Y));
                    lpa[curY - minY].vertex.Add(new PointPair(keyPoints[i]));
                }
            }

            log.write("init cross points");
            List<CrossPoint> lcp = new List<CrossPoint>();

            foreach(PointPair pp in lpa[0].vertex)
            {
                if (!pp.isEnd)
                {
                    log.write(String.Format("add start point pair, ({0}, {1}) -> {2}, {3}",
                        pp.v.X, pp.v.Y, pp.next.X, pp.next.Y));
                    lcp.Add(new CrossPoint(pp.v, pp.next));
                }
            }


            List<Point> lps = new List<Point>();
            for (int y = minY + 1; y <= maxY; y++)
            {
                lcp.Sort((u, v)=> { return u.p.X - v.p.X; });

                for (int j = 0; j < lcp.Count; j += 2)
                {
                    if (j + 1 >= lcp.Count)
                        break;
                    for (int x = lcp[j].p.X; x <= lcp[j + 1].p.X; x++)
                    {
                        lps.Add(new Point(x, y));
                    }
                }

                // calculate next point in lcp
                for(int j = 0; j < lcp.Count; j++)
                {
                    lcp[j].CalculateNextPoint();
                }

                // remove end point in cross points
                for (int i = lcp.Count - 1; i >= 0; i--)
                {
                    if (lcp[i].p.Y >= lcp[i].edy)
                    {
                        lcp.RemoveAt(i);
                    }
                }

                foreach(PointPair pp in lpa[y - minY].vertex)
                {
                    if (pp.isEnd)
                        continue;

                    lcp.Add(new CrossPoint(pp.v, pp.next));
                }

                log.write(String.Format("calculate cross point at y={0}", y));
                for (int i = 0; i < lcp.Count; i++)
                {
                    log.write(String.Format("({0}, {1})", lcp[i].p.X, lcp[i].p.Y));
                }
            }

            return lps;
        }
    }

    public class CGUserGraphicsRectangle : CGUserGraphicsPolygon
    {
        public CGUserGraphicsRectangle() { }
        public CGUserGraphicsRectangle(Point st, Point ed)
        {
            int minX = Math.Min(st.X, ed.X);
            int minY = Math.Min(st.Y, ed.Y);
            int maxX = Math.Max(st.X, ed.X);
            int maxY = Math.Max(st.Y, ed.Y);

            keyPoints.Add(new Point(minX, minY));
            keyPoints.Add(new Point(minX, maxY));
            keyPoints.Add(new Point(maxX, maxY));
            keyPoints.Add(new Point(maxX, minY));
            
            base.CalculatePointsSet();
        }
    }

    public class CGUserGraphicsBezier : CGUserGraphics
    {
        public Vector[,] pv;
        public CGUserGraphicsBezier() { }

        public CGUserGraphicsBezier(List<Point> inEndPoints)
        {
            inEndPoints.ForEach((u) => { keyPoints.Add(u); });
            CalculatePointsSet();
        }

        private Vector CalculateBezierPointWithFactor(double factor)
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

        private void CalculateBezierPoint(double stf, Vector st, double edf, Vector ed)
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


        public override bool IsCursorNearby(Point cursorPos)
        {
            foreach(Point p in keyPoints)
            {
                if (GetDistance(p, cursorPos) < 4)
                    return true;
            }
            return base.IsCursorNearby(cursorPos);
        }
    }

    public class CGUserGraphicsBStyleCurve : CGUserGraphics
    {
        public const int K = 3;
        public CGUserGraphicsBStyleCurve() { }

        public CGUserGraphicsBStyleCurve(List<Point> inEndPoints)
        {
            inEndPoints.ForEach((u) => { keyPoints.Add(u); });
            CalculatePointsSet();
        }

        private Vector CalculateBStylePointWithFactor(double t, ref Vector[] pv)
        {
            double it = 1 - t;

            double b0 = it * it * it / 6.0f;

            double b1 = (3 * t * t * t - 6 * t * t + 4) / 6.0f;

            double b2 = (-3 * t * t * t + 3 * t * t + 3 * t + 1) / 6.0f;

            double b3 = t * t * t / 6.0f;

            return b0 * pv[0] + b1 * pv[1] + b2 * pv[2] + b3 * pv[3];
        }

        private void CalculateBStylePoint(double stf, Vector st, double edf, Vector ed, ref Vector[] pv)
        {
            double tf = (stf + edf) / 2;
            Vector v = CalculateBStylePointWithFactor(tf, ref pv);
            pointsSet.Add(new Point((int)v.X, (int)v.Y));

            if (Math.Abs((int)st.X - (int)v.X) <= 1
                && Math.Abs((int)st.Y - (int)v.Y) <= 1
                && Math.Abs((int)v.X - (int)ed.X) <= 1
                && Math.Abs((int)v.Y - (int)ed.Y) <= 1)
                return;

            CalculateBStylePoint(stf, st, tf, v, ref pv);
            CalculateBStylePoint(tf, v, edf, ed, ref pv);
        }
        public override void CalculatePointsSet()
        {
            if (keyPoints.Count <= 4)
                return;

            for(int i = 0; i < keyPoints.Count - 3; i++)
            {
                Vector[] pv = new Vector[4]
                {
                    new Vector(keyPoints[i]),
                    new Vector(keyPoints[i + 1]),
                    new Vector(keyPoints[i + 2]),
                    new Vector(keyPoints[i + 3]),
                };

                Vector st = CalculateBStylePointWithFactor(0, ref pv);
                Vector ed = CalculateBStylePointWithFactor(1, ref pv);
                pointsSet.Add(st.ToPoint());
                pointsSet.Add(ed.ToPoint());
                CalculateBStylePoint(0, st, 1, ed, ref pv);
            }
        }
        public override bool IsCursorNearby(Point cursorPos)
        {
            foreach (Point p in keyPoints)
            {
                if (GetDistance(p, cursorPos) < 4)
                    return true;
            }
            return base.IsCursorNearby(cursorPos);
        }
    }

    public class CGUserGraphicsBlock : CGUserGraphics
    {
        public CGUserGraphicsBlock() { }
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

        private delegate void TransformOperation(ref CGUserGraphics g);
        private void TransformGraphicsFrame(TransformOperation btf)
        {
            if (!isUserGraphicsSelected || selectedUserGraphics == basePoint)
                return;

            if (rawSelectedGraphics == null)
                return;

            DeleteSelectedGraphics();

            // why not remove rawSelectedGraphics firstly and transform and refill it?
            //   reduce the loss between two adjacent graphics
            transfromSelectedGraphics = rawSelectedGraphics.Copy();
            btf(ref transfromSelectedGraphics);

            UpdateSelectedGraphics(transfromSelectedGraphics);
        }

        public void InternalColoring()
        {
            rawSelectedGraphics = selectedUserGraphics;
            TransformGraphicsFrame((ref CGUserGraphics g) => {
                g.InternalColoring();
            });
        }

        public void MoveSelectedGraphics(Point newPos)
        {
            TransformGraphicsFrame((ref CGUserGraphics g) => {
                g = g.TransformMove(newPos.X - posOfGraphicsWhenSelected.X,
                    newPos.Y - posOfGraphicsWhenSelected.Y);
            });
        }

        public void ZoomSelectedGraphics(Point newPos)
        {
            if (basePoint == null)
                return;
            TransformGraphicsFrame((ref CGUserGraphics g) =>
            {
                // except for ellipse
                CGUserGraphics newG = g.TransformZoom(
                    new Point(basePoint.X, basePoint.Y),
                    posOfGraphicsWhenSelected,
                    newPos);

                if (g.GetType().ToString() == "ComputerGraphicsWork.CGUserGraphicsCircle"
                && newG.GetType().ToString() == "ComputerGraphicsWork.CGUserGraphicsEllipse")
                {
                    rawSelectedGraphics = newG;
                }
                g = newG;
            });
        }

        public void RotateSelectedGraphics(Point newPos)
        {
            if (basePoint == null)
                return;
            TransformGraphicsFrame((ref CGUserGraphics g) => {
                g = g.TransformRotation(
                    new Point(basePoint.X, basePoint.Y),
                    posOfGraphicsWhenSelected,
                    newPos);
            });
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

        public void SaveToCGFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);


            sw.Write(".cg\n");

            foreach(var g in userGraphicsSet)
            {
                sw.Write(String.Format("{0}:", g.GetType().ToString()));

                foreach(var p in g.keyPoints)
                {
                    sw.Write(String.Format(" {0} {1}", p.X, p.Y));
                }

                sw.Write("\n");
            }


            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public void LoadFromCGFile(string fileName)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(fileName, FileMode.OpenOrCreate);
            }
            catch
            {
                MessageBox.Show(String.Format("fail to open file '{0}'", fileName));
                return;
            }
            StreamReader sr = new StreamReader(fs);


            String fileType = sr.ReadLine();

            if(fileType != ".cg")
            {
                sr.Close();
                fs.Close();
                MessageBox.Show(String.Format("file '{0}' is not cg file", fileName));
                return;
            }

            while (!sr.EndOfStream)
            {
                string lineText = sr.ReadLine();

                // split by :
                string[] columns = lineText.Split(':');

                if (columns.Count() < 2)
                    continue;

                // delete spaces at the begin and end position
                string allNumStrings = columns[1].Trim();

                // split by ' ' ==> "12" "23" "78" "89" ...
                string[] numStrings = allNumStrings.Split(' ');

                int[] numInts = new int[numStrings.Count()];

                // to int
                for(int i = 0; i < numStrings.Count(); i++)
                {
                    numInts[i] = Convert.ToInt32(numStrings[i]);
                }


                if (numInts.Count() == 0 ||  numInts.Count() % 2 != 0)
                    continue;

                // convert to key points
                List<Point> keyPoints = new List<Point>();

                for(int i = 0; i < numInts.Count(); i += 2)
                {
                    keyPoints.Add(new Point(numInts[i], numInts[i + 1]));
                }

                CGUserGraphics graphics = null;
                switch (columns[0])
                {
                    case "ComputerGraphicsWork.CGUserGraphicsPoint":
                        if (keyPoints.Count < 1)
                            break;
                        graphics = new CGUserGraphicsPoint(keyPoints[0]);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsLine":
                        if (keyPoints.Count < 2)
                            break;
                        graphics = new CGUserGraphicsLine(keyPoints[0], keyPoints[1]);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsCircle":
                        if (keyPoints.Count < 2)
                            break;
                        graphics = new CGUserGraphicsCircle(keyPoints[0], keyPoints[1]);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsEllipse":
                        if (keyPoints.Count < 3)
                            break;
                        CGUserGraphicsEllipse ellipse = new CGUserGraphicsEllipse();
                        keyPoints.ForEach((p) => { ellipse.keyPoints.Add(p); });
                        ellipse.CalculatePointsSet();
                        graphics = ellipse;
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsRectangle":
                    case "ComputerGraphicsWork.CGUserGraphicsPolygon":
                        graphics = new CGUserGraphicsPolygon(keyPoints);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsBStyleCurve":
                        graphics = new CGUserGraphicsBStyleCurve(keyPoints);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsBezier":
                        graphics = new CGUserGraphicsBezier(keyPoints);
                        break;
                    case "ComputerGraphicsWork.CGUserGraphicsBlock":
                        graphics = new CGUserGraphicsBlock(keyPoints);
                        break;
                    default:
                        // MessageBox.Show(String.Format(""));
                        break;
                }

                if(graphics != null)
                {
                    this.AddGraphics(graphics);
                }
            }


            sr.Close();
            fs.Close();

        }
    }
}
