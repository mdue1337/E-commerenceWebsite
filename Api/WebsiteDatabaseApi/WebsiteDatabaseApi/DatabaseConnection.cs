using Dapper;
using System;
using System.Buffers.Text;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Transactions;

namespace WebsiteDatabaseApi
{
    public class DatabaseConnection
    {
        private string ConnectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // User querys.

        public List<UserModel> LoadUsers()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM USERS";
                var query = cnn.Query<UserModel>(sql);
                return query.ToList();
            }
        }

        public void CreateUser(string FirstName, string LastName, string Street, int StreetNumber, string City, int PostNumber, string Email)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "INSERT INTO Users (FirstName, LastName, Street, StreetNumber, City, PostNumber, Email) VALUES (@FirstName, @LastName, @Street, @StreetNumber, @City, @PostNumber, @Email)";
                cnn.Execute(sql, new { FirstName = FirstName, LastName = LastName, Street = Street, StreetNumber = StreetNumber, City = City, PostNumber = PostNumber, Email = Email });
            }
        }

        // Product querys.

        public List<ReviewModel> ReviewsForProduct(int Id)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM Review INNER JOIN Products ON Review.ProductId = @Id";
                var output = cnn.Query<ReviewModel>(sql, new { Id = Id });
                return output.ToList();
            }
        }

        public string CreateListingClothes(int[] sizes, string color, string brand, string name, double price, byte[] picture)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    int categoryId = 1;

                    cnn.Open();

                    using (IDbTransaction transaction = cnn.BeginTransaction())
                    {
                        try
                        {
                            string sql1 = "INSERT INTO ClothingSizes (Small, Medium, Large, XL) VALUES (@Small, @Medium, @Large, @XL); SELECT last_insert_rowid();";
                            int ClothingSizeId = cnn.QuerySingleOrDefault<int>(sql1, new { Small = sizes[0], Medium = sizes[1], Large = sizes[2], XL = sizes[3] });

                            if (ClothingSizeId == 0)
                            {
                                throw new Exception("Failed to retrieve id");
                            }

                            string sql2 = "INSERT INTO ClothingProperties (SizeId, Color, Brand) VALUES (@SizeId, @Color, @Brand); SELECT last_insert_rowid();";
                            int ClothingPropertiesId = cnn.QuerySingleOrDefault<int>(sql2, new { SizeId = ClothingSizeId, Color = color, Brand = brand });

                            if (ClothingPropertiesId == 0)
                            {
                                throw new Exception("Failed to retrieve id");
                            }

                            string sql3 = "INSERT INTO Products (Name, CategoryId, Price, PictureBytes, ClothingPropertiesId) VALUES (@Name, @CategoryId, @Price, @PictureBytes, @ClothingPropertiesId)";
                            cnn.Execute(sql3, new { Name = name, CategoryId = categoryId, Price = price, PictureBytes = picture, ClothingPropertiesId = ClothingPropertiesId });

                            transaction.Commit();
                            cnn.Close();
                            return null;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return ex.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        public List<ProductsModel> GetAllProducts()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Open();
                try
                {
                    using (IDbTransaction transaction = cnn.BeginTransaction())
                    {
                        string sql = "SELECT * FROM Products";
                        List<ProductsModel> output = cnn.Query<ProductsModel>(sql).ToList();

                        foreach (var product in output)
                        {
                            if (product.CategoryId == 1)
                            {
                                string _sql = "SELECT * FROM ClothingProperties WHERE Id = @ClothingPropertiesId";
                                ClothingProperties clothingProperties = cnn.QueryFirstOrDefault<ClothingProperties>(_sql, new { ClothingPropertiesId = product.ClothingPropertiesId });

                                string _sql2 = "SELECT SizeId FROM ClothingProperties WHERE Id = @ClothingPropertiesId";
                                int sizeId = cnn.QueryFirstOrDefault<int>(_sql2, new { ClothingPropertiesId = product.ClothingPropertiesId });

                                string _sql3 = "SELECT * FROM ClothingSizes WHERE Id = @SizeId";
                                ClothingSizes clothingSizes = cnn.QueryFirstOrDefault<ClothingSizes>(_sql3, new { SizeId = sizeId });

                                product.ClothingPropertiesId = clothingProperties.Id;
                                product.ClothingProperties = clothingProperties;
                                product.ClothingSizes = clothingSizes;

                                product.ShoesPropertiesId = null;
                                product.ShoesSizes = null;
                                product.ShoesProperties = null;
                            }
                            else if (product.CategoryId == 2)
                            {
                                string _sql = "SELECT * FROM ShoesProperties WHERE Id = @ShoesPropertiesId";
                                ShoesProperties ShoesProperties = cnn.QueryFirstOrDefault<ShoesProperties>(_sql, new { ShoesPropertiesId = product.ShoesPropertiesId });

                                string _sql2 = "SELECT SizeId FROM ShoesProperties WHERE Id = @ShoesPropertiesId";
                                int sizeId = cnn.QueryFirstOrDefault<int>(_sql2, new { ShoesPropertiesId = product.ShoesPropertiesId });

                                string _sql3 = "SELECT * FROM ShoesSizes WHERE Id = @SizeId";
                                ShoesSizes ShoesSizes = cnn.QueryFirstOrDefault<ShoesSizes>(_sql3, new { SizeId = sizeId });

                                product.ShoesPropertiesId = ShoesProperties.Id;
                                product.ShoesProperties = ShoesProperties;
                                product.ShoesSizes = ShoesSizes;

                                product.ClothingPropertiesId = null;
                                product.ClothingSizes = null;
                                product.ClothingProperties = null;
                            }
                            else
                            {
                                return null;
                            }
                        }

                        transaction.Commit();
                        return output;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool CheckIfProductExist(int productId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM Products WHERE Id = @Id";
                var num = cnn.QueryFirstOrDefault<int>(sql, new { Id = productId });

                if (num == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // Skal fixes - start med at hente produkterne, så kan du derefter bare lave LinQ?
    }
}