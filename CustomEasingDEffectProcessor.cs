using System.Diagnostics;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using CustomEasingD;
using System.Windows;

namespace CustomEasingD
{
    internal class CustomEasingDEffectProcessor : IVideoEffectProcessor
    {
        readonly CustomEasingDEffect item;
        ID2D1Image? input;
        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + " is null");

        public CustomEasingDEffectProcessor(IGraphicsDevicesAndContext devices, CustomEasingDEffect item)
        {
            this.item = item;
        }


        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var animationXStart = item.AnimateXStart;
            var animationXEnd = item.AnimateXEnd;

            double t = (double)frame / length;

            var control1 = item.ControlPoints.ControlPoint1;
            var control2 = item.ControlPoints.ControlPoint2;

            // ベジェ評価
            var bezier = new BezierEvaluator(control1, control2);
            double eased = bezier.Evaluate(t) / 200.0;

            float x = (float)(item.AnimateXStart + (item.AnimateXEnd - item.AnimateXStart) * eased);
            //float y = (float)(item.AnimateYStart + (item.AnimateYEnd - item.AnimateYStart) * eased);

            var drawDesc = effectDescription.DrawDescription;
            return drawDesc with
            {
                Draw = new(
                    drawDesc.Draw.X -(float)x ,
                    drawDesc.Draw.Y,
                    drawDesc.Draw.Z
                ),
            };
        }

        public class BezierEvaluator
        {
            public Point ControlPoint1 { get; set; }
            public Point ControlPoint2 { get; set; }

            private readonly Point P0 = new Point(0, 200); // 始点固定
            private readonly Point P3 = new Point(200, 0); // 終点固定

            public BezierEvaluator(Point control1, Point control2)
            {
                ControlPoint1 = control1;
                ControlPoint2 = control2;
            }

            public double Evaluate(double t)
            {
                double y = Math.Pow(1 - t, 3) * P0.Y +
                           3 * Math.Pow(1 - t, 2) * t * ControlPoint1.Y +
                           3 * (1 - t) * t * t * ControlPoint2.Y +
                           t * t * t * P3.Y;

                return y;
            }
        }

        public void ClearInput()
        {
            input = null;
        }

        public void SetInput(ID2D1Image? input)
        {
            this.input = input;
        }

        public void Dispose()
        {
        }
    }
}
