using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SqlBulkTools.TestCommon.Model;
using System.Collections.Generic;
using System.Linq;

namespace SqlBulkTools.Net.IntegrationTests.Data
{
    public class DataAccess
    {
        public DataAccess()
        {
            SqlMapper.SetTypeMap(typeof(CustomColumnMappingTest), new ColumnAttributeTypeMapper<CustomColumnMappingTest>());
        }

        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    var config = new ConfigurationBuilder()
                        .AddJsonFile("appconfig.json")
                        .Build();
                    _connectionString = config["connectionString"];
                }
                return _connectionString;
            }
        }

        public List<Book> GetBookList(string isbn = null)
        {
            using var connection = new SqlConnection(ConnectionString);
            return connection
                .Query<Book>("dbo.GetBooks", new { Isbn = isbn })
                .ToList();
        }

        public int GetBookCount()
        {
            using var connection = new SqlConnection(ConnectionString);
            return connection.QuerySingle<int>("dbo.GetBookCount");
        }

        public List<SchemaTest1> GetSchemaTest1List()
        {
            using var connection = new SqlConnection(ConnectionString);

            return connection
                .Query<SchemaTest1>("dbo.GetSchemaTest", new { Schema = "dbo" })
                .ToList();
        }

        public List<SchemaTest2> GetSchemaTest2List()
        {
            using var connection = new SqlConnection(ConnectionString);

            return connection
                .Query<SchemaTest2>("dbo.GetSchemaTest", new { Schema = "AnotherSchema" })
                .ToList();
        }

        public List<CustomColumnMappingTest> GetCustomColumnMappingTests()
        {
            using var connection = new SqlConnection(ConnectionString);

            //.CustomColumnMapping<CustomColumnMappingTest>(x => x.NaturalIdTest, "NaturalId")
            //.CustomColumnMapping<CustomColumnMappingTest>(x => x.ColumnXIsDifferent, "ColumnX")
            //.CustomColumnMapping<CustomColumnMappingTest>(x => x.ColumnYIsDifferentInDatabase, "ColumnY")

            var result = connection
                .Query<CustomColumnMappingTest>("dbo.GetCustomColumnMappingTests", commandType: System.Data.CommandType.StoredProcedure)
                .ToList();

            return result;
        }

        public List<ReservedColumnNameTest> GetReservedColumnNameTests()
        {
            using var connection = new SqlConnection(ConnectionString);

            return connection
                .Query<ReservedColumnNameTest>("dbo.GetReservedColumnNameTests")
                .ToList();
        }

        public int GetComplexTypeModelCount()
        {
            using var connection = new SqlConnection(ConnectionString);

            return connection.QuerySingle<int>("dbo.GetComplexModelCount");
        }

        public void ReseedBookIdentity(int idStart)
        {
            using var connection = new SqlConnection(ConnectionString);

            connection
                .Execute("dbo.ReseedBookIdentity", new { IdStart = idStart });
        }

        public List<CustomIdentityColumnNameTest> GetCustomIdentityColumnNameTestList()
        {
            using var connection = new SqlConnection(ConnectionString);

            return connection
                .Query<CustomIdentityColumnNameTest>("dbo.GetCustomIdentityColumnNameTestList")
                .ToList();
        }
    }
}
