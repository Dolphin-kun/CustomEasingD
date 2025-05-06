using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierEasing.View.BezierEditor
{
    public partial class BezierEditorCanvas : UserControl
    {
        public BezierEditorCanvasViewModel ViewModel { get; }

        private enum DragMode
        {
            None,
            ControlPoint,
            Handle
        }

        private DragMode currentDragMode = DragMode.None;
        private int dragIndex = -1;
        private bool isRightHandle = false;
        private Point dragStartPosition;

        public BezierEditorCanvas()
        {
            InitializeComponent();

            ViewModel = new BezierEditorCanvasViewModel();
            //DataContext = ViewModel;
            //ViewModel.BezierDataChanged += (_, _) => RedrawPaths();

            RedrawPaths();
        }

        private void BezierEditor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(BezierEditor);
            dragStartPosition = pos;

            // ハンドルクリック判定
            for (int i = 0; i < ViewModel.BezierSegments.Count; i++)
            {
                var seg = ViewModel.BezierSegments[i];
                if ((seg.Handle1 - pos).Length < 8)
                {
                    currentDragMode = DragMode.Handle;
                    dragIndex = i;
                    isRightHandle = true;
                    BezierEditor.CaptureMouse();
                    return;
                }
                if ((seg.Handle2 - pos).Length < 8)
                {
                    currentDragMode = DragMode.Handle;
                    dragIndex = i;
                    isRightHandle = false;
                    BezierEditor.CaptureMouse();
                    return;
                }
            }

            // コントロールポイントクリック判定
            for (int i = 0; i < ViewModel.ControlPoints.Count; i++)
            {
                if ((ViewModel.ControlPoints[i] - pos).Length < 8)
                {
                    currentDragMode = DragMode.ControlPoint;
                    dragIndex = i;
                    BezierEditor.CaptureMouse();
                    return;
                }
            }
        }

        private void BezierEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentDragMode = DragMode.None;
            dragIndex = -1;
            BezierEditor.ReleaseMouseCapture();
        }

        private void BezierEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentDragMode == DragMode.None)
                return;

            var pos = e.GetPosition(BezierEditor);
            var delta = pos - dragStartPosition;
            dragStartPosition = pos;

            if (currentDragMode == DragMode.ControlPoint)
            {
                ViewModel.MoveControlPoint(dragIndex, delta);
            }
            else if (currentDragMode == DragMode.Handle)
            {
                ViewModel.MoveHandle(dragIndex, isRightHandle, pos, mirror: true);
            }

            RedrawPaths();
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(BezierEditor);
            ViewModel.AddControlPoint(pos);
            RedrawPaths();
        }

        private void RedrawPaths()
        {
            BezierEditor.Children.Clear();
            // ベジェ本体
            var bezierGeometry = new PathGeometry();
            for (int i = 0; i < ViewModel.BezierSegments.Count; i++)
            {
                var p1 = ViewModel.ControlPoints[i];
                var p2 = ViewModel.ControlPoints[i + 1];
                var segment = ViewModel.BezierSegments[i];

                var figure = new PathFigure
                {
                    StartPoint = p1,
                    IsClosed = false
                };
                figure.Segments.Add(new BezierSegment(segment.Handle1, segment.Handle2, p2, true));
                bezierGeometry.Figures.Add(figure);
            }
            var bezierPath = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Fill = Brushes.Transparent,
                Data = bezierGeometry
            };
            BezierEditor.Children.Add(bezierPath);

            // ハンドル線
            var handleGeometry = new GeometryGroup();
            for (int i = 0; i < ViewModel.BezierSegments.Count; i++)
            {
                var p1 = ViewModel.ControlPoints[i];
                var p2 = ViewModel.ControlPoints[i + 1];
                var segment = ViewModel.BezierSegments[i];

                var line1 = new Line
                {
                    X1 = p1.X,
                    Y1 = p1.Y,
                    X2 = segment.Handle1.X,
                    Y2 = segment.Handle1.Y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                var line2 = new Line
                {
                    X1 = p2.X,
                    Y1 = p2.Y,
                    X2 = segment.Handle2.X,
                    Y2 = segment.Handle2.Y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                BezierEditor.Children.Add(line1);
                BezierEditor.Children.Add(line2);
            }

            // コントロールポイント
            foreach (var point in ViewModel.ControlPoints)
            {
                var ellipse = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.Red
                };
                Canvas.SetLeft(ellipse, point.X - 4);
                Canvas.SetTop(ellipse, point.Y - 4);
                BezierEditor.Children.Add(ellipse);
            }

            // ハンドル（controlPoint1, controlPoint2）
            foreach (var segment in ViewModel.BezierSegments)
            {
                var handle1 = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Blue
                };
                Canvas.SetLeft(handle1, segment.Handle1.X - 3);
                Canvas.SetTop(handle1, segment.Handle1.Y - 3);
                BezierEditor.Children.Add(handle1);

                var handle2 = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(handle2, segment.Handle2.X - 3);
                Canvas.SetTop(handle2, segment.Handle2.Y - 3);
                BezierEditor.Children.Add(handle2);
            }
        }
    }
}
