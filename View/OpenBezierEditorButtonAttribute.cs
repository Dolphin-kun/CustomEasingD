using System.Windows;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Views.Converters;

namespace BezierEasing.View
{
    internal class OpenBezierEditorButtonAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new OpenBezierEditorButton();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if(control is not OpenBezierEditorButton editor)
                return;
            editor.ItemProperties = itemProperties;
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not OpenBezierEditorButton editor)
                return;
            editor.ItemProperties = null;
        }
    }
}
