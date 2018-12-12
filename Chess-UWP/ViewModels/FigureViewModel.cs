using Caliburn.Micro;
using Chess_UWP.Models;
using System;

namespace Chess_UWP.ViewModels
{
    public class FigureViewModel : Screen
    {
        private FigureState figureState;
        public FigureState FigureState
        {
            get => figureState;
            set
            {
                figureState = value;
                NotifyOfPropertyChange(() => FigureState);
                FigureState.PositionChanged += FigureState_PositionChanged;
            }
        }

        private void FigureState_PositionChanged(object sender, EventArgs e)
        {
            Move?.Invoke(null, EventArgs.Empty);
            NotifyOfPropertyChange(() => FigureState);
        }

        public event EventHandler Move;
    }
}
