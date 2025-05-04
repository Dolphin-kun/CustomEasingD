using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierEasing.View
{
    public partial class BezierEditorWindow : Window
    {
        private readonly BezierEditorViewModel _viewModel = new();

        private Ellipse? _activeDot;
        private bool _isDragging = false;
        private Point _mouseOffset;

        private readonly int MaxControlDots = 32;

        private readonly List<Ellipse> _ellipseDots = new();
        private readonly List<Line> _connectionLines = new();

        public BezierEditorWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        //Pathの描画
        private void DrawBezierCurve()
        {
            var oldPath = BezierEditor.Children.OfType<Path>().FirstOrDefault();
            if (oldPath is not null)
                BezierEditor.Children.Remove(oldPath);

            PathFigure figure = new();

            Point start = new(0, BezierEditor.ActualHeight);
            Point end = new(BezierEditor.ActualWidth, 0);

            figure.StartPoint = start;
            figure.IsClosed = false;
            PathSegmentCollection segments = new();

            var points = _viewModel.ControlPoints;

            if (points.Count == 0)
            {
                segments.Add(new LineSegment(end, true));
            }
            else if (points.Count == 1)
            {
                segments.Add(new QuadraticBezierSegment(points[0], end, true));
            }
            else
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Point control = points[i];
                    Point next = points[i + 1];
                    Point mid = new((control.X + next.X) / 2, (control.Y + next.Y) / 2);

                    segments.Add(new QuadraticBezierSegment(control, mid, true));
                }

                Point lastControl = points.Last();

                segments.Add(new QuadraticBezierSegment(lastControl, end, true));
            }

            figure.Segments = segments;

            PathGeometry geometry = new() { Figures = { figure } };

            Path path = new()
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Data = geometry
            };

            BezierEditor.Children.Add(path);
        }


        private void DrawControlDots()
        {
            //制御点の削除
            foreach (var dot in _ellipseDots)
            {
                BezierEditor.Children.Remove(dot);
            }
            _ellipseDots.Clear();

            //制御点同士の線の削除
            foreach (var line in _connectionLines)
            {
                BezierEditor.Children.Remove(line);
            }
            _connectionLines.Clear();

            foreach (var point in _viewModel.ControlPoints)
            {
                Ellipse newDot = CreateControlDot(point);
                BezierEditor.Children.Add(newDot);
                _ellipseDots.Add(newDot);
            }

            List<Point> allPoints = new();

            Point start = new(0, BezierEditor.ActualHeight);
            Point end = new(BezierEditor.ActualWidth, 0);

            allPoints.Add(start);
            allPoints.AddRange(_viewModel.ControlPoints);
            allPoints.Add(end);

            for (int i = 0; i < allPoints.Count - 1; i++)
            {
                var fromPoint = allPoints[i];
                var toPoint = allPoints[i + 1];

                var line = CreateConnectionLine(fromPoint, toPoint);
                BezierEditor.Children.Add(line);
                _connectionLines.Add(line);
            }
        }


        // 制御点の描画
        private Ellipse CreateControlDot(Point position)
        {
            Ellipse dot = new()
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = [2, 2],
                Cursor = Cursors.Hand
            };

            Canvas.SetLeft(dot, position.X - dot.Width / 2);
            Canvas.SetTop(dot, position.Y - dot.Height / 2);

            dot.MouseLeftButtonDown += ControlDot_MouseLeftButtonDown;
            dot.MouseMove += ControlDot_MouseMove;
            dot.MouseLeftButtonUp += ControlDot_MouseLeftButtonUp;

            return dot;
        }

        private Line CreateConnectionLine(Point from, Point to)
        {
            Line line = new()
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                X1 = from.X,
                Y1 = from.Y,
                X2 = to.X,
                Y2 = to.Y,
            };

            return line;
        }

        private void UpdateLinePosition(Line line, Ellipse? fromDot, Ellipse? toDot)
        {
            double fromX, fromY;
            if (fromDot != null)
            {
                fromX = Canvas.GetLeft(fromDot) + fromDot.Width / 2;
                fromY = Canvas.GetTop(fromDot) + fromDot.Height / 2;
            }
            else
            {
                fromX = 0;
                fromY = BezierEditor.ActualHeight;
            }

            double toX, toY;
            if (toDot != null)
            {
                toX = Canvas.GetLeft(toDot) + toDot.Width / 2;
                toY = Canvas.GetTop(toDot) + toDot.Height / 2;
            }
            else
            {
                toX = BezierEditor.ActualWidth;
                toY = 0;
            }

            line.X1 = fromX;
            line.Y1 = fromY;
            line.X2 = toX;
            line.Y2 = toY;
        }


        //制御点追加
        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(_viewModel.ControlPoints.Count >= MaxControlDots)
            {
                MessageBox.Show(
                    "これ以上制御点を追加できません。",
                    "最大" + MaxControlDots + "点まで。",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Point clickPosition = e.GetPosition(BezierEditor);

            int insertIndex = _viewModel.ControlPoints.FindIndex(p=> p.X > clickPosition.X);
            if (insertIndex == -1)
                _viewModel.ControlPoints.Add(clickPosition);
            else
                _viewModel.ControlPoints.Insert(insertIndex, clickPosition);

            RedrawEverything();
        }

        private void RedrawEverything()
        {
            DrawControlDots();
            DrawBezierCurve();
        }

        //制御点コントロール
        private void ControlDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse dot)
            {
                _isDragging = true;
                _activeDot = dot;
                _mouseOffset = e.GetPosition(BezierEditor);
                _mouseOffset.X -= Canvas.GetLeft(dot);
                _mouseOffset.Y -= Canvas.GetTop(dot);
                _activeDot.CaptureMouse();
                _viewModel.ActivePointIndex = _ellipseDots.IndexOf(dot);
            }
        }

        private void ControlDot_MouseMove(object sender, MouseEventArgs e)
        {
            if (_activeDot != null && _isDragging)
            {
                Point position = e.GetPosition(BezierEditor);
                double newLeft = position.X - _mouseOffset.X;
                double newTop = position.Y - _mouseOffset.Y;

                int index = _viewModel.ActivePointIndex;
                if (index > 0)
                {
                    var prev = _viewModel.ControlPoints[index - 1];
                    newLeft = Math.Max(prev.X + 1, newLeft);
                }
                if(index< _viewModel.ControlPoints.Count - 1)
                {
                    var next = _viewModel.ControlPoints[index + 1];
                    newLeft = Math.Min(next.X - 1, newLeft);
                }

                newLeft = Math.Max(0, Math.Min(BezierEditor.ActualWidth - _activeDot.Width, newLeft));
                newTop = Math.Max(0, Math.Min(BezierEditor.ActualHeight - _activeDot.Height, newTop));

                var newPoint = new Point(newLeft, newTop);
                _viewModel.ControlPoints[_viewModel.ActivePointIndex] = newPoint;

                Canvas.SetLeft(_activeDot, newLeft);
                Canvas.SetTop(_activeDot, newTop);

                // 線を更新
                if (_viewModel.ActivePointIndex >= 0)
                {
                    int pointIndex = _viewModel.ActivePointIndex;

                    // 前の線を更新（start → point）
                    if (pointIndex >= 0 && pointIndex < _connectionLines.Count)
                    {
                        var fromDot = (pointIndex == 0) ? null : _ellipseDots[pointIndex - 1];
                        UpdateLinePosition(_connectionLines[pointIndex],
                            fromDot, _ellipseDots[pointIndex]);
                    }

                    // 次の線を更新（point → 次のpoint）
                    if (pointIndex + 1 < _viewModel.ControlPoints.Count)
                    {
                        UpdateLinePosition(_connectionLines[pointIndex + 1],
                            _ellipseDots[pointIndex],
                            _ellipseDots[pointIndex + 1]);
                    }
                }


                DrawBezierCurve();
            }
        }

        private void ControlDot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_activeDot is not null)
            {
                _isDragging = false;
                _activeDot.ReleaseMouseCapture();
                _activeDot = null;
            }
        }
    }
}
