using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieList.Core.Interfaces;
using MovieList.Core.SharedObjects;
using MovieList.DB.Mongo;
using System;
using System.Diagnostics;

namespace MovieList.UnitTests
{
    [TestClass]
    public class MongoMovieListTests
    {
        #region Members
        static IMovieManager _movieManager;
        static Process _startedMongoService;

        #endregion Members

        #region Methods

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            StartMongo();
            _movieManager = new MovieManager();
        }

        private static void CloseMongo()
        {
            int IDstring = System.Convert.ToInt32(_startedMongoService.Id.ToString());
            Process tempProc = Process.GetProcessById(IDstring);
            tempProc.CloseMainWindow();
            tempProc.WaitForExit();
        }

        private static void StartMongo()
        {
            _startedMongoService = new Process();
            _startedMongoService.StartInfo.FileName = @"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe";
            _startedMongoService.StartInfo.Arguments = @"--dbpath c:\Codding\NeedToSeeMovieList\Data\Mongo.DB.Tests\";
            _startedMongoService.StartInfo.CreateNoWindow = false;
            _startedMongoService.Start();
        }

        [ClassCleanup]
        public static void TestCleanUp()
        {
            _movieManager.ClearAll();
            CloseMongo();
        }

        [TestMethod]
        public void TestCleanupAction()
        {
            MovieItem movie = new MovieItem();
            movie.Title = "Testas1";
            movie.Duration = new DateTime(1, 1, 1, 1, 30, 0);
            _movieManager.AddNewItem(movie).Wait();
            var task = _movieManager.GetAllMovies();
            task.Wait();
            Assert.AreEqual(1, task.Result.Count, "Naujas irasas nepridetas");
            _movieManager.ClearAll().Wait();
            task = _movieManager.GetAllMovies();
            task.Wait();
            Assert.AreEqual(0, task.Result.Count, "duomenys nebuvo isvalyti");
        }

        [TestInitialize]
        //https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.testtools.unittesting.testinitializeattribute.aspx
        public void TestInitialize()
        {
            _movieManager.ClearAll();
        }

        #endregion Methods
    }
}
