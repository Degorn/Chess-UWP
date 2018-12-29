namespace Chess_UWP.Infrastructure.GameProviderComponents
{
    public interface IMotionHandler
    {
        event MovingDelegate Moving;
        event MovedDelegate Moved;

        void FinalizeMove();
    }
}