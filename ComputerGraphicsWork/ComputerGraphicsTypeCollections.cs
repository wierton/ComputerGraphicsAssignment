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
    public class CGUserGraphics
    {
        public List<Point> pointsSet { get; } = new List<Point>();

        public virtual List<Point> CornerPoints()
        {
            return new List<Point> ();
        }


        public virtual bool IsCursorNearby(Point cursorPos)
        {
            return false;
        }

        public virtual CGUserGraphics TransformMove(int dx, int dy)
        {
            CGUserGraphics newUserGraphics = new CGUserGraphics();
            pointsSet.ForEach((u) => { newUserGraphics.pointsSet.Add(new Point(u.X + dx, u.Y + dy)); });
            return newUserGraphics;
        }

        public Point TagPointNearCursor(Point cursorPos)
        {
            foreach (Point p in CornerPoints())
            {
                int dx = p.X - cursorPos.X;
                int dy = p.Y - cursorPos.Y;

                if (Math.Sqrt(dx * dx + dy * dy) < 4)
                {
                    return p;
                }
            }

            return new Point(-1, -1);
        }

        public virtual CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            // do nothing
            return TransformMove(newPos.X - oldPos.X, newPos.Y - oldPos.Y);
        }
        static public double Dist(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
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

        public override bool IsCursorNearby(Point cursorPos)
        {
            int dx = Math.Abs(point.X - cursorPos.X);
            int dy = Math.Abs(point.Y - cursorPos.Y);
            return (dx <= 4 && dy <= 4);
        }
        public override CGUserGraphics TransformMove(int dx, int dy)
        {
            CGUserGraphicsPoint newUserGraphics = new CGUserGraphicsPoint(new Point(point.X + dx, point.Y + dy));
            return newUserGraphics;
        }
    }

    public class CGUserGraphicsLine : CGUserGraphics
    {
        public Point firstPoint { get; }
        public Point nextPoint { get; }
        public CGUserGraphicsLine(Point start, Point end)
        {
            firstPoint = start;
            nextPoint = end;

            Point startPoint = start;
            Point endPoint = end;

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

        public override List<Point> CornerPoints()
        {
            return new List<Point>() { firstPoint, nextPoint };
        }

        public override CGUserGraphics TransformMove(int dx, int dy)
        {
            CGUserGraphicsLine newUserGraphics = new CGUserGraphicsLine(new Point(firstPoint.X + dx, firstPoint.Y + dy), new Point(nextPoint.X + dx, nextPoint.Y + dy));
            return newUserGraphics;
        }
        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            if(Dist(firstPoint, oldPos) < 4)
            {
                CGUserGraphicsLine newUserGraphics = new CGUserGraphicsLine(newPos, nextPoint);
                return newUserGraphics;
            }
            else if(Dist(nextPoint, oldPos) < 4)
            {
                CGUserGraphicsLine newUserGraphics = new CGUserGraphicsLine(newPos, firstPoint);
                return newUserGraphics;
            }
            else
            {
                return this;
            }
        }
    }
    public class CGUserGraphicsCircle : CGUserGraphics
    {
        Point centerPoint;
        Point edgePoint;
        int radius;

        public CGUserGraphicsCircle(Point center, Point edge)
        {
            centerPoint = center;
            edgePoint = edge;
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
        public override CGUserGraphics TransformMove(int dx, int dy)
        {
            CGUserGraphicsCircle newUserGraphics = new CGUserGraphicsCircle(new Point(centerPoint.X + dx, centerPoint.Y + dy), new Point(edgePoint.X + dx, edgePoint.Y + dy));
            return newUserGraphics;
        }

        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            if (Math.Abs(Dist(centerPoint, oldPos) - radius) < 4)
            {
                CGUserGraphicsCircle newUserGraphics = new CGUserGraphicsCircle(centerPoint, newPos);
                return newUserGraphics;
            }
            else
            {
                return this;
            }
        }
    }
    public class CGUserGraphicsEllipse : CGUserGraphics
    {
        Point centerPoint;
        Point edgePoint;
        int xRadius, yRadius;
        public CGUserGraphicsEllipse(Point center, Point edge)
        {
            centerPoint = center;
            edgePoint = edge;
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
            foreach(Point p in pointsSet)
            {
                if (Dist(cursorPos, p) < 4.0)
                    return true;
            }
            return false;
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
        public override CGUserGraphics TransformMove(int dx, int dy)
        {
            CGUserGraphicsEllipse newUserGraphics = new CGUserGraphicsEllipse(new Point(centerPoint.X + dx, centerPoint.Y + dy), new Point(edgePoint.X + dx, edgePoint.Y + dy));
            return newUserGraphics;
        }
        public override CGUserGraphics TransformAdjust(Point oldPos, Point newPos)
        {
            if (Dist(new Point(centerPoint.X, centerPoint.Y + yRadius), oldPos) < 8
             || Dist(new Point(centerPoint.X, centerPoint.Y - yRadius), oldPos) < 8)
            {
                CGUserGraphicsEllipse newUserGraphics = new CGUserGraphicsEllipse(centerPoint, new Point(edgePoint.X, newPos.Y));
                return newUserGraphics;
            }
            else if (Dist(new Point(centerPoint.X + xRadius, centerPoint.Y), oldPos) < 8
                  || Dist(new Point(centerPoint.X - xRadius, centerPoint.Y), oldPos) < 8)
            {
                CGUserGraphicsEllipse newUserGraphics = new CGUserGraphicsEllipse(centerPoint, new Point(newPos.X, edgePoint.Y));
                return newUserGraphics;
            }
            else
            {
                return this;
            }
        }
    }

    public class CGUserGraphicsPolygon : CGUserGraphics
    {
        List<Point> endPoints = new List<Point>();
        public List<CGUserGraphicsLine> edgeLines { get; } = new List<CGUserGraphicsLine>();

        public CGUserGraphicsPolygon(List<Point> inEndPoints)
        {
            inEndPoints.ForEach((u) => { endPoints.Add(u); });

            for(int i = 0; i < endPoints.Count - 1; i++)
            {
                edgeLines.Add(new CGUserGraphicsLine(endPoints[i], endPoints[i + 1]));
            }
            edgeLines.Add(new CGUserGraphicsLine(endPoints[endPoints.Count - 1], endPoints[0]));

            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });
        }

        public CGUserGraphicsPolygon(List<CGUserGraphicsLine> inEdgeLines)
        {
            inEdgeLines.ForEach((l) => { edgeLines.Add(l); });
            inEdgeLines.ForEach((l) => { endPoints.Add(l.firstPoint); });
            edgeLines.ForEach((g) => { g.pointsSet.ForEach((p) => { pointsSet.Add(p); }); });
        }
        public override CGUserGraphics TransformMove(int dx, int dy)
        {
            List<Point> lps = new List<Point>();
            endPoints.ForEach((p) => { lps.Add(new Point(p.X + dx, p.Y + dy)); });
            CGUserGraphicsPolygon newUserGraphics = new CGUserGraphicsPolygon(lps);
            return newUserGraphics;
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

        public override List<Point> CornerPoints()
        {
            return endPoints;
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
        public bool isUserGraphicsSelected { get; set; }
        public CGUserGraphics selectedUserGraphics;
        private int[,] refCount;
        private List<CGUserGraphics> userGraphicsSet = new List<CGUserGraphics>();
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

        public void SetGraphicsSelected(CGUserGraphics userGraphics)
        {
            this.ClearStateOfSelectedGraphics();

            selectedUserGraphics = userGraphics;
            isUserGraphicsSelected = true;

            foreach (Point p in userGraphics.CornerPoints())
            {
                this.DrawGraphics(new CGUserGraphicsTinyRectangle(p));
            }
        }

        public void ClearStateOfSelectedGraphics()
        {
            if (!isUserGraphicsSelected)
                return;

            foreach (Point p in selectedUserGraphics.CornerPoints())
            {
                this.UndrawGraphics(new CGUserGraphicsTinyRectangle(p));
            }
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

        public void MoveSelectedGraphics(int dx, int dy)
        {
            if (!isUserGraphicsSelected)
                return;

            CGUserGraphics newGraphics = selectedUserGraphics.TransformMove(dx, dy);

            CGUserGraphics oldGraphics = selectedUserGraphics;
            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(oldGraphics);

            this.AddGraphics(newGraphics);
            this.SetGraphicsSelected(newGraphics);

            selectedUserGraphics = newGraphics;
        }

        public void DeleteSelectedGraphics()
        {
            if (!isUserGraphicsSelected)
                return;

            CGUserGraphics oldGraphics = selectedUserGraphics;
            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(oldGraphics);
        }

        public void AdjustGraphicsByCursor(Point oldPos, Point newPos)
        {
            if (!isUserGraphicsSelected)
                return;

            CGUserGraphics newGraphics = selectedUserGraphics.TransformAdjust(oldPos, newPos);

            this.ClearStateOfSelectedGraphics();
            this.RemoveGraphics(selectedUserGraphics);

            this.AddGraphics(newGraphics);
            this.SetGraphicsSelected(newGraphics);

            selectedUserGraphics = newGraphics;
        }
    }
}
