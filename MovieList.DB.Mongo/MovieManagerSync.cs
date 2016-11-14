using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using MovieList.DB.Mongo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieList.DB.Mongo
{
    public class MovieManagerSync : IMovieManager
    {
        #region Members
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMapper _mapper;

        #endregion Members

        #region Constructors

        public MovieManagerSync()
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

        public bool AddNewItem(MovieItem movie)
        {
            try
            {
                GetFullCollection().InsertOne(_mapper.Map<MovieItemEntity>(movie));
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        public bool DeleteItem(MovieItem movie)
        {
            var result = GetFullCollection().DeleteOne(i => i.Id == movie.Id);
            return result.DeletedCount > 0;
        }

        public IList<MovieItem> FilterAllMovies(string namePart, MovieTypes? movieType)
        {
            return FilterMoviesBuilder(namePart, movieType, false);
        }

        public IList<MovieItem> FilterAllUnseenMovies(string namePart, MovieTypes? movieType)
        {
            return FilterMoviesBuilder(namePart, movieType, true);
        }

        public IList<MovieItem> GetAllMovies()
        {
            return mapReturnList(GetFullCollection().Find((a) => true).SortBy(i => i.Priority));
        }

        public IList<MovieItem> GetAllUnseenMovies()
        {
            return mapReturnList(GetFullCollection().Find((a) => a.Seen == false).SortBy(i => i.Priority));
        }

        private IMongoCollection<MovieItemEntity> GetFullCollection()
        {
            return _database.GetCollection<MovieItemEntity>("Movies");
        }

        IList<MovieItem> mapReturnList(IFindFluent<MovieItemEntity, MovieItemEntity> source)
        {
            return _mapper.Map<IList<MovieItemEntity>, IList<MovieItem>>(source.ToList<MovieItemEntity>());
        }

        public bool UpdateItem(MovieItem movie)
        {
            var result = GetFullCollection().FindOneAndReplace(Builders<MovieItemEntity>.Filter.Eq("Id", movie.Id), _mapper.Map<MovieItemEntity>(movie));
            return result != null;
        }

        #endregion Methods

        #region Private methods

        private IList<MovieItem> FilterMoviesBuilder(string namePart, MovieTypes? movieType, bool showOnlyUnseen)
        {
            FilterDefinition<MovieItemEntity> filter = Builders<MovieItemEntity>.Filter.Empty;
            if (movieType != null)
                filter &= Builders<MovieItemEntity>.Filter.Eq("Type", movieType);
            if (showOnlyUnseen)
                filter &= Builders<MovieItemEntity>.Filter.Eq("Seen", false);
            if (!string.IsNullOrEmpty(namePart))
                filter &= Builders<MovieItemEntity>.Filter.Regex("Title", BsonRegularExpression.Create(new Regex(namePart, RegexOptions.IgnoreCase)));
            var resultList = GetFullCollection().Find(filter);
            return mapReturnList(resultList);
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
        #endregion
    }
}
