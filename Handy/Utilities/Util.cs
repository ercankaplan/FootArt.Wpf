using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace Handy.Utilities
{
    public static class Util
    {
        public static double PixelMeterRate = 1;

        public static double PolygonArea(System.Windows.Shapes.Polygon polygon,int index)
        {

            double area = 0;
            if (index == 0 || index == 1 || index == 2)
            {
                if (polygon.Points.Count == 3)
                    area = SignedPolygonArea(polygon);

            }
            else if (index == 3)
            {
                if (polygon.Points.Count == 4)
                    area = SignedPolygonArea(polygon);
            }
            else if (index == 4)
            {
                if (polygon.Points.Count == 6)
                    area = SignedPolygonArea(polygon);
            }
            return area;

        }
        private static double SignedPolygonArea(System.Windows.Shapes.Polygon polygon)
        {
            // Add the first point to the end.    
            int num_points = polygon.Points.Count;
            System.Windows.Media.PointCollection pts = new System.Windows.Media.PointCollection(num_points + 1);
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                System.Windows.Point p = new System.Windows.Point();
                p.X = polygon.Points[i].X;
                p.Y = polygon.Points[i].Y;
                pts.Add(p);
            }
          
            // polygon.Points.CopyTo(pts, 0);
            System.Windows.Point ps = new System.Windows.Point();
            ps.X = polygon.Points[0].X;
            ps.Y = polygon.Points[0].Y;
            pts.Add(ps);
            // Get the areas.    
            double area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area += (pts[i + 1].X - pts[i].X) * (1 / PixelMeterRate) * (pts[i + 1].Y + pts[i].Y) * (1 / PixelMeterRate) / 2;

            }
            // Return the result.    
            return Math.Abs(area);
        }


    }



}
