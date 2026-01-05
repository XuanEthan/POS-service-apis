using System;

namespace Baocao2.Models
{
    public enum TrackInventoryStrategy
    {
        None = 0,
        Items = 1,    // per-item/location totals
        Batches = 2,  // batch/lot tracked
        Serials = 3   // per-serial-number tracked
    }

    public class Sanpham
    {
        // Primary key (UUID). Change to long if you prefer bigint PK.
        public Guid SanphamId { get; set; }
        public Guid? DanhmucId { get; set; }

        // Internal SKU code (unique per tenant/store or global)
        public string? Sku { get; set; } = string.Empty;

        // Display name
        public string Name { get; set; } = string.Empty;

        // Barcode / EAN / UPC (nullable)
        public string? Barcode { get; set; }

        // Current selling price (store default) - use decimal for money
        public decimal Price { get; set; }

        // ISO currency code (3 chars), e.g. "VND", "USD"
        public string? Currency { get; set; } = "VND";

        // Cost price (optional) for margin calculations
        public decimal CostPrice { get; set; }

        // Unit of measure, e.g. "pcs", "kg"
        public string? UnitOfMeasure { get; set; } = "pcs";

        public decimal? StockOnHand { get; set; }

        public bool? Active { get; set; } = true;

        // Audit fields
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class Vw_Sanpham
    {
        // Primary key (UUID). Change to long if you prefer bigint PK.
        public Guid SanphamId { get; set; }
        public string? Sku { get; set; } = string.Empty;
        public string? DanhmucName { get; set; }
        // Display name
        public string Name { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
    }

    public class Sanpham_Search
    {
        public string? search { get; set; }
    }

    public static class Sanphams
    {
        public static List<Sanpham> list = new List<Sanpham>
        {
             new Sanpham
            {
                SanphamId = Guid.Parse("a1b2c3d4-0001-4f00-9000-000000000001"),
                Sku = "MILK001",
                Name = "Fresh Milk 1L",
                Barcode = "8938500000001",
                Price = 18000m,
                Currency = "VND",
                CostPrice = 12000m,
                UnitOfMeasure = "pcs",
                StockOnHand = 150m,
                Active = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Sanpham
            {
                SanphamId = Guid.Parse("a1b2c3d4-0002-4f00-9000-000000000002"),
                Sku = "RICE5KG",
                Name = "Premium Rice 5kg",
                Barcode = "8938500000002",
                Price = 150000m,
                Currency = "VND",
                CostPrice = 100000m,
                UnitOfMeasure = "bag",
                StockOnHand = 40m,
                Active = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Sanpham
            {
                SanphamId = Guid.Parse("a1b2c3d4-0003-4f00-9000-000000000003"),
                Sku = "PEN-BL",
                Name = "Ballpoint Pen - Blue",
                Barcode = "8938500000003",
                Price = 3000m,
                Currency = "VND",
                CostPrice = 1200m,
                UnitOfMeasure = "pcs",
                StockOnHand = 1000m,
                Active = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = null
            }
        };
    }
    public partial class PERMISSION_FIX // quyền tg ưg với mỗi cn đc xây dựng
    {
        public const string Sanpham = "b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60";
        public const string Sanpham_LIST = "c1d2e3f4-7a8b-4c9d-9e0f-1234567890ab";
        public const string Sanpham_ADD = "d2a3b4c5-8e9f-4a1b-9c0d-abcdef123456";
        public const string Sanpham_EDIT = "e3b4c5d6-9f0a-4b1c-8d2e-654321fedcba";
        public const string Sanpham_DELETE = "f4c5d6e7-0a1b-4c2d-7e3f-0f1e2d3c4b5a";

    }
}
