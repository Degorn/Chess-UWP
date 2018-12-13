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

        public double XPos => FigureState.Position.X * Board.CELL_SIZE;
        public double YPos => FigureState.Position.Y * Board.CELL_SIZE;

        private void FigureState_PositionChanged(object sender, EventArgs e)
        {
            Move?.Invoke(null, EventArgs.Empty);
            NotifyOfPropertyChange(() => XPos);
            NotifyOfPropertyChange(() => YPos);
        }

        public event EventHandler Move;
    }
}
