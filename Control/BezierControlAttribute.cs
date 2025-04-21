using System.Windows;
using System.Windows.Controls;
using YukkuriMovieMaker.Commons;
using static CustomEasingD.CustomEasingDEffect;

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
            if (control is not BezierControl editor)
                return;

            editor.ItemProperties = itemProperties;
            editor.ResetDotPositions();

            if (itemProperties is [var prop])
            {
                var data = prop.GetValue<BezierData>();
                if (data.ControlPoint1 != default && data.ControlPoint2 != default)
                {
                    // 外部データを制御点に反映
                    Canvas.SetLeft(editor.ControlDot1, data.ControlPoint1.X);
                    Canvas.SetTop(editor.ControlDot1, data.ControlPoint1.Y);
                    Canvas.SetLeft(editor.ControlDot2, data.ControlPoint2.X);
                    Canvas.SetTop(editor.ControlDot2, data.ControlPoint2.Y);
                }
            }
        }
        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not BezierControl editor)
                return;
            editor.ItemProperties = null;
        }
    }
}