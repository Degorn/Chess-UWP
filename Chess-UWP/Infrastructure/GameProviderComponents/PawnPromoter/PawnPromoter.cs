using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System.Collections.Generic;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure.GameProviderComponents.PawnPromoter
{
    public class PawnPromoter : IPawnPromoter
    {
        public event PawnPromotionDelegate StartPawnPromotion;

        private readonly IGameProvider gameProvider;
        private FigureState pawnToPromotion;

        public PawnPromoter(IGameProvider gameProvider)
        {
            this.gameProvider = gameProvider;
            this.gameProvider.Moved += GameProvider_Moved;
        }

        private void GameProvider_Moved(object sender, MovedEventArgs e)
        {
            if (CheckPawnPromotion(e.Figure))
            {
                pawnToPromotion = e.Figure;
                StartPawnPromotion(this, new PawnPromotionEventArgs
                {
                    Types = GetPawnPromotionTypes()
                });
            }
        }

        private bool CheckPawnPromotion(FigureState pawn)
        {
            if (pawn.Figure.GetType() != typeof(Pawn))
            {
                return false;
            }

            return pawn.Color == Color.Black && pawn.Position.Y == BOARD_HEIGHT - 1 ||
                   pawn.Color == Color.White && pawn.Position.Y == 0;
        }

        private IEnumerable<PawnPromotionType> GetPawnPromotionTypes()
        {
            return new PawnPromotionType[]
            {
                PawnPromotionType.Rook, PawnPromotionType.Knight, PawnPromotionType.Bishop, PawnPromotionType.Queen
            };
        }

        public void PromotePawn(PawnPromotionType type)
        {
            if (pawnToPromotion.Figure.GetType() != typeof(Pawn))
            {
                return;
            }

            Figure figure;
            switch (type)
            {
                case PawnPromotionType.Rook:
                    figure = new Rook();
                    break;
                case PawnPromotionType.Knight:
                    figure = new Knight();
                    break;
                case PawnPromotionType.Bishop:
                    figure = new Bishop();
                    break;
                case PawnPromotionType.Queen:
                    figure = new Queen();
                    break;
                default: return;
            }

            gameProvider.AddFigure(figure, pawnToPromotion.Position, pawnToPromotion.Color);
            gameProvider.RemoveFigure(pawnToPromotion);
        }
    }
}
