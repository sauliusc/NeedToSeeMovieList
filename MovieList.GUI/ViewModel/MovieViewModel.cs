using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using MovieList.GUI.Models;
using MovieList.GUI.Navigation;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI.ViewModel
{
    [NotifyPropertyChanged]
    [Log(AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
    public class MovieViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Members
        IMovieManager _movieManager;

        #endregion Members

        #region Properties

        public RelayCommand AddNewItemCommand { get; set; }

        public RelayCommand<MovieItemModel> EditItemCommand { get; set; }

        public object FilterType { get; set; }

        public RelayCommand<MovieItemModel> MarkAsSeenCommand { get; set; }

        public ObservableCollection<MovieItemModel> MovieSource { get; set; }

        public string SearchText { get; set; }

        public bool ShowAll { get; set; }

        GuiObjectMapper GuiMapper
        {
            get
            {
                return SimpleIoc.Default.GetInstance<GuiObjectMapper>();
            }
        }

        #endregion Properties

        #region Constructors

        public MovieViewModel()
        {
            _movieManager = SimpleIoc.Default.GetInstance<IMovieManager>();
            AddNewItemCommand = new RelayCommand(AddNewItem);
            MarkAsSeenCommand = new RelayCommand<MovieItemModel>(MarkAsSeen);
            EditItemCommand = new RelayCommand<MovieItemModel>(EditItem);
            ShowAll = false;
            ReloadDataSource();
            Messenger.Default.Register<NavigateMessage>(this, Navigate);
        }

        #endregion Constructors

        #region Methods

        private void AddNewItem()
        {
            Messenger.Default.Send<NavigateMessage>(new NavigateMessage(WindowAction.AddNew, new MovieItemModel() { Priority = 5 }));
        }

        private void EditItem(MovieItemModel selectedItem)
        {
            Messenger.Default.Send<NavigateMessage>(new NavigateMessage(WindowAction.Update, selectedItem));
        }

        private void LoadAllMovies()
        {
            _movieManager.GetAllMovies();
        }

        private void MarkAsSeen(MovieItemModel parameter)
        {
            parameter.Seen = true;
            parameter.SeenTime = DateTime.Now;
            _movieManager.UpdateItem(GuiMapper.Mapper.Map<MovieItem>(parameter));
            ReloadDataSource();
        }

        private void Navigate(NavigateMessage message)
        {
            if (message.WindowAction == WindowAction.Close)
            {
                ReloadDataSource();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "ShowAll" || propertyName == "SearchText" || propertyName == "FilterType")
            {
                ReloadDataSource();
            }
        }

        private void ReloadDataSource()
        {
            try {
                IList<MovieItem> movies;
                MovieTypes? selectedType = null;
                if (FilterType is MovieTypes)
                    selectedType = (MovieTypes?)FilterType;
                if (this.ShowAll)
                {
                    if (string.IsNullOrEmpty(SearchText) && !selectedType.HasValue)
                    {
                        movies = _movieManager.GetAllMovies();
                    }
                    else
                    {
                        movies = _movieManager.FilterAllMovies(SearchText, selectedType);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(SearchText) && !selectedType.HasValue)
                    {
                        movies = _movieManager.GetAllUnseenMovies();
                    }
                    else
                    {
                        movies = _movieManager.FilterAllUnseenMovies(SearchText, selectedType);
                    }
                }
                MovieSource = GuiMapper.Mapper.Map<IList<MovieItem>, ObservableCollection<MovieItemModel>>(movies);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<ExceptionMessage>(new ExceptionMessage(ex));
            }
        }

        #endregion Methods
    }
}
