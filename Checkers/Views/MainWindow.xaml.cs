using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Checkers.ViewModels;
using Checkers.Views;

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
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            if (radioButton.IsChecked != true) return;
            ((GameViewModel)DataContext).MultiJumps = radioButton.Content.ToString() == "Enabled";
        }

        private void AboutButton_OnClick(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}