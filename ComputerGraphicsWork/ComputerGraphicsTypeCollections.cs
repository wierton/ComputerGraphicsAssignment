using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerGraphicsWork
{
    public class UserLog
    {
        public void write(String s)
        {
            FileStream fs = new FileStream("log.txt", FileMode.OpenOrCreate | FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(s);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
    public class CGUserGraphics
    {
        public List<Point> pointsSet { get; } = new List<Point>();

        public virtual List<Point> CornerPoints()
        {
            return pointsSet;
        }

        public virtual bool IsCursorNearby(Point cursorPos)
        {
            return false;
        }

        public void DrawTagPoints(CGUserCanvas userCanvas)
        {
            foreach (Point p in CornerPoints())
            {
                CGUserGraphicsTinyRectangle tinyRect = new CGUserGraphicsTinyRectangle(p);
                userCanvas.SelectGraphics(tinyRect);
            }
        }

        public void ClearTagPoints(CGUserCanvas userCanvas)
        {
            foreach (Point p in CornerPoints())
            {
                CGUserGraphicsTinyRectangle tinyRect = new CGUserGraphicsTinyRectangle(p);
                userCanvas.ClearGraphics(tinyRect);
            }
        }
    }

    public class CGUserGraphicsPoint : CGUserGraphics
    {
        private Point point;
        public CGUserGraphicsPoint(Point start)
        {
            point = start;
            pointsSet.Add(start);
        }

        public override List<Point> CornerPoints()
        {
            return new List<Point>() { point };
        }
    }

    public class CGUserGraphicsLine : CGUserGraphics
    {
        private Point startPoint, endPoint;
        public CGUserGraphicsLine(Point start, Point end)
        {
            startPoint = start;
            endPoint = end;

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
            int dx = startPoint.X - endPoint.X;
            int dy = endPoint.Y - startPoint.Y;

            int maxX = Math.Max(startPoint.X, endPoint.X);
            int minX = Math.Min(startPoint.X, endPoint.X);

            int maxY = Math.Max(startPoint.Y, endPoint.Y);
            int minY = Math.Min(startPoint.Y, endPoint.Y);

            bool isInX = minX - 4 <= cursorPos.X && cursorPos.X <= maxX + 4;
            bool isInY = minY - 4 <= cursorPos.Y && cursorPos.Y <= maxY + 4;


            double baseDist = Math.Sqrt(dx * dx + dy * dy);
            double c = endPoint.X * startPoint.Y - startPoint.X * endPoint.Y;

            double d = Math.Abs(cursorPos.X * dy + cursorPos.Y * dx + c) / baseDist;

            if (d < 4 && isInX && isInY)
                return true;
            else
                return false;
        }

        public override List<Point> CornerPoints()
        {
            return new List<Point>() { startPoint, endPoint };
        }

    }
    public class CGUserGraphicsCircle : CGUserGraphics
    {
        Point centerPoint;
        int radius;

        public CGUserGraphicsCircle(Point center, Point edge)
        {
            centerPoint = center;
            int dx = edge.X - center.X;
            int dy = edge.Y - center.Y;
            radius = (int)Math.Sqrt((double)(dx * dx + dy * dy));

            List<Point> baseSet = new List<Point>();

            Point fp = new Point(0, radius);
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
            int dx = cursorPos.X - centerPoint.X;
            int dy = cursorPos.Y - centerPoint.Y;

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
        public override List<Point> CornerPoints()
        {
            return new List<Point>() {
                new Point(centerPoint.X + (int)radius, centerPoint.Y),
                new Point(centerPoint.X - (int)radius, centerPoint.Y),
                new Point(centerPoint.X, centerPoint.Y + (int)radius),
                new Point(centerPoint.X, centerPoint.Y - (int)radius),
            };
        }
    }
    public class CGUserGraphicsEllipse : CGUserGraphics
    {
        Point centerPoint;
        int xRadius, yRadius;
        public CGUserGraphicsEllipse(Point center, Point edge)
        {
            centerPoint = center;
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
            int dx = cursorPos.X - centerPoint.X;
            int dy = cursorPos.Y - centerPoint.Y;

            int xRadiusSquare = xRadius * xRadius;
            int yRadiusSquare = yRadius * yRadius;

            double dist = dx * dx * yRadiusSquare + dy * dy * xRadiusSquare;

            if (dist < xRadiusSquare * yRadiusSquare)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override List<Point> CornerPoints()
        {
            return new List<Point>() {
                new Point(centerPoint.X + (int)xRadius, centerPoint.Y),
                new Point(centerPoint.X - (int)xRadius, centerPoint.Y),
                new Point(centerPoint.X, centerPoint.Y + (int)yRadius),
                new Point(centerPoint.X, centerPoint.Y - (int)yRadius),
            };
        }
    }

    public class CGUserGraphicsTinyRectangle : CGUserGraphics
    {
        Rectangle rect;
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

        public override List<Point> CornerPoints()
        {
            return new List<Point>() {
                new Point(rect.X, rect.Y),
                new Point(rect.X + rect.Width, rect.Y),
                new Point(rect.X + rect.Width, rect.Y + rect.Width),
                new Point(rect.X, rect.Y + rect.Width),
            };
        }
    }

    public class CGUserCanvas
    {
        private int canvasWidth, canvasHeight;
        public Bitmap bmp { get; }
        private int[,] refCount;
        public CGUserCanvas(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
            bmp = new Bitmap(width, height);
            refCount = new int[width, height];
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
                bmp.SetPixel(pos.X, pos.Y, Color.White);
        }

        private Point ClipPoint(Point pos)
        {
            if (pos.X < 0) pos.X = 0;
            if (pos.X >= canvasWidth) pos.X = canvasWidth - 1;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.Y >= canvasHeight) pos.Y = canvasHeight - 1;
            return pos;
        }


        public void SelectGraphics(CGUserGraphics userGraphics)
        {
            foreach (Point point in userGraphics.pointsSet)
            {
                SetPixel(point);
            }
        }

        public void ClearGraphics(CGUserGraphics userGraphics)
        {
            foreach (Point point in userGraphics.pointsSet)
            {
                ClearPixel(point);
            }
        }

        public void FindGraphics(Point pos)
        {

        }

    }

}
