using Chess_UWP.Models;

namespace Chess_UWP.ViewModels
{
    class FigureStateViewModel : NotificationBase
    {
        private FigureState figureState;
        public FigureState FigureState
        {
            get => figureState;
            set => SetProperty(ref figureState, value);
        }
    }
}
