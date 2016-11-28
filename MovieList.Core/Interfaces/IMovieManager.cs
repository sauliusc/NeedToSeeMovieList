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
        Task<IList<MovieItem>> GetAllMovies();
        Task<IList<MovieItem>> GetAllUnseenMovies();
        Task<IList<MovieItem>> FilterAllMovies(string namePart, MovieTypes? movieType);
        Task<IList<MovieItem>> FilterAllUnseenMovies(string namePart, MovieTypes? movieType);
        Task AddNewItem(MovieItem movie);
        Task DeleteItem(MovieItem movie);
        Task UpdateItem(MovieItem movie);
        Task ClearAll();
    }
}
