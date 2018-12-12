using Chess_UWP.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Chess_UWP.Controls
{
    public sealed partial class FigureView : UserControl
    {
        public FigureViewModel FigureModel { get; set; }

        public FigureView()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FigureModel = (DataContext as FigureViewModel);
            FigureModel.Move += FigureModel_Move;
        }

        private void FigureModel_Move(object sender, EventArgs e)
        {
             MyStoryboard.Begin();
        }
    }
}
