namespace Baocao2.Models
{
    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public int? IsDelete { get; set; }
        public int? StatusId { get; set; }
    }
    public static class RolePermissions
    {
        public static List<RolePermission> List = new List<RolePermission>()
        {
            new RolePermission{RolePermissionId = Guid.NewGuid() , RoleId = Guid.Parse("9ff33dec-0671-40d7-aba9-6c8060b7f0b2") , PermissionId=Guid.Parse("30c474e1-38e9-489a-a5b7-d322f0e09c57")},
            new RolePermission{RolePermissionId = Guid.NewGuid() , RoleId = Guid.Parse("9ff33dec-0671-40d7-aba9-6c8060b7f0b2") , PermissionId=Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139152")},
            new RolePermission{RolePermissionId = Guid.NewGuid() , RoleId = Guid.Parse("7febfdd7-1fa9-4312-80b5-c993810479db") , PermissionId=Guid.Parse("ebb7d7f0-ca3a-4dfe-be48-7182bf139152")},
        };
    }
    public partial class PERMISSION_FIX
    {
        public const string Role_Permisstion = "7ba31e3a-c7d6-4b77-a26d-2d540c768378";
        public const string Role_Permisstion_LIST = "c439a659-a054-41b5-87f8-82fc683d62be";
        public const string Role_Permisstion_ADD = "f4c57c3c-014c-48d9-9eaa-94ec82be1c76";
        public const string Role_Permisstion_EDIT = "7719d677-ad28-4055-8831-314633797933";
        public const string Role_Permisstion_VIEW = "9fe142e5-c1c9-4889-baab-f7d6c0ad50c4";
        public const string Role_Permisstion_IMPORT = "3b8ddb0a-d169-43fe-82d2-3af2b38c9059";
        public const string Role_Permisstion_EXPORT = "9183e0ac-7a57-4bd6-856f-6a7ca25b2a37";
        public const string Role_Permisstion_DELETE = "833847c4-b54a-4f67-b6ce-39f7cd1be35b";
        public const string Role_Permisstion_DELETE_TEMP = "87cf93ec-0d8a-4561-be2b-7edd9e0a272e";
        public const string Role_Permisstion_RESTORE = "d79c5e49-0fdc-4ea5-9af7-b8620928419a";
        public const string Role_Permisstion_HISTORY = "ebb39f02-3d2c-4b46-b14f-ac3395b9f0d8";
    }
}
