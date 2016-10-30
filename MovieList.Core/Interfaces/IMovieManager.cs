using MovieList.Core.SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Core.Interfaces
{
    public interface IMovieManager
    {
        IList<MovieItem> GetAllMovies();
        IList<MovieItem> GetAllUnseenMovies();
        IList<MovieItem> FilterAllMovies(string namePart, MovieTypes? movieType);
        IList<MovieItem> FilterAllUnseenMovies(string namePart, MovieTypes? movieType);
        bool AddNewItem(MovieItem movie);
        bool DeleteItem(MovieItem movie);
        bool UpdateItem(MovieItem movie);
    }
}
