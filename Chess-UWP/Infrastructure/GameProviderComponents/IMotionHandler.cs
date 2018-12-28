namespace Chess_UWP.Infrastructure.GameProviderComponents
{
    public interface IMotionHandler
    {
        event MoveDelegate Move;

        void FinalizeMove();
    }
}