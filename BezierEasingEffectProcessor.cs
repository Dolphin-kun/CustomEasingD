using BezierEasing.View;
using BezierEasing.View.BezierEditor;
using System.Diagnostics;
using System.Windows.Markup;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BezierEasing
{
    internal class BezierEasingEffectProcessor : IVideoEffectProcessor
    {
        readonly private IGraphicsDevicesAndContext devices;
        readonly BezierEasingEffect item;
        private ID2D1Image? input;
        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + " is null");


        public BezierEasingEffectProcessor(IGraphicsDevicesAndContext devices, BezierEasingEffect item)
        {
            this.devices = devices;
            this.item = item;
        }

        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var data = item.ControlPointsData;

            float x;

            if (data.ControlPoints.Count >= 2 && data.Segments.Count >= 1)
            {
                var easingFunction = new BezierEasingFunction(data.ControlPoints, data.Segments, data.CnavasSize);
                double rate = frame / (double)length;
                double easingRate = easingFunction.Evaluate(rate);

                Debug.WriteLine(data.ControlPoints);
                Debug.WriteLine(data.Segments);
                Debug.WriteLine(easingRate);
                Debug.WriteLine("-----------------");
            }
                
            

            var drawDesc = effectDescription.DrawDescription;
            return drawDesc with
            {
                Draw = new(
                    drawDesc.Draw.X,
                    drawDesc.Draw.Y,
                    drawDesc.Draw.Z
                ),
            };
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
