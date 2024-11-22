using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp4.ViewModel;

namespace WpfApp4.View
{
    /// <summary>
    /// Interaction logic for UpdatedView.xaml
    /// </summary>
    public partial class UpdatedView : Window
    {
        public UpdatedView()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            DataContext = viewModel;
        }
    }
}
