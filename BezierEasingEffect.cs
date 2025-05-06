using BezierEasing.View;
using BezierEasing.View.BezierEditor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace BezierEasing
{
    [VideoEffect("BezierEasing", ["さんぷる"], [""], isAviUtlSupported: false)]
    internal class BezierEasingEffect : VideoEffectBase
    {
        public override string Label => "BezierEasing";

        [Display(GroupName = "", Name = "", Description = "")]
        [OpenBezierEditorButton]
        public BezierData ControlPointsData { get; set; } = new();

        [Display(GroupName = "", Name = "X", Description = "X")]
        [Range(-10000, 10000)]
        [TextBoxSlider("F1", "px", -100, 100)]
        [DefaultValue(0)]
        public double X { get => x; set => Set(ref x, value); }
        double x = 1;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new BezierEasingEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
