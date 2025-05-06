using BezierEasing.View.BezierEditor;
using System.Windows;

namespace BezierEasing
{
    public class BezierEasingFunction
    {
        private readonly List<Point> controlPoints;
        private readonly List<BezierSegmentData> bezierSegments;
        private readonly double canvasSize;

        public BezierEasingFunction(IEnumerable<Point> controlPoints, IEnumerable<BezierSegmentData> segments, double canvasSize)
        {
            this.controlPoints = [.. controlPoints];
            this.bezierSegments = [.. segments];
            this.canvasSize = canvasSize;
        }

        public double Evaluate(double x01)
        {
            if (controlPoints.Count < 2)
                return 0;

            double x = x01 * canvasSize;

            for (int i = 0; i < bezierSegments.Count; i++)
            {
                var p0 = controlPoints[i];
                var p3 = controlPoints[i + 1];

                if (x < p0.X || x > p3.X)
                    continue;

                var seg = bezierSegments[i];
                var p1 = seg.Handle1;
                var p2 = seg.Handle2;

                double t = FindBezierTForX(p0.X, p1.X, p2.X, p3.X, x);
                double y = EvaluateCubicBezier(p0.Y, p1.Y, p2.Y, p3.Y, t);

                return y / canvasSize;
            }

            return controlPoints.Last().Y / canvasSize;
        }
        private static double EvaluateCubicBezier(double p0, double p1, double p2, double p3, double t)
        {
            double mt = 1 - t;
            return mt * mt * mt * p0 +
                   3 * mt * mt * t * p1 +
                   3 * mt * t * t * p2 +
                   t * t * t * p3;
        }

        private static double FindBezierTForX(double x0, double x1, double x2, double x3, double targetX, int iterations = 12)
        {
            double tMin = 0.0;
            double tMax = 1.0;

            for (int i = 0; i < iterations; i++)
            {
                double t = (tMin + tMax) / 2;
                double x = EvaluateCubicBezier(x0, x1, x2, x3, t);

                if (Math.Abs(x - targetX) < 0.5)
                    return t;

                if (x < targetX)
                    tMin = t;
                else
                    tMax = t;
            }

            return (tMin + tMax) / 2;
        }
    }
}
