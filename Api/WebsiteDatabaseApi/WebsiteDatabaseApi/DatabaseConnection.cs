using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace WebsiteDatabaseApi
{
    public class DatabaseConnection
    {
        private string ConnectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void CreateUser(string FirstName, string LastName, string Street, int StreetNumber, string City, int PostNumber, string Email)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "INSERT INTO Users (FirstName, LastName, Street, StreetNumber, City, PostNumber, Email) VALUES (@FirstName, @LastName, @Street, @StreetNumber, @City, @PostNumber, @Email)";
                cnn.Execute(sql, new { FirstName = FirstName, LastName = LastName, Street = Street, StreetNumber = StreetNumber, City = City, PostNumber = PostNumber, Email = Email });
            }
        }

        public List<UserModel> LoadUsers()
        {
            using(IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "";
                var query = cnn.Query<UserModel>("SELECT * FROM USERS");
                return query.ToList();
            }
        }

        public List<ReviewModel> ReviewsForProduct(int Id)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM Review INNER JOIN Products ON Review.ProductId = @Id";
                var output = cnn.Query<ReviewModel>(sql, new {Id = Id});
                return output.ToList();
            }
        }
    }
}
