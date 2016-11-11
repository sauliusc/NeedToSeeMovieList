using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        #endregion Members

        #region Constructors

        public MovieManager()
        {
            _szHelper = new SerializeHelper();
            _allMovieList = GetAllMovies();
        }

        #endregion Constructors

        #region Methods

        public bool AddNewItem(MovieItem movie)
        {
            _allMovieList.Add(movie);
            return UpdateList(_allMovieList);
        }

        public bool DeleteItem(MovieItem movie)
        {
            var movieToRemove = _allMovieList.FirstOrDefault(item => item.Guid == movie.Guid);
            _allMovieList.Remove(movieToRemove);
            return UpdateList(_allMovieList);
        }

        public IList<MovieItem> FilterAllMovies(string namePart, MovieTypes? movieType)
        {
            return _allMovieList.Where(item => String.IsNullOrEmpty(namePart) || item.Title.ToUpper().Contains(namePart.ToUpper())).Where(fType => !movieType.HasValue || fType.Type == movieType.Value).ToList<MovieItem>();
        }

        public IList<MovieItem> FilterAllUnseenMovies(string namePart, MovieTypes? movieType)
        {
            return GetAllUnseenMovies().Where(item => String.IsNullOrEmpty(namePart) || item.Title.ToUpper().Contains(namePart.ToUpper())).Where(fType => !movieType.HasValue || fType.Type == movieType.Value).ToList<MovieItem>();
        }

        public IList<MovieItem> GetAllMovies()
        {
            List<MovieItem> allMovies = new List<MovieItem>();
            allMovies = _szHelper.DeSerializeObject<List<MovieItem>>(_storageFile);
            return allMovies.OrderBy(item => item.Priority).ToList<MovieItem>();
        }

        public IList<MovieItem> GetAllUnseenMovies()
        {
            return _allMovieList.Where(itemf => !itemf.Seen).ToList<MovieItem>();
        }

        public bool UpdateItem(MovieItem movie)
        {
            var movieToRemove = _allMovieList.FirstOrDefault(item => item.Guid == movie.Guid);
            _allMovieList.Remove(movieToRemove);
            _allMovieList.Add(movie);
            return UpdateList(_allMovieList);
        }

        private bool UpdateList(IList<MovieItem> movies)
        {
            _szHelper.SerializeObject<IList<MovieItem>>(movies, _storageFile);
            return true;
        }

        #endregion Methods
    }
}
