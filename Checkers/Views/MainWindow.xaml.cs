using System.Windows.Controls;
using System.Windows.Input;
using Checkers.ViewModels;

namespace Checkers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image { DataContext: BoardSquare boardSquare }) return;
            if (DataContext is GameViewModel { PieceClickedCommand: not null } viewModel)
            {
                viewModel.PieceClickedCommand.Execute(boardSquare);
            }
        }

    }
}