using AutoMapper;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using MovieList.GUI.Models;
using MovieList.GUI.Navigation;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.GUI.ViewModel
{
    [NotifyPropertyChanged]
    public class MovieEditViewModel
    {
        #region Properties

        public RelayCommand AddItemCommand { get; set; }

        public MovieItemModel EditableItem { get; set; }

        public RelayCommand RemoveItemCommand { get; set; }

        #endregion Properties

        #region Constructors

        public MovieEditViewModel()
        {
            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
        }

        #endregion Constructors

        #region Methods

        private void AddItem()
        {
            throw new Exception("444");
            if (string.IsNullOrEmpty(EditableItem.Guid))
            {
                EditableItem.Guid = Guid.NewGuid().ToString();
                EditableItem.CreateDate = DateTime.Now;
                SimpleIoc.Default.GetInstance<IMovieManager>().AddNewItem(Mapper.Map<MovieItem>(EditableItem));
            }
            else
            {
                SimpleIoc.Default.GetInstance<IMovieManager>().UpdateItem(Mapper.Map<MovieItem>(EditableItem));
            }
            Messenger.Default.Send<NavigateMessage>(new NavigateMessage(WindowAction.Close));
        }

        private void RemoveItem()
        {
            SimpleIoc.Default.GetInstance<IMovieManager>().DeleteItem(Mapper.Map<MovieItem>(EditableItem));
            Messenger.Default.Send<NavigateMessage>(new NavigateMessage(WindowAction.Close));
        }

        #endregion Methods
    }
}
