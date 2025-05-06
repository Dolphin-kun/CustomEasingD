using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BezierEasing.View
{
    public partial class BezierEditorWindow : Window
    {
        
        public BezierEditorWindow()
        {
            InitializeComponent();
            DataContext = new BezierEditorViewModel();
        }

    }
}
