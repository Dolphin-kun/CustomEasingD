using BezierEasing.Settings;
using BezierEasing.View.BezierEditor;
using System.Windows;
using System.Windows.Input;
using YukkuriMovieMaker.Commons;

namespace BezierEasing.View
{
    internal class BezierEditorViewModel : Bindable
    {
        public BezierMode BezierMode { get => beziermode; set => Set(ref beziermode, value); }
        BezierMode beziermode = BezierMode.Bezier;

        public BezierEditorCanvasViewModel EditorCanvasViewModel { get; } = new();

        public List<BezierMode> EnumValues { get; }
        private BezierMode _selectedEnumValue;
        public BezierMode SelectedEnumValue
        {
            get => _selectedEnumValue;
            set => Set(ref _selectedEnumValue, value);
        }
        

        private List<Point> _controlPoints = new();

        public List<Point> ControlPoints
        {
            get => _controlPoints;
            set
            {
                _controlPoints = value;
            }
        }

        public ICommand DeleteActivePointCommand { get; }
        public ActionCommand UndoCommand { get; }
        public ActionCommand RedoCommand { get; }

        public bool Undoable => undoHistory.Count > 0;
        public bool Redoable => redoHistory.Count > 0;
        bool isUndoRedoing = false;

        const int undoredoCapacity = 100;
        readonly List<Point> undoHistory = [];
        readonly List<Point> redoHistory = [];
        Point currentHistory = new();

        private int _activePointIndex = -1;
        public int ActivePointIndex
        {
            get => _activePointIndex;
            set
            {
                _activePointIndex = value;
            }
        }

        public BezierEditorViewModel()
        {
            EnumValues = Enum.GetValues<BezierMode>().Cast<BezierMode>().ToList();
            SelectedEnumValue = EnumValues.FirstOrDefault();

            DeleteActivePointCommand = new ActionCommand(
                _ => ActivePointIndex >= 0 && ActivePointIndex < ControlPoints.Count,
                _ => DeleteActivePoint()
            );
            UndoCommand = new ActionCommand(_ => Undoable, _ =>
            {
                isUndoRedoing = true;
                try
                {

                }
                finally
                {
                    isUndoRedoing = false;
                }
                OnPropertyChanged(nameof(Undoable));
                OnPropertyChanged(nameof(Redoable));
                UndoCommand?.RaiseCanExecuteChanged();
                RedoCommand?.RaiseCanExecuteChanged();
            });
            RedoCommand = new ActionCommand(_ => Redoable, _ =>
            {
                isUndoRedoing = true;
                try
                {
                    
                }
                finally
                {
                    isUndoRedoing = false;
                }
                OnPropertyChanged(nameof(Undoable));
                OnPropertyChanged(nameof(Redoable));
                UndoCommand?.RaiseCanExecuteChanged();
                RedoCommand?.RaiseCanExecuteChanged();
            });
        }

        private void DeleteActivePoint()
        {
            if (ActivePointIndex >= 0 && ActivePointIndex < ControlPoints.Count)
            {
                ControlPoints.RemoveAt(ActivePointIndex);
                ActivePointIndex = -1;

                OnPropertyChanged(nameof(ControlPoints));
            }
        }
    }
}
