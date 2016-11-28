using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using MovieList.DB.Mongo.Entities;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MovieList.DB.Mongo
{
    [Log(AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Protected | MulticastAttributes.Internal | MulticastAttributes.Public)]
    public class MovieManager : IMovieManager
    {
        #region Members
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMapper _mapper;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion Members

        #region Constructors

        public MovieManager()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("MovieListDB");
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MovieItem, MovieItemEntity>().ReverseMap();
            });
            mapperConfig.AssertConfigurationIsValid();
            _mapper = mapperConfig.CreateMapper();
        }

        #endregion Constructors

        #region public methods

        public Task AddNewItem(MovieItem movie)
        {
            ResetCancellation();
            try
            {
                return GetFullCollection().InsertOneAsync(_mapper.Map<MovieItemEntity>(movie), null, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task DeleteItem(MovieItem movie)
        {
            return DeleteItemAsync(movie);
        }

        public Task<IList<MovieItem>> FilterAllMovies(string namePart, MovieTypes? movieType)
        {
            return FilterMoviesBuilder(namePart, movieType, false);
        }

        public Task<IList<MovieItem>> FilterAllUnseenMovies(string namePart, MovieTypes? movieType)
        {
            return FilterMoviesBuilder(namePart, movieType, true);
        }

        public Task<IList<MovieItem>> GetAllMovies()
        {
            ResetCancellation();
            return mapReturnListAsync(GetFullCollection().FindAsync((a) => true, null, _cancellationTokenSource.Token));
        }

        public Task<IList<MovieItem>> GetAllUnseenMovies()
        {
            ResetCancellation();
            return mapReturnListAsync(GetFullCollection().FindAsync((a) => a.Seen == false, null, _cancellationTokenSource.Token));
        }

        public Task UpdateItem(MovieItem movie)
        {
            return UpdateItemAsync(movie);
        }

        public Task ClearAll()
        {
            return GetFullCollection().DeleteManyAsync(a => true);
        }

        #endregion Methods

        #region Private methods

        async Task<IList<MovieItem>> FilterMoviesBuilder(string namePart, MovieTypes? movieType, bool showOnlyUnseen)
        {
            ResetCancellation();
            FilterDefinition<MovieItemEntity> filter = Builders<MovieItemEntity>.Filter.Empty;
            if (movieType != null)
                filter &= Builders<MovieItemEntity>.Filter.Eq("Type", movieType);
            if (showOnlyUnseen)
                filter &= Builders<MovieItemEntity>.Filter.Eq("Seen", false);
            if (!string.IsNullOrEmpty(namePart))
                filter &= Builders<MovieItemEntity>.Filter.Regex("Title", BsonRegularExpression.Create(new Regex(namePart, RegexOptions.IgnoreCase)));
            var resultList = GetFullCollection().FindAsync(filter, null, _cancellationTokenSource.Token);
            return await mapReturnListAsync(resultList);
        }

        async Task<IList<MovieItem>> mapReturnListAsync(Task<IAsyncCursor<MovieItemEntity>> source)
        {
            //for async testing
            //for (int i = 0; i < 5; i++)
            //    await Task.Delay(1000, _cancellationTokenSource.Token);
            return _mapper.Map<IList<MovieItemEntity>, IList<MovieItem>>((await source).ToList<MovieItemEntity>());
        }

        private IList<MovieItem> FilterMoviesExpression(string namePart, MovieTypes? movieType, bool showOnlyUnseen)
        {
            Expression<Func<MovieItemEntity, bool>> predicate;
            if (movieType.HasValue)
            {
                predicate = c => (c.Type == movieType) && (string.IsNullOrEmpty(namePart) || c.Title.Contains(namePart)) &&
                (!showOnlyUnseen || !c.Seen);
            }
            else
            {
                predicate = c => (string.IsNullOrEmpty(namePart) || c.Title.Contains(namePart)) &&
                (!showOnlyUnseen || !c.Seen);
            }
            var resultList = GetFullCollection().Find(predicate);
            return mapReturnList(resultList);
        }

        private void ResetCancellation()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }
            _cancellationTokenSource = new CancellationTokenSource(); 
        }

        async Task DeleteItemAsync(MovieItem movie)
        {
            await GetFullCollection().DeleteOneAsync(i => i.Id == movie.Id);
        }

        private IMongoCollection<MovieItemEntity> GetFullCollection()
        {
            return _database.GetCollection<MovieItemEntity>("Movies");
        }

        IList<MovieItem> mapReturnList(IFindFluent<MovieItemEntity, MovieItemEntity> source)
        {
            return _mapper.Map<IList<MovieItemEntity>, IList<MovieItem>>(source.ToList<MovieItemEntity>());
        }
        async Task UpdateItemAsync(MovieItem movie)
        {
            await GetFullCollection().FindOneAndReplaceAsync(Builders<MovieItemEntity>.Filter.Eq("Id", movie.Id), _mapper.Map<MovieItemEntity>(movie));
        }
        #endregion
    }
}
