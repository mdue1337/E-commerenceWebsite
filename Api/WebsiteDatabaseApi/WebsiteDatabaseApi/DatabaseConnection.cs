using Dapper;
using System;
using System.Buffers.Text;
using System.ComponentModel;
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

        public string CreateListing(string Name, int CategoryId, double Price, int Stock, byte[] ImageBytes, string categoryColor, string categorySize, string categoryBrand)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    if (CategoryId == 1)
                    {
                        string ClothingColor = categoryColor;
                        string ClothingSize = categorySize;
                        string ClothingBrand = categoryBrand;
                        string sql = "INSERT INTO Products (Name, CategoryId, Price, Stock, PictureBytes, ClothingColor, ClothingSize, ClothingBrand) VALUES (@Name, @CategoryId, @Price, @Stock, @PictureBytes, @ClothingColor, @ClothingSize, @ClothingBrand)";
                        cnn.Execute(sql, new { Name = Name, CategoryId = CategoryId, Price = Price, Stock = Stock, PictureBytes = ImageBytes, ClothingColor = ClothingColor, ClothingSize = ClothingSize, ClothingBrand = ClothingBrand });
                    }
                    else if (CategoryId == 2)
                    {
                        string ShoesColor = categoryColor;
                        int ShoesSize = int.Parse(categorySize);
                        string ShoesBrand = categoryBrand;
                        string sql = "INSERT INTO Products (Name, CategoryId, Price, Stock, PictureBytes, ShoesColor, ShoesSize, ShoesBrand) VALUES (@Name, @CategoryId, @Price, @Stock, @PictureBytes, @ShoesColor, @ShoesSize, @ShoesBrand)";
                        cnn.Execute(sql, new { Name = Name, CategoryId = CategoryId, Price = Price, Stock = Stock, PictureBytes = ImageBytes, ShoesColor = ShoesColor, ShoesSize = ShoesSize, ShoesBrand = ShoesBrand });
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        public List<ProductsModel> GetProductByCategoryAndAllSizes(int CategoryId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var products = GetProductsByCategory(CategoryId);
                var productListType = products.GetType().GetGenericArguments()[0];

                var output = new List<ProductsModel>();

                if (productListType == typeof(ClothingProductModel))
                {
                    output = cnn.Query<ClothingProductModel>("").ToList<ProductsModel>();
                }
                else if (productListType == typeof(ShoesProductModel))
                {
                    output = cnn.Query<ShoesProductModel>("").ToList<ProductsModel>();
                }
                
                return output;
            }
        }

        public List<ProductsModel> GetProductsByCategory(int categoryId)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string sql = "SELECT * FROM Products WHERE CategoryId = @CategoryId";
                var products = cnn.Query<ProductsModel>(sql, new { CategoryId = categoryId }).ToList();

                var mappedProducts = new List<ProductsModel>();

                foreach (var product in products)
                {
                    ProductsModel mappedProduct = categoryId switch
                    {
                        // Clothing category
                        1 => MapToClothingProduct(cnn.Query<ClothingProductModel>(sql, new { CategoryId = categoryId, ProductId = product.Id }).FirstOrDefault()),
                        // Shoes category
                        2 => MapToShoesProduct(cnn.Query<ShoesProductModel>(sql, new { CategoryId = categoryId, ProductId = product.Id }).FirstOrDefault()),
                        _ => product // Default to the base Products model
                    };
                    mappedProducts.Add(mappedProduct);
                }

                return mappedProducts;
            }
        }

        private ClothingProductModel MapToClothingProduct(ProductsModel product)
        {
            return new ClothingProductModel
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Stock = product.Stock,
                PictureBytes = product.PictureBytes,
                ClothingColor = ((ClothingProductModel)product).ClothingColor,
                ClothingSize = ((ClothingProductModel)product).ClothingSize,
                ClothingBrand = ((ClothingProductModel)product).ClothingBrand
            };
        }

        private ShoesProductModel MapToShoesProduct(ProductsModel product)
        {
            return new ShoesProductModel
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                Price = product.Price,
                Stock = product.Stock,
                PictureBytes = product.PictureBytes,
                ShoesColor = ((ShoesProductModel)product).ShoesColor,
                ShoesSize = ((ShoesProductModel)product).ShoesSize,
                ShoesBrand = ((ShoesProductModel)product).ShoesBrand
            };
        }
    }
}
