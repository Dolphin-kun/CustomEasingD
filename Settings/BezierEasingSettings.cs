using YukkuriMovieMaker.Plugin;

namespace BezierEasing.Settings
{
    internal class BezierEasingSettings : SettingsBase<BezierEasingSettings>
    {
        public override SettingsCategory Category => SettingsCategory.None;

        public override string Name => "ベジェ軌道プラグイン";

        public override bool HasSettingView => false;

        public override object? SettingView => throw new NotImplementedException();

        public BezierMode BezierMode { get => bezierMode; set => Set(ref bezierMode, value); }
        BezierMode bezierMode = BezierMode.Bezier;

        public override void Initialize()
        {
        }
    }
}
