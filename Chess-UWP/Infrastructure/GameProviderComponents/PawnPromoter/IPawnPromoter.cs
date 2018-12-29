namespace Chess_UWP.Infrastructure.GameProviderComponents.PawnPromoter
{
    public interface IPawnPromoter
    {
        event PawnPromotionDelegate StartPawnPromotion;

        void PromotePawn(PawnPromotionType type);
    }
}
