using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MovieList.Core.SharedObjects;
using MovieList.GUI.Navigation;
using MovieList.GUI.ViewModel;
using MovieList.GUI.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieList.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window _activeWindow;
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NavigateMessage>(this, Navigate);
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Messenger.Default.Send<ExceptionMessage>(new ExceptionMessage(e.ExceptionObject as Exception));
        }

        private void Navigate(NavigateMessage message)
        {
            if (message.WindowAction == WindowAction.Close)
            {
                _activeWindow.Close();
            }
            else
            {
                var movieEditViewModel = SimpleIoc.Default.GetInstance<MovieEditViewModel>();
                movieEditViewModel.EditableItem = message.Context;
                _activeWindow = new MovieEditWindow() { DataContext = movieEditViewModel };
                _activeWindow.ShowDialog();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
