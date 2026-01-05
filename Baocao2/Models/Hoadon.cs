using System;

namespace Baocao2.Models
{
    public enum HoadonStatus
    {
        Draft = 0,
        Open = 1,
        Paid = 2,
        Cancelled = 3
    }

    public class Hoadon
    {
        // Primary key (UUID). If you prefer bigint, change type to long.
        public Guid HoadonId { get; set; }

        // Display invoice number (unique, sequential logic must be enforced outside the model)
        public string InvoiceNumber { get; set; } = string.Empty;

        // Identifier for store / POS (using Guid for consistency with other models)
        public Guid StoreId { get; set; }

        // Time the invoice was issued (distinct from CreatedAt)
        public DateTime IssuedAt { get; set; }

        // Employee who created / cashier (using Guid)
        public Guid CreatedBy { get; set; }

        // Minimal status enum: draft | open | paid | cancelled
        public HoadonStatus Status { get; set; } = HoadonStatus.Draft;

        // ISO currency code (3 chars), e.g. "VND", "USD"
        public string Currency { get; set; } = "VND";

        // Monetary fields - use decimal for money
        public decimal Subtotal { get; set; } = 0m;
        public decimal TaxTotal { get; set; } = 0m;
        public decimal DiscountsTotal { get; set; } = 0m;

        // Total = Subtotal - DiscountsTotal + TaxTotal (store rounded value)
        public decimal Total { get; set; } = 0m;

        // Amount still due (Total - payments). Payments aggregation handled elsewhere.
        public decimal AmountDue { get; set; } = 0m;

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Convenience: recalculate totals and update AmountDue (does not persist)
        //public void RecalculateTotals()
        //{
        //    Total = Subtotal - DiscountsTotal + TaxTotal;
        //    // rounding strategy / precision should be applied by caller if needed
        //    AmountDue = Total;
        //}
    }
    public static class Hoadons
    {
       public static List<Hoadon> list = new List<Hoadon>
        {
             new Hoadon
            {
                HoadonId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09199"),
                InvoiceNumber = "INV-1001",
                StoreId = Guid.NewGuid(),
                IssuedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                Status = HoadonStatus.Open,
                Currency = "VND",
                DiscountsTotal = 5000m,
                Total = 105000m,
                AmountDue = 105000m,
                CreatedAt = DateTime.UtcNow,
            }
        };
    }

    public partial class PERMISSION_FIX // quyền tg ưg với mỗi cn đc xây dựng
    {
        public const string Hoadon = "30c474e1-38e9-489a-a5b7-d322f0e09123";
        public const string Hoadon_LIST = "30c474e1-38e9-489a-a5b7-d322f0e09c57";
        public const string Hoadon_ADD = "30c474e1-38e9-489a-a5b7-d322f0e09c58";
        public const string Hoadon_EDIT = "30c474e1-38e9-489a-a5b7-d322f0e09c59";
        public const string Hoadon_DELETE = "30c474e1-38e9-489a-a5b7-d322f0e09c60";
    }
}
