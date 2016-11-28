using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MovieList.DB.File
{
    [Log(AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
    public class MovieManager : IMovieManager
    {
        #region Members
        SerializeHelper _szHelper;
        const string _storageFile = @"..\..\..\Data\File.DB\MovieDB.xml";
        IList<MovieItem> _allMovieList;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion Members

        #region Constructors

        public MovieManager()
        {
            _szHelper = new SerializeHelper();
            LoadListFromFile();
        }

        #endregion Constructors

        #region Public Methods

        public Task AddNewItem(MovieItem movie)
        {
            return GetVoidActionTask(() =>
            {
                _allMovieList.Add(movie);
                UpdateList(_allMovieList);
            });
        }

        public Task ClearAll()
        {
            return GetVoidActionTask(() => _szHelper.SerializeObject<IList<MovieItem>>(new List<MovieItem>(), _storageFile));
        }

        public Task DeleteItem(MovieItem movie)
        {
            return GetVoidActionTask(() =>
            {
                var movieToRemove = _allMovieList.FirstOrDefault(item => item.Id == movie.Id);
                _allMovieList.Remove(movieToRemove);
                UpdateList(_allMovieList);
            });
        }

        public Task<IList<MovieItem>> FilterAllMovies(string namePart, MovieTypes? movieType)
        {
            return GetListActionTask(() => { return _allMovieList.Where(item => String.IsNullOrEmpty(namePart) || item.Title.ToUpper().Contains(namePart.ToUpper())).Where(fType => !movieType.HasValue || fType.Type == movieType.Value).ToList<MovieItem>(); });
        }

        public Task<IList<MovieItem>> FilterAllUnseenMovies(string namePart, MovieTypes? movieType)
        {
            return GetListActionTask(() => { return _allMovieList.Where(itemf => !itemf.Seen).Where(item => String.IsNullOrEmpty(namePart) || item.Title.ToUpper().Contains(namePart.ToUpper())).Where(fType => !movieType.HasValue || fType.Type == movieType.Value).ToList<MovieItem>(); });
        }

        public Task<IList<MovieItem>> GetAllMovies()
        {
            return GetListActionTask(() => { return _allMovieList.OrderBy(item => item.Priority).ToList<MovieItem>(); });
        }

        public Task<IList<MovieItem>> GetAllUnseenMovies()
        {
            return GetListActionTask(() => { return _allMovieList.Where(itemf => !itemf.Seen).ToList<MovieItem>();});
        }

        public Task UpdateItem(MovieItem movie)
        {
            return GetVoidActionTask(() =>
            {
                var movieToRemove = _allMovieList.FirstOrDefault(item => item.Id == movie.Id);
                _allMovieList.Remove(movieToRemove);
                _allMovieList.Add(movie);
                UpdateList(_allMovieList);
            });
        }

        private void UpdateList(IList<MovieItem> movies)
        {
            _szHelper.SerializeObject<IList<MovieItem>>(movies, _storageFile);
        }
        #endregion Public Methods

        #region Private Methods

        private void ResetCancellation()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private Task<IList<MovieItem>> GetListActionTask(Func<IList<MovieItem>> filterFunction)
        {
            ResetCancellation();
            return Task.Run<IList<MovieItem>>(filterFunction, _cancellationTokenSource.Token);
        }

        private Task GetVoidActionTask(Action filterFunction)
        {
            ResetCancellation();

            return Task.Run(filterFunction, _cancellationTokenSource.Token);
        }

        private void LoadListFromFile()
        {
            _allMovieList = new List<MovieItem>();
            _allMovieList = _szHelper.DeSerializeObject<List<MovieItem>>(_storageFile);
        }
        #endregion Methods

    }
}
