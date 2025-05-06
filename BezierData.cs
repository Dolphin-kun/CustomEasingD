using BezierEasing.View.BezierEditor;
using System.Windows;

namespace BezierEasing
{
    public class BezierData
    {
        public List<Point> ControlPoints { get; set; } = new();
        public List<BezierSegmentData> Segments { get; set; } = new();

        public double CnavasSize { get; set; } = 300;
    }
}
