using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using YukkuriMovieMaker.Commons;

namespace BezierEasing.View
{
    public partial class OpenBezierEditorButton : UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ItemProperty[]? ItemProperties { get; set; }

        public ICommand Button_Click { get; }

        public OpenBezierEditorButton()
        {
            Button_Click = new ActionCommand(
                _ => true,
                _ =>
                {
                    BeginEdit?.Invoke(this, EventArgs.Empty);

                    if (ItemProperties?.FirstOrDefault()?.PropertyOwner is BezierEasingEffect effect)
                    {
                        var vm = new BezierEditorViewModel();
                        vm.EditorCanvasViewModel.LoadFromBezierData(effect.ControlPointsData);

                        // ベジェ曲線編集→即反映
                        vm.EditorCanvasViewModel.BezierDataChanged += (_, _) =>
                        {
                            effect.ControlPointsData = vm.EditorCanvasViewModel.ExportToBezierData();

                            Debug.WriteLine(effect.ControlPointsData);
                            // 通知（YMM4エディタ更新）
                            foreach (var itemProp in ItemProperties ?? [])
                            {
                                var newValue = vm.EditorCanvasViewModel.ExportToBezierData();
                                itemProp.SetValue(newValue);
                            }
                        };

                        var window = new BezierEditorWindow
                        {
                            Owner = Window.GetWindow(this),
                            DataContext = vm
                        };
                        window.Show();
                    }


                    EndEdit?.Invoke(this, EventArgs.Empty);
                });

            InitializeComponent();
        }
    }
}
