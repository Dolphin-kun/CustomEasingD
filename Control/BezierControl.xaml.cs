using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using YukkuriMovieMaker.Commons;
using static CustomEasingD.CustomEasingDEffect;

namespace CustomEasingD.Control
{
    public partial class BezierControl : IPropertyEditorControl2
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;
        IEditorInfo? EditorInfo;

        public ItemProperty[]? ItemProperties { get; set; }

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

        public void SetEditorInfo(IEditorInfo info)
        {
            EditorInfo = info;
        }
        
        private void DrawBezierCurve()
        {
            var dot1Position = new Point(0, 200);
            var dot2Position = new Point(200, 0);

            var controlPoint1 = new Point(Canvas.GetLeft(ControlDot1) + ControlDot1.Width / 2, Canvas.GetTop(ControlDot1) + ControlDot1.Height / 2);
            var controlPoint2 = new Point(Canvas.GetLeft(ControlDot2) + ControlDot2.Width / 2, Canvas.GetTop(ControlDot2) + ControlDot2.Height / 2);

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure
            {
                StartPoint = dot1Position
            };

            BezierSegment bezierSegment = new BezierSegment(controlPoint1, controlPoint2, dot2Position, true);
            figure.Segments.Add(bezierSegment);

            geometry.Figures.Add(figure);

            BezierPath.Data = geometry;
        }



        private void ControlDot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Ellipse dot)
            {
                BeginEdit?.Invoke(this,EventArgs.Empty);

                isDragging = true;
                activeDot = dot;
                mouseOffset = e.GetPosition(DrawCanvas);
                mouseOffset.X -= Canvas.GetLeft(dot);
                mouseOffset.Y -= Canvas.GetTop(dot);
                dot.CaptureMouse();
            }
        }

        private void ControlDot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging && activeDot != null)
            {
                var position = e.GetPosition(DrawCanvas);
                double newLeft = position.X - mouseOffset.X;
                double newTop = position.Y - mouseOffset.Y;

                newLeft = Math.Max(0, Math.Min(DrawCanvas.ActualWidth - activeDot.Width, newLeft));
                newTop = Math.Max(0, Math.Min(DrawCanvas.ActualHeight - activeDot.Height, newTop));
                Canvas.SetLeft(activeDot, newLeft);
                Canvas.SetTop(activeDot, newTop);

                DrawBezierCurve();
            }
        }

        private void ControlDot_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (activeDot != null)
            {
                isDragging = false;
                activeDot.ReleaseMouseCapture();
                activeDot = null;

                if (ItemProperties is [var prop])
                {
                    var bezier = new BezierData
                    {
                        ControlPoint1 = new Point(Canvas.GetLeft(ControlDot1), Canvas.GetTop(ControlDot1)),
                        ControlPoint2 = new Point(Canvas.GetLeft(ControlDot2), Canvas.GetTop(ControlDot2))
                    };

                    prop.SetValue(bezier);
                }

                EndEdit?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ResetDotPositions()
        {
            Canvas.SetLeft(ControlDot1, 45);
            Canvas.SetTop(ControlDot1, 145);

            Canvas.SetLeft(ControlDot2, 145);
            Canvas.SetTop(ControlDot2, 45);

            DrawBezierCurve();
        }
    }
}
