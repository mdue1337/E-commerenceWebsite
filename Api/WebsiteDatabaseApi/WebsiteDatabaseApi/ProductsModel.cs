using System.Text.Json.Serialization;

namespace WebsiteDatabaseApi
{
    public class ProductsModel 
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public byte[] PictureBytes { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ClothingPropertiesId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ShoesPropertiesId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ClothingSizes ClothingSizes { get; internal set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ClothingProperties ClothingProperties { get; internal set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ShoesSizes ShoesSizes { get; internal set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ShoesProperties ShoesProperties { get; internal set; }
    }

    public class ClothingProperties
    {
        public int Id { get; set; }
        public int SizeId { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
    }

    public class ClothingSizes
    {
        // The sizes are the number of stock for that size.
        public int Id { get; set; }
        public int Small { get; set; }
        public int Medium { get; set; }
        public int Large { get; set; }
        public int XL { get; set; }
    }

    public class ShoesProperties
    {
        public int Id { get; set; }
        public int SizeId { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
    }
    public class ShoesSizes
    {
        // The sizes are the number of stock for that size.
        public int Id { get; set; }
        public int Size38 { get; set; }
        public int Size39 { get; set; }
        public int Size40 { get; set; }
        public int Size41 { get; set; }
        public int Size42 { get; set; }
        public int Size43 { get; set; }
        public int Size44 { get; set; }
        public int Size45 { get; set; }
        public int Size46 { get; set; }
    }
}
