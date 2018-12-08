using Caliburn.Micro;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.ViewModels
{
    public class FigureControlViewModel : Screen
    {
        private FigureState figureState;
        public FigureState FigureState
        {
            get => figureState;
            set
            {
                figureState = value;
                NotifyOfPropertyChange(() => FigureState);
            }
        }

    }
}
