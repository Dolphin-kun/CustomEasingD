using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YukkuriMovieMaker.Commons;

namespace CustomEasingD.Control
{
    public partial class BezierControl : IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ItemProperty[]? ItemProperties { get; set; }

        public Point ControlPoint1
        {
            get { return (Point)GetValue(ControlPoint1Property); }
            set { SetValue(ControlPoint1Property, value); }
        }
        public Point ControlPoint2
        {
            get { return (Point)GetValue(ControlPoint2Property); }
            set { SetValue(ControlPoint2Property, value); }
        }


        public static readonly DependencyProperty ControlPoint1Property =
           DependencyProperty.Register(nameof(ControlPoint1), typeof(Point), typeof(BezierControl), new FrameworkPropertyMetadata(new Point(0,0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ControlPoint2Property =
           DependencyProperty.Register(nameof(ControlPoint2), typeof(Point), typeof(BezierControl), new FrameworkPropertyMetadata(new Point(0,0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        private bool isDragging = false;
        private Point mouseOffset;
        private Ellipse? activeDot;

        public BezierControl()
        {
            InitializeComponent();

            Panel.SetZIndex(ControlDot1, 10);
            Panel.SetZIndex(ControlDot2, 10);
            Panel.SetZIndex(BezierPath, 0);
        }

        private void DrawBezierCurve()
        {
            Point start = new Point(0,DrawCanvas.Height);
            Point end = new Point(DrawCanvas.Width, 0);
            Point control1 = new Point(Canvas.GetLeft(ControlDot1) + ControlDot1.Width / 2, Canvas.GetTop(ControlDot1) + ControlDot1.Height / 2);
            Point control2 = new Point(Canvas.GetLeft(ControlDot2) + ControlDot2.Width / 2, Canvas.GetTop(ControlDot2) + ControlDot2.Height / 2);
       

            PathFigure figure = new PathFigure
            {
                StartPoint = start,
                Segments = new PathSegmentCollection
                {
                    new BezierSegment(control1, control2, end, true)
                }
            };

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            BezierPath.Data = geometry;
        }

        private void ControlDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is Ellipse dot)
            {
                isDragging = true;
                activeDot = dot;
                mouseOffset = e.GetPosition(DrawCanvas);
                mouseOffset.X -= Canvas.GetLeft(dot);
                mouseOffset.Y -= Canvas.GetTop(dot);
                activeDot.CaptureMouse();
            }
           
        }

        private void ControlDot_MouseMove(object sender, MouseEventArgs e)
        {
            if(activeDot != null && isDragging)
            {
                Point position = e.GetPosition(DrawCanvas);
                double newLeft = position.X - mouseOffset.X;
                double newTop = position.Y - mouseOffset.Y;

                newLeft = Math.Max(0,Math.Min(DrawCanvas.ActualWidth - activeDot.Width, newLeft));
                newTop = Math.Max(0, Math.Min(DrawCanvas.ActualHeight - activeDot.Height, newTop));
                Canvas.SetLeft(activeDot, newLeft);
                Canvas.SetTop(activeDot, newTop);

                DrawBezierCurve();
            }
        }

        private void ControlDot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(activeDot != null)
            {
                BeginEdit?.Invoke(this, EventArgs.Empty);
                isDragging = false;
                activeDot.ReleaseMouseCapture();

                ControlPoint1 = new Point(
                    Canvas.GetLeft(ControlDot1) + ControlDot1.Width / 2,
                    Canvas.GetTop(ControlDot1) + ControlDot1.Height / 2);

                ControlPoint2 = new Point(
                    Canvas.GetLeft(ControlDot2) + ControlDot2.Width / 2,
                    Canvas.GetTop(ControlDot2) + ControlDot2.Height / 2);

                activeDot = null;
                EndEdit?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void ResetDotPositions()
        {
            Canvas.SetLeft(ControlDot1, 45);
            Canvas.SetTop(ControlDot1, 145);

            Canvas.SetLeft(ControlDot2, 145);
            Canvas.SetTop(ControlDot2, 45);

            DrawBezierCurve();
        }
    }
}
