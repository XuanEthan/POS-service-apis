namespace Baocao2.Models
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public Guid ParentId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
    }
    public static class Permissions
    {
        public static List<Permission> permissions = new List<Permission>()
        {
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c57") , ParentId = Guid.Empty , Code="HOADON_LIST" , Title = "Danh sách hóa đơn"},
            new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139152") , ParentId = Guid.Empty , Code = "NGUOIDUNG_LIST" , Title = "Danh sách người dùng"}
        };
    }

    public partial class PERMISSION_FIX
    {
        public const string Permission = "5b32b4ec-c755-415a-98a6-ecae5102d6bd";
        public const string Permission_LIST = "31f88c91-96fa-4ddc-bef7-d7cf71ed2480";
        public const string Permission_ADD = "5a834979-1410-47f1-9bf9-b0429afcb38e";
        public const string Permission_EDIT = "c44601a2-8f2b-4b19-ae23-cc323e2b578e";
        public const string Permission_VIEW = "8c9f1744-1f63-47d9-8c97-82ef3282bf65";
        public const string Permission_IMPORT = "3be24036-48f0-4601-acf5-8700e989cded";
        public const string Permission_EXPORT = "1da6227c-955e-4fc6-ba75-3ef3c1fbc90a";
        public const string Permission_DELETE = "84c7f38d-e9c5-46d0-9a1e-c75462bbdb55";
        public const string Permission_DELETE_TEMP = "f414efac-a569-4df7-8788-7cf7780865ef";
        public const string Permission_RESTORE = "7b2db018-f0c9-4e62-a1fb-0ee99fa13456";
        public const string Permission_HISTORY = "594a7ed6-a738-40bc-ab81-866e890e82ce";
    }
}
