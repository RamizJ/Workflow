using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Workflow.Tests.Services
{
    [TestFixture]
    public class ScopesServiceTests
    {
        private SqliteConnection _dbConnection;

        [SetUp]
        public void Setup()
        {
            _dbConnection = ContextHelper.OpenSqliteInMemoryConnection();
            using var dataContext = ContextHelper.CreateContext(_dbConnection, true);
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
