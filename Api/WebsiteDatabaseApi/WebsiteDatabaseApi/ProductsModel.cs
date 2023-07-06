namespace WebsiteDatabaseApi
{
    public class ProductsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public byte[] PictureBytes { get; set; }
    }

    public class ClothingProductModel : ProductsModel
    {
        public string ClothingColor { get; set; }
        public string ClothingSize { get; set; }
        public string ClothingBrand { get; set; }
    }

    public class ShoesProductModel : ProductsModel
    {
        public string ShoesColor { get; set; }
        public string ShoesSize { get; set; }
        public string ShoesBrand { get; set; }
    }
}
