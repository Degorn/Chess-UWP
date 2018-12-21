using Windows.UI.Xaml.Controls;

namespace Chess_UWP.Controls
{
    public sealed partial class FigureView : UserControl
    {
        public FigureView()
        {
            this.InitializeComponent();
        }

        private void ScaleUp_Completed(object sender, object e)
        {
            MoveAnimation.Begin();
        }

        private void Move_Completed(object sender, object e)
        {
            ScaleDown.Begin();
        }
    }
}
