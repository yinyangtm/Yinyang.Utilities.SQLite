using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yinyang.Utilities.SQLite.Test
{
    [TestClass]
    public class SqliteTest
    {
        private const string ConnectionString =
            "DataSource = \"TestDB.db\"";

        [TestMethod]
        public void EasySelect()
        {
            Sqlite.ConnectionString = ConnectionString;
            using (var db = new Sqlite(ConnectionString))
            {
                db.AddParameter("@id", 1);

                var result = db.EasySelect<EntityTest>("select * from test where \"id\" = @id;").First();

                var answer = new EntityTest {id = 1, key = 1, value = "あいうえお"};
                Assert.AreEqual(answer.id, result.id);
                Assert.AreEqual(answer.key, result.key);
                Assert.AreEqual(answer.value, result.value);
                db.Close();
            }
        }

        [TestMethod]
        public void ExecuteReaderFirst()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();
                db.CommandText = "select * from test where \"id\" = @id;";
                db.AddParameter("@id", 1);
                var result = db.ExecuteReaderFirst<EntityTest>();

                var answer = new EntityTest {id = 1, key = 1, value = "あいうえお"};
                Assert.AreEqual(answer.id, result.id);
                Assert.AreEqual(answer.key, result.key);
                Assert.AreEqual(answer.value, result.value);
                db.Close();
            }
        }

        [TestMethod]
        public void Full()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();

                db.CommandText = "CREATE TABLE test3 ( id INTEGER NOT NULL, value TEXT NOT NULL, PRIMARY KEY ( id ) );";
                db.ExecuteNonQuery();

                db.BeginTransaction();

                db.CommandText = "INSERT INTO test3 VALUES(@id, @value)";
                db.AddParameter("@id", 1);
                db.AddParameter("@value", "あいうえお");
                Assert.AreEqual(1, db.ExecuteNonQuery());

                Assert.AreEqual(1, db.TableRowsCount("test3"));

                db.Refresh();

                db.CommandText = "select * from test3 where \"id\" = @id;";
                db.AddParameter("@id", 1);
                var result = db.ExecuteReaderFirst<EntityTest>();

                var answer = new EntityTest {id = 1, value = "あいうえお"};
                Assert.AreEqual(answer.id, result.id);
                Assert.AreEqual(answer.key, result.key);
                Assert.AreEqual(answer.value, result.value);

                db.Rollback();
                Assert.AreEqual(0, db.TableRowsCount("test3"));


                db.CommandText = "drop table test3;";
                db.ExecuteNonQuery();
                db.Close();
            }
        }

        [TestMethod]
        public void Insert()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();
                db.BeginTransaction();

                db.CommandText = "INSERT INTO test2 VALUES(@id, @value)";
                db.AddParameter("@id", 1);
                db.AddParameter("@value", "あいうえお");
                Assert.AreEqual(1, db.ExecuteNonQuery());

                Assert.AreEqual(1, db.TableRowsCount("test2"));

                db.Refresh();

                db.CommandText = "select * from test2 where \"id\" = @id;";
                db.AddParameter("@id", 1);
                var result = db.ExecuteReaderFirst<EntityTest>();

                var answer = new EntityTest {id = 1, value = "あいうえお"};
                Assert.AreEqual(answer.id, result.id);
                Assert.AreEqual(answer.key, result.key);
                Assert.AreEqual(answer.value, result.value);

                db.Rollback();
                Assert.AreEqual(0, db.TableRowsCount("test2"));

                db.Close();
            }
        }

        [TestMethod]
        public void Select()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();
                db.CommandText = "select * from test where \"id\" = @id;";
                db.AddParameter("@id", 1);
                var result = db.ExecuteReader<EntityTest>().First();

                var answer = new EntityTest {id = 1, key = 1, value = "あいうえお"};
                Assert.AreEqual(answer.id, result.id);
                Assert.AreEqual(answer.key, result.key);
                Assert.AreEqual(answer.value, result.value);
                db.Close();
            }
        }

        [TestMethod]
        public void SelectCount()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();
                db.CommandText = "select count(*) from test where \"id\" = @id;";
                db.AddParameter("@id", 1);
                var result = db.ExecuteScalarToInt();

                Assert.AreEqual(1, result);
                db.Close();
            }
        }

        [TestMethod]
        public void TableRowsCount()
        {
            using (var db = new Sqlite(ConnectionString))
            {
                db.Open();
                Assert.AreEqual(1, db.TableRowsCount("test"));
                db.Close();
            }
        }
    }
}
