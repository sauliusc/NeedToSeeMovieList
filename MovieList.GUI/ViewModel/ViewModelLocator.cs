using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using MovieList.Core.Interfaces;
using MovieList.DB.File;
using System.Windows;

namespace MovieList.GUI.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Properties

        public MovieEditViewModel MovieEditViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MovieEditViewModel>();
            }
        }

        public MovieViewModel MovieViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MovieViewModel>();
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MovieViewModel>();
            SimpleIoc.Default.Register<MovieEditViewModel>();
            SimpleIoc.Default.Register<IMovieManager, MovieManager>();
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);
        }

        #endregion Constructors

        #region Methods

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        private void NotifyUserMethod(NotificationMessage message)
        {
            MessageBox.Show(message.Notification);
        }

        #endregion Methods
    }
}
