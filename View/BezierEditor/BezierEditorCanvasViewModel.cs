using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace BezierEasing.View.BezierEditor
{
    public class BezierSegmentData
    {
        public Point Handle1 { get; set; }
        public Point Handle2 { get; set; }
    }

    public class BezierEditorCanvasViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Point> ControlPoints { get; } = new();
        public List<BezierSegmentData> BezierSegments { get; } = new();

        private readonly int canvasSize = 300;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? BezierDataChanged;

        public BezierEditorCanvasViewModel()
        {
            ControlPoints.CollectionChanged += (_, _) => OnBezierDataChanged();

            ControlPoints.Add(new Point(0, canvasSize));
            ControlPoints.Add(new Point(canvasSize, 0));

            InitializeSegments();
        }

        private void OnBezierDataChanged()
        {
            BezierDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LoadFromBezierData(BezierData data)
        {
            ControlPoints.Clear();
            foreach (var p in data.ControlPoints)
            {
                ControlPoints.Add(p);
            }

            InitializeSegments();
        }

        public BezierData ExportToBezierData()
        {
            return new BezierData
            {
                ControlPoints = ControlPoints.ToList(),
                Segments = BezierSegments.ToList()
            };
        }


        private void InitializeSegments()
        {
            BezierSegments.Clear();

            for (int i = 0; i < ControlPoints.Count - 1; i++)
            {
                Point p0 = i > 0 ? ControlPoints[i - 1] : ControlPoints[i];
                Point p1 = ControlPoints[i];
                Point p2 = ControlPoints[i + 1];
                Point p3 = (i + 2 < ControlPoints.Count) ? ControlPoints[i + 2] : p2;

                Point handle1 = new(p1.X + (p2.X - p0.X) / 6, p1.Y + (p2.Y - p0.Y) / 6);
                Point handle2 = new(p2.X - (p3.X - p1.X) / 6, p2.Y - (p3.Y - p1.Y) / 6);


                BezierSegments.Add(new BezierSegmentData
                {
                    Handle1 = handle1,
                    Handle2 = handle2
                });
            }
        }

        public void AddControlPoint(Point position)
        {
            ControlPoints.Insert(ControlPoints.Count - 1, position);
            SortControlPoints();
            InitializeSegments();
            OnBezierDataChanged();
        }

        public void MoveControlPoint(int index, Vector delta)
        {
            if (index < 0 || index >= ControlPoints.Count)
                return;

            var oldPosition = ControlPoints[index];
            var moved = ControlPoints[index] + delta;

            if (index > 0)
                moved.X = Math.Max(BezierSegments[index - 1].Handle1.X + 1, moved.X);
            if (index < ControlPoints.Count - 1)
                moved.X = Math.Min(BezierSegments[index].Handle2.X - 1, moved.X);

            moved.X = Math.Clamp(moved.X, 0, canvasSize);
            moved.Y = Math.Clamp(moved.Y, 0, canvasSize);

            Vector actualDelta = moved - oldPosition;

            if (index > 0 && index - 1 < BezierSegments.Count)
            {
                var center = oldPosition;
                var newCenter = moved;
                var leftHandle = BezierSegments[index - 1].Handle2;

                Vector handleOffset = leftHandle - center;
                double angle = Math.Atan2(handleOffset.Y, handleOffset.X);
                double originalLength = handleOffset.Length;

                var newHandle = newCenter + new Vector(
                    Math.Cos(angle) * originalLength,
                    Math.Sin(angle) * originalLength
                );

                double minX = Math.Min(newCenter.X, BezierSegments[index - 1].Handle1.X);
                double maxX = Math.Max(newCenter.X, BezierSegments[index - 1].Handle1.X);

                if (newHandle.X < minX || newHandle.X > maxX)
                {
                    double clampedX = Math.Clamp(newHandle.X, minX, maxX);
                    double dx = clampedX - newCenter.X;
                    double dy = dx * Math.Tan(angle);
                    newHandle = (Point)new Vector(clampedX, newCenter.Y + dy);
                }

                BezierSegments[index - 1].Handle2 = newHandle;
            }

            if (index < BezierSegments.Count)
            {
                var center = oldPosition;
                var newCenter = moved;
                var rightHandle = BezierSegments[index].Handle1;

                Vector handleOffset = rightHandle - center;
                double angle = Math.Atan2(handleOffset.Y, handleOffset.X);
                double originalLength = handleOffset.Length;

                var newHandle = newCenter + new Vector(
                    Math.Cos(angle) * originalLength,
                    Math.Sin(angle) * originalLength
                );

                double minX = Math.Min(newCenter.X, BezierSegments[index].Handle2.X);
                double maxX = Math.Max(newCenter.X, BezierSegments[index].Handle2.X);

                if (newHandle.X < minX || newHandle.X > maxX)
                {
                    double clampedX = Math.Clamp(newHandle.X, minX, maxX);
                    double dx = clampedX - newCenter.X;
                    double dy = dx * Math.Tan(angle);
                    newHandle = (Point)new Vector(clampedX, newCenter.Y + dy);
                }

                BezierSegments[index].Handle1 = newHandle;
            }



            ControlPoints[index] = moved;
            SortControlPoints();
            OnBezierDataChanged();
        }

        public void MoveHandle(int segmentIndex, bool isRightHandle, Point newPosition, bool mirror = true)
        {
            if (segmentIndex < 0 || segmentIndex >= BezierSegments.Count)
                return;

            if (isRightHandle)
            {
                var nextHandleX = BezierSegments[segmentIndex].Handle2.X;
                var controlPointX = ControlPoints[segmentIndex].X;
                newPosition.X = Math.Min(newPosition.X, nextHandleX);
                newPosition.X = Math.Max(newPosition.X, controlPointX);
                BezierSegments[segmentIndex].Handle1 = newPosition;
            }
            else
            {
                var preHandleX = BezierSegments[segmentIndex].Handle1.X;
                var controlPointX = ControlPoints[segmentIndex + 1].X;
                newPosition.X = Math.Max(newPosition.X, preHandleX);
                newPosition.X = Math.Min(newPosition.X, controlPointX);
                BezierSegments[segmentIndex].Handle2 = newPosition;
            }


            OnBezierDataChanged();
        }

        private void SortControlPoints()
        {
            var sorted = ControlPoints.OrderBy(p => p.X).ToList();
            ControlPoints.Clear();
            foreach (var p in sorted)
            {
                ControlPoints.Add(p);
            }
        }
    }
}
