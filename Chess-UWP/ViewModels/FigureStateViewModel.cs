using Caliburn.Micro;
using Chess_UWP.Models;

namespace Chess_UWP.ViewModels
{
    public class FigureStateViewModel : PropertyChangedBase
    {
        private FigureState figureState;
        public FigureState FigureState
        {
            get => figureState;
            set => NotifyOfPropertyChange(() => FigureState);
        }
    }
}
