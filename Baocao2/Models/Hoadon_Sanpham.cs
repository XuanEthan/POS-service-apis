namespace Baocao2.Models
{
    public class Hoadon_Sanpham
    {
        public Guid Hoadon_SanphamId { get; set; }
        public Guid HoadonId { get; set; }
        public Guid SanphamId { get; set; }
        public string SanphamName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }

    public static class Hoadon_Sanphams
    {
        public static List<Hoadon_Sanpham> list = new List<Hoadon_Sanpham>
        {
            new Hoadon_Sanpham
            {
                Hoadon_SanphamId = Guid.NewGuid(),
                HoadonId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09199"),
                SanphamId = Guid.Parse("a1b2c3d4-0001-4f00-9000-000000000001"),
                SanphamName = "Sản phẩm 1",
                Quantity = 2,
                TotalPrice = 100000m
            },
            new Hoadon_Sanpham
            {
                Hoadon_SanphamId = Guid.NewGuid(),
                HoadonId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09199"),
                SanphamId = Guid.Parse("a1b2c3d4-0002-4f00-9000-000000000002"),
                SanphamName = "Sản phẩm 2",
                Quantity = 1,
                TotalPrice = 75000m
            },
            new Hoadon_Sanpham
            {
                Hoadon_SanphamId = Guid.NewGuid(),
                HoadonId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09199"),
                SanphamId = Guid.Parse("a1b2c3d4-0003-4f00-9000-000000000003"),
                SanphamName = "Sản phẩm 3",
                Quantity = 5,
                TotalPrice = 60000m
            }
        };
    }
}
