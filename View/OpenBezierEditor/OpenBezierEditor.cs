using BezierEasing.View.BezierEditor;
using YukkuriMovieMaker.Plugin;

namespace BezierEasing.View.OpenBezierEditor
{
    class OpenBezierEditor : IToolPlugin
    {
        public string Name => "さんぷる";
        public Type ViewModelType => typeof(OpenBezierEditorViewModel) ;
        public Type ViewType => typeof(BezierEditorCanvas);
    }
}
