using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Views.Converters;

namespace CustomEasingD.Control
{
    internal class BezierControlAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new BezierControl();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            var editor = (BezierControl)control;
            editor.SetBinding(BezierControl.ControlPoint1Property,ItemPropertiesBinding.Create(itemProperties));
            editor.SetBinding(BezierControl.ControlPoint2Property, ItemPropertiesBinding.Create(itemProperties));
            editor.ResetDotPositions();
        }
        public override void ClearBindings(FrameworkElement control)
        {
            BindingOperations.ClearBinding(control, BezierControl.ControlPoint1Property);
            BindingOperations.ClearBinding(control, BezierControl.ControlPoint2Property);
        }
    }
}