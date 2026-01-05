namespace Baocao2.Models
{
    public class Permission
    {
        public Guid PermissionId { get; set; }
        public Guid ParentId { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int? StatusId { get; set; }
        public int? IsDelete { get; set; }
        public Guid UseridCreated { get; set; }
        public Guid UseridEdited { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
    }

    public static class Permissions
    {
        public static List<Permission> permissions = new List<Permission>()
        {
            // Hoadon Permissions
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , ParentId = Guid.Empty , Code="HOADON" , Title = "Quản lý hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c57") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_LIST" , Title = "Xem danh sách hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c58") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_ADD" , Title = "Thêm hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c59") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_EDIT" , Title = "Sửa hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c60") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_VIEW" , Title = "Xem hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c61") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_DELETE" , Title = "Xóa hóa đơn"},
            new Permission{PermissionId = Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c62") , ParentId =  Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c11") , Code="HOADON_SEARCH" , Title = "Tìm kiếm hóa đơn"},

            new Permission{PermissionId = Guid.Parse("ad5126b2-a617-400b-868f-4ffda4e255ec") , ParentId = Guid.Empty , Code = "BANHANG"  , Title = "Quản lý bán hàng"},
            new Permission{PermissionId = Guid.Parse("ad5126b2-a617-400b-868f-4ffda4e25890") , ParentId = Guid.Empty , Code = "BAOCAO"  , Title = "Báo cáo"},
            
            //// User Permissions
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , ParentId = Guid.Empty , Code = "USER" , Title = "Quản lý người dùng"},
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139152") , ParentId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_LIST" , Title = "Xem danh sách người dùng"},
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139153") , ParentId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_ADD" , Title = "Thêm người dùng"},
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139154") ,  ParentId =Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_EDIT" , Title = "Sửa người dùng"},
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139155") , ParentId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_VIEW" , Title = "Xem người dùng"},
            //new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139156") , ParentId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_DELETE" , Title = "Xóa người dùng"},
            //            new Permission{PermissionId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139133") , ParentId = Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139111") , Code = "USER_SEARCH" , Title = "Tìm kiếm người dùng"},

            
            //// Role Permissions
            //new Permission{PermissionId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , ParentId = Guid.Empty , Code = "ROLE" , Title = "Quản lý vai trò"},
            //new Permission{PermissionId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c12a") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_LIST" , Title = "Xem danh sách vai trò"},
            //new Permission{PermissionId = Guid.Parse("ffceb425-7223-45ff-9283-8d7c93ad4483") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_ADD" , Title = "Thêm vai trò"},
            //new Permission{PermissionId = Guid.Parse("15206ba3-9d3f-41e3-b484-90b4b5fdc83d") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_EDIT" , Title = "Sửa vai trò"},
            //new Permission{PermissionId = Guid.Parse("9eb5cd87-3303-4455-bc48-1fd4747bebfe") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_VIEW" , Title = "Xem vai trò"},
            //new Permission{PermissionId = Guid.Parse("28f5e266-cba7-4a75-b145-5cd612641d98") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_DELETE" , Title = "Xóa vai trò"},
            //new Permission{PermissionId = Guid.Parse("28f5e266-cba7-4a75-b145-5cd612641d33") , ParentId = Guid.Parse("66e708f0-00f3-430d-b16d-80d58357c133") , Code = "ROLE_SEARCH" , Title = "Tìm kiếm vai trò"},

            
            //// Permission Permissions
            //new Permission{PermissionId = Guid.Parse("31f88c91-96fa-4ddc-bef7-d7cf71ed2480") , ParentId = Guid.Empty , Code = "PERMISSION_LIST" , Title = "Xem danh sách quyền"},
            
            //// RolePermission Permissions
            //new Permission{PermissionId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , ParentId = Guid.Empty , Code = "ROLEPERMISSION" , Title = "Quản lý phân quyền"},
            //new Permission{PermissionId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d62be") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_LIST" , Title = "Danh sách phân quyền"},
            //new Permission{PermissionId = Guid.Parse("f4c57c3c-014c-48d9-9eaa-94ec82be1c76") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_ADD" , Title = "Thêm phân quyền"},
            //new Permission{PermissionId = Guid.Parse("7719d677-ad28-4055-8831-314633797933") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_EDIT" , Title = "Sửa phân quyền"},
            //new Permission{PermissionId = Guid.Parse("9fe142e5-c1c9-4889-baab-f7d6c0ad50c4") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_VIEW" , Title = "Xem phân quyền"},
            //new Permission{PermissionId = Guid.Parse("833847c4-b54a-4f67-b6ce-39f7cd1be35b") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_DELETE" , Title = "Xóa phân quyền"},
            //new Permission{PermissionId = Guid.Parse("833847c4-b54a-4f67-b6ce-39f7cd1be399") , ParentId = Guid.Parse("c439a659-a054-41b5-87f8-82fc683d6211") , Code = "ROLEPERMISSION_SEARCH" , Title = "Tìm kiếm phân quyền"},

             new Permission{PermissionId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), ParentId = Guid.Empty, Code = "SANPHAM", Title = "Quản lý sản phẩm"},
            new Permission{PermissionId = Guid.Parse("c1d2e3f4-7a8b-4c9d-9e0f-1234567890ab"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_LIST", Title = "Xem danh sách sản phẩm"},
            new Permission{PermissionId = Guid.Parse("d2a3b4c5-8e9f-4a1b-9c0d-abcdef123456"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_ADD", Title = "Thêm sản phẩm"},
            new Permission{PermissionId = Guid.Parse("e3b4c5d6-9f0a-4b1c-8d2e-654321fedcba"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_EDIT", Title = "Sửa sản phẩm"},
            new Permission{PermissionId = Guid.Parse("f4c5d6e7-0a1b-4c2d-7e3f-0f1e2d3c4b5a"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_DELETE", Title = "Xóa sản phẩm"},
            new Permission{PermissionId = Guid.Parse("a5d6e7f8-1b2c-4d3e-6f4a-1b2c3d4e5f6a"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_VIEW", Title = "Xem sản phẩm"},
            new Permission{PermissionId = Guid.Parse("b6e7f8a9-2c3d-4e5f-5a6b-2c3d4e5f6a7b"), ParentId = Guid.Parse("b3e9f2a1-6c7d-4a2b-9f0e-1d2c3b4a5f60"), Code = "SANPHAM_SEARCH", Title = "Tìm kiếm sản phẩm"},
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