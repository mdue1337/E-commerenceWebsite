using Dapper;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml;

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

        public void CreateUser(string firstName, string lastName, string street, int streetNum, string city, int postNum, string email, string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the hash to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashedBytes = sha256Hash.ComputeHash(bytes);

                // Convert the hashed bytes to a string
                string finalHash = BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLower();

                password = finalHash;
            }

            UserModel user = new UserModel()
            {
                FirstName = firstName,
                LastName = lastName,
                Street = street,
                StreetNumber = streetNum,
                City = city,
                PostNumber = postNum,
                Email = email,
                Password = password
            };

            try
            {
                using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
                {
                    string sql = "INSERT INTO Users (FirstName, LastName, Street, StreetNumber, City, PostNumber, Email, Password) VALUES (@FirstName, @LastName, @Street, @StreetNumber, @City, @PostNumber, @Email, @Password)";
                    cnn.Execute(sql, user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<UserModel> LoadUsers()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM USERS";
                var query = cnn.Query<UserModel>(sql);
                return query.ToList();
            }
        }

        public void DeleteUser(int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "DELETE FROM Users WHERE Users.Id = @Id";
                cnn.Execute(sql, new { Id = userId });
            }
        }

        public bool CheckIfUserExist(int userId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Id = @Id";
                var num = cnn.QueryFirstOrDefault<int>(sql, new { Id = userId });

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

        // Review querys.

        public void CreateReview(int productId, int userId, int rating, string text, string timestamp)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "INSERT INTO Review (ProductId, UserId, Rating, Text, Timestamp) VALUES (@ProductId, @UserId, @Rating, @Text, @Timestamp)";
                cnn.Execute(sql, new { ProductId = productId, UserId = userId, Rating = rating, Text = text, Timestamp = timestamp });
            }
        }

        public List<ReviewModel> ReviewsForProduct(int productId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM Review WHERE Review.ProductId = @Id";
                var output = cnn.Query<ReviewModel>(sql, new { Id = productId });
                return output.ToList();
            }
        }

        // Product querys.

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

        public string CreateListingShoes(int[] sizes, string color, string brand, string name, double price, byte[] picture)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    int categoryId = 2;

                    cnn.Open();

                    using (IDbTransaction transaction = cnn.BeginTransaction())
                    {
                        try
                        {
                            string sql1 = "INSERT INTO ShoesSizes (Size38, Size39, Size40, Size41, Size42, Size43, Size44, Size45, Size46) VALUES (@Size38, @Size39, @Size40, @Size41, @Size42, @Size43, @Size44, @Size45, @Size46); SELECT last_insert_rowid();";
                            int ShoesSizeId = cnn.QuerySingleOrDefault<int>(sql1, new { Size38 = sizes[0], Size39 = sizes[1], Size40 = sizes[2], Size41 = sizes[3], Size42 = sizes[4], Size43 = sizes[5], Size44 = sizes[6], Size45 = sizes[7], Size46 = sizes[8] });

                            if (ShoesSizeId == 0)
                            {
                                throw new Exception("Failed to retrieve id");
                            }

                            string sql2 = "INSERT INTO ShoesProperties (SizeId, Color, Brand) VALUES (@SizeId, @Color, @Brand); SELECT last_insert_rowid();";
                            int ShoesPropertiesId = cnn.QuerySingleOrDefault<int>(sql2, new { SizeId = ShoesSizeId, Color = color, Brand = brand });

                            if (ShoesPropertiesId == 0)
                            {
                                throw new Exception("Failed to retrieve id");
                            }

                            string sql3 = "INSERT INTO Products (Name, CategoryId, Price, PictureBytes, ShoesPropertiesId) VALUES (@Name, @CategoryId, @Price, @PictureBytes, @ShoesPropertiesId)";
                            cnn.Execute(sql3, new { Name = name, CategoryId = categoryId, Price = price, PictureBytes = picture, ShoesPropertiesId = ShoesPropertiesId });

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
                        IDictionary<string, object> result = new Dictionary<string, object>();

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
    }
}