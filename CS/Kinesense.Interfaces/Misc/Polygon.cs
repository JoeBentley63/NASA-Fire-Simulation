using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces.Geo
{
	/// <summary>
	/// Class which allows comparisons between polygons, overlap and area calculations. 
	/// To be used in the filter class
	/// </summary>
	public class Polygon
	{
		List<Point> _points = new List<Point>();
		public Point[] Points
		{
			get { return _points.ToArray(); }
			set
			{
				_points.Clear();
				_points.AddRange(value);
			}
		}

		public int Length { get { return _points.Count; } }

		public Polygon()
		{

		}

		public Polygon(IEnumerable<Point> points)
		{
			_points.AddRange(points);
		}

		/// <summary>
		/// rect = {x1, y1, x2, y2}
		/// </summary>
		/// <param name="rect"></param>
		public Polygon(int[] rect)
		{
			_points.Add(new Point(rect[0], rect[1]));
			_points.Add(new Point(rect[0], rect[3]));
			_points.Add(new Point(rect[2], rect[1]));
			_points.Add(new Point(rect[2], rect[3]));
		}

		/// <summary>
		/// rect = {x1, y1, x2, y2}
		/// </summary>
		/// <param name="rect"></param>
		public Polygon(float[] rect, int scale)
		{
			_points.Add(new Point((int)(rect[0] * scale), (int)(rect[1] * scale)));
			_points.Add(new Point((int)(rect[0] * scale), (int)(rect[3] * scale)));
			_points.Add(new Point((int)(rect[2] * scale), (int)(rect[1] * scale)));
			_points.Add(new Point((int)(rect[2] * scale), (int)(rect[3] * scale)));
		}

		public Point this[int n]
		{
			get { return _points[n]; }
		}

		public void Add(Point p)
		{
			_points.Add(p);
		}

		public void AddRange(Point[] p)
		{
			_points.AddRange(p);
		}

		public static bool IsPointInside(Point point, Polygon polygon)
		{
			//from local.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/
			int counter = 0;
			double xinters;
			Point p1, p2;
			int N = polygon.Length;

			//check if the point equals any of the verts
			for (int i = 0; i < polygon.Length; i++)
				if (polygon[i].X == point.X && polygon[i].Y == point.Y)
					return true;

			p1 = polygon[0];
			for (int i = 1; i <= N; i++)
			{
				p2 = polygon[i % N];
				if (point.Y > Math.Min(p1.Y, p2.Y)
					&& point.Y <= Math.Max(p1.Y, p2.Y)
						&& point.X <= Math.Max(p1.X, p2.X)
							&& p1.Y != p2.Y)
							{
								xinters = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
								if (p1.X == p2.X || point.X <= xinters)
									counter++;
							}
				p1 = p2;
			}
			return counter % 2 != 0;
		}

		public bool IsPointInside(Point point)
		{
			//from local.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/
			int counter = 0;
			double xinters;
			Point p1, p2;
			int N = _points.Count;

			//check if the point equals any of the verts
			for (int i = 0; i < this.Points.Length; i++ )
				if(Points[i].X == point.X && Points[i].Y == point.Y)
					return true;

			p1 = _points[0];
			for (int i = 1; i <= N; i++)
			{
				p2 = _points[i % N];
				if (point.Y > Math.Min(p1.Y, p2.Y)
					&& point.Y <= Math.Max(p1.Y, p2.Y)
						&&point.X <= Math.Max(p1.X, p2.X)
							&&p1.Y != p2.Y)
							{
								xinters = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
								if (p1.X == p2.X || point.X <= xinters)
									counter++;
							}
				p1 = p2;
			}
			return counter % 2 != 0;
		}

		public bool HasOverlap(Polygon rectCorners)
		{
			return HasInsidePoints(rectCorners, this)
			       || HasInsidePoints(this, rectCorners)
			       || HasIntersectionPoints(rectCorners, this);
		}

		/// <summary>
		/// returns the overlap area
		/// </summary>
		/// <param name="rectCorners"></param>
		/// <returns></returns>
		public int FindOverlap(Polygon rectCorners)
		{

			Polygon overlapPoly = new Polygon();
			overlapPoly.AddRange(GetInsidePoints(rectCorners, this));
			overlapPoly.AddRange(GetInsidePoints(this, rectCorners));
			overlapPoly.AddRange(GetAllIntersectionPoints(rectCorners, this));

			if (overlapPoly.Length < 3)
				return 0;

			return (int)overlapPoly.GetArea();
		}

		public double GetArea()
		{
			//area algorithm needs to be sorted.
			_points = new List<Point>(SortPointsClockwise(_points.ToArray()));
			double area = 0d;
			for (int i = 0; i < _points.Count; i++)
				area += (_points[i].X * _points[(i + 1) % _points.Count].Y)
					- (_points[(i + 1) % _points.Count].X * _points[i].Y);
			return Math.Abs(area / 2);
		}

        //public Rectangle GetBoundingBox()
        //{
        //    get bounding box
        //    int Xmax = 0, Xmin = int.MaxValue, Ymax = 0, Ymin = int.MaxValue;
        //    for (int i = 0; i < this.Length; i++)
        //    {
        //        if (Xmax < this[i].X)
        //            Xmax = this[i].X;
        //        if (Xmin > this[i].X)
        //            Xmin = this[i].X;
        //        if (Ymax < this[i].Y)
        //            Ymax = this[i].Y;
        //        if (Ymin > this[i].Y)
        //            Ymin = this[i].Y;
        //    }

        //    return new Rectangle(Xmin, Ymin, Xmax - Xmin, Ymax - Ymin);
        //}

		private Point[] SortPointsClockwise(Point[] polygon)
		{
			if (polygon.Length < 3)
				return polygon;

			List<Point> sorted = new List<Point>(),
				transfored = new List<Point>();

			//get bounding box
			int Xavg = 0, Yavg = 0;
			for (int i = 0; i < polygon.Length; i++)
			{
				Xavg += polygon[i].X;
				Yavg += polygon[i].Y;
			}
			Xavg /= polygon.Length;
			Yavg /= polygon.Length;

			List<double[]> angles = new List<double[]>();

			for (int i = 0; i < polygon.Length; i++)
				transfored.Add(new Point(polygon[i].X - Xavg, polygon[i].Y - Yavg));

			transfored.Sort(CompareClockwise);

			for (int i = 0; i < transfored.Count; i++)
				sorted.Add(new Point(transfored[i].X + Xavg, transfored[i].Y + Yavg));

			return sorted.ToArray();
		}

		protected static int CompareClockwise(Point A, Point B)
		{
			return (B.X * A.Y) - (A.X * B.Y);
		}

		public static Point[] GetInsidePoints(Polygon shape1, Polygon shape2)
		{
			//are any corners inside?
			List<Point> insidepoints = new List<Point>();
			for (int i = 0; i < shape1.Length; i++)
				if (IsPointInside(shape1[i], shape2))
					insidepoints.Add(shape1[i]);
			return insidepoints.ToArray();
		}

		public static bool HasInsidePoints(Polygon shape1, Polygon shape2)
		{
			//are any corners inside?
			for (int i = 0; i < shape1.Length; i++)
				if (IsPointInside(shape1[i], shape2))
					return true;
			
			return false;
		}

		public static bool HasIntersectionPoints(Polygon shape1, Polygon shape2)
		{
			//find overlap points
			Point[] r = new Point[] { shape1[0], Point.Empty };
			List<Point> overlapPolygon = new List<Point>();
			for (int i = 1; i <= shape1.Length; i++)
			{
				r[1] = shape1[i % shape1.Length];

				Point[] p = new Point[] { shape2[0], Point.Empty };
				for (int j = 1; j <= shape2.Length; j++)
				{
					p[1] = shape2[j % shape2.Length];
					Point intersection = GetCrossingPoint(r, p);
					if (!intersection.IsEmpty)
						return true;

					p[0] = p[1];
				}

				r[0] = r[1];
			}

			return false;
		}

		public static Point[] GetAllIntersectionPoints(Polygon shape1, Polygon shape2)
		{
			//find overlap points
			Point[] r = new Point[] { shape1[0], Point.Empty };
			List<Point> overlapPolygon = new List<Point>();
			for (int i = 1; i <= shape1.Length; i++)
			{
				r[1] = shape1[i%shape1.Length];

				//overlapPolygon.Add(r[0]);
				Point[] p = new Point[] {shape2[0], Point.Empty};
				for (int j = 1; j <= shape2.Length; j++)
				{
					p[1] = shape2[j%shape2.Length];
					Point intersection = GetCrossingPoint(r, p);
					if (!intersection.IsEmpty)
						overlapPolygon.Add(intersection);

					p[0] = p[1];
				}

				r[0] = r[1];
			}

			return overlapPolygon.ToArray();
		}

		public static Point[] GetIntersectionPoints(Polygon shape1, Polygon shape2)
		{
			//find overlap points
			Point[] r = new Point[] { shape1[0], Point.Empty };
			List<Point> overlapPolygon = new List<Point>();
			for (int i = 1; i <= shape1.Length; i++)
			{
				r[1] = shape1[i % shape1.Length];

				bool r0inside = IsPointInside(r[0], shape2);
				bool r1inside = IsPointInside(r[1], shape2);

				if (r0inside != r1inside)
				{
					Point[] test;
					if (r0inside == true)
						test = new Point[] { r[0], r[1] };
					else
						test = new Point[] { r[1], r[0] };

					//overlapPolygon.Add(r[0]);
					Point[] p = new Point[] { shape2[0], Point.Empty };
					for (int j = 1; j <= shape2.Length; j++)
					{
						p[1] = shape2[j % shape2.Length];
						Point intersection = GetCrossingPoint(r, p);
						if (!intersection.IsEmpty)
						{
							overlapPolygon.Add(intersection);
							break;
						}
						p[0] = p[1];
					}
				}

				r[0] = r[1];
			}

			return overlapPolygon.ToArray();
		}

		public static Point GetCrossingPoint(Point[] l1, Point[] l2)
		{
			float d = (l1[1].X - l1[0].X) * (l2[1].Y - l2[0].Y) - (l1[1].Y - l1[0].Y) * (l2[1].X - l2[0].X);
			if (d == 0f)
				return Point.Empty;

			float r = ((l1[0].Y - l2[0].Y) * (l2[1].X - l2[0].X) - (l1[0].X - l2[0].X) * (l2[1].Y - l2[0].Y)) / d;
			float s = ((l1[0].Y - l2[0].Y) * (l1[1].X - l1[0].X) - (l1[0].X - l2[0].X) * (l1[1].Y - l1[0].Y)) / d;

			if (r < 0 || r > 1 || s < 0 || s > 1)
				//intersection is outside linesegment
				return Point.Empty;

			return new Point(
				l1[0].X + (int)(0.5f + r * (l1[1].X - l1[0].X)),
				l1[0].Y + (int)(0.5f + r * (l1[1].Y - l1[0].Y)));

		}

	}

    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;

        public static Point Empty { get { return new Point(-1, -1); } }

        public bool IsEmpty { get { return (X == -1 && Y == -1); } }
    }

    public struct Line
    {
        public Line(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }
        public Point P1;
        public Point P2;
    }

}
