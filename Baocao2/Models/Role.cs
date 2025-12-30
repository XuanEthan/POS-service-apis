namespace Baocao2.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public Guid ParentId { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
    }
    public static class Roles
    {
        public static List<Role> roles = new List<Role>()
        {
            new Role{RoleId  = Guid.Parse("9ff33dec-0671-40d7-aba9-6c8060b7f0b2") , ParentId = Guid.Empty , Code = "QUANTRI" , Title = "Quản trị"},
            new Role{RoleId = Guid.Parse("7febfdd7-1fa9-4312-80b5-c993810479db") , ParentId = Guid.Empty , Code = "NHANVIENBANHANG" , Title = "Nhân viên bán hàng"}
        };
    }

    public partial class PERMISSION_FIX
    {
        public const string Role = "20cfd7f1-7ee0-468d-91b0-28c937a17812";
        public const string Role_LIST = "66e708f0-00f3-430d-b16d-80d58357c12a";
        public const string Role_ADD = "ffceb425-7223-45ff-9283-8d7c93ad4483";
        public const string Role_EDIT = "15206ba3-9d3f-41e3-b484-90b4b5fdc83d";
        public const string Role_VIEW = "9eb5cd87-3303-4455-bc48-1fd4747bebfe";
        public const string Role_IMPORT = "f6693fa4-9b01-4564-a1af-28cb281cc98d";
        public const string Role_EXPORT = "65154b9c-8dd9-4e8b-9184-7a733ef55a2b";
        public const string Role_DELETE = "28f5e266-cba7-4a75-b145-5cd612641d98";
        public const string Role_DELETE_TEMP = "ff5453bb-8d30-446d-a06e-431a01604313";
        public const string Role_RESTORE = "3f07366f-3048-43f5-b8f1-103664281074";
        public const string Role_HISTORY = "0af7e9e7-8c28-4ef1-b4c4-11ec245e3b5f";
    }
}
