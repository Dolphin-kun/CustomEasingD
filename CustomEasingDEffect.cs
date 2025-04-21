using CustomEasingD.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace CustomEasingD
{
    [VideoEffect("カスタムイージングD", ["さんぷる"], [], isAviUtlSupported: false)]
    internal class CustomEasingDEffect : VideoEffectBase
    {
        public override string Label => "カスタムイージングD";

        [Display(GroupName = "カスタムイージングD", Name = "", Description = "")]
        [BezierControl]
        public BezierData ControlPoints { get; set; } = new();

        [Display(GroupName = "カスタムイージングD", Name = "始点", Description = "始点")]
        [Range(-10000, 10000)]
        [TextBoxSlider("F1", "px", -100, 100)]
        [DefaultValue(0)]
        public double AnimateXStart { get => animateXStart; set => Set(ref animateXStart, value); }
        double animateXStart = 0;

        [Display(GroupName = "カスタムイージングD", Name = "終点", Description = "終点")]
        [Range(-10000, 10000)]
        [TextBoxSlider("F1", "px", -100, 100)]
        [DefaultValue(0)]
        public double AnimateXEnd { get => animateYEnd; set => Set(ref animateYEnd, value); }
        double animateYEnd = 0;


        public class BezierData
        {
            public Point ControlPoint1 { get; set; }
            public Point ControlPoint2 { get; set; }

            public BezierData()
            {
                ControlPoint1 = new Point(45, 145);
                ControlPoint2 = new Point(145, 45);
            }
        }


        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new CustomEasingDEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
