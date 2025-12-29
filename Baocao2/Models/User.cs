namespace Baocao2.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public Guid RoleId { get; set; }
        public string? RoleCode { get; set; }
    }
    
    public class User_Login
    {
        public string userName { get; set; }
        public string? pwd { get; set; }
        public bool? rememberMe { get; set; }
    }
    
    public static class Users
    {
        public static List<User> UserList = new List<User>
        {
            new User { UserId = Guid.NewGuid(), UserName = "admin", Password = "1", RoleId = Guid.Parse("9ff33dec-0671-40d7-aba9-6c8060b7f0b2") , RoleCode = string.Empty },
            new User { UserId = Guid.NewGuid(), UserName = "user", Password = "1", RoleId = Guid.Parse("7febfdd7-1fa9-4312-80b5-c993810479db") , RoleCode =  string.Empty }
        };
    }

    public partial class PERMISSION_FIX
    {
        public const string User = "ebb7d7f0-ca3a-4dfe-be48-7182bf139123";
        public const string User_LIST = "ebb7d7f0-ca3a-4dfe-be48-7182bf139152";
        public const string User_ADD = "ebb7d7f0-ca3a-4dfe-be48-7182bf139153";
        public const string User_EDIT = "ebb7d7f0-ca3a-4dfe-be48-7182bf139154";
        public const string User_VIEW = "ebb7d7f0-ca3a-4dfe-be48-7182bf139155";
        public const string User_DELETE = "ebb7d7f0-ca3a-4dfe-be48-7182bf139156";
        public const string User_IMPORT = "ebb7d7f0-ca3a-4dfe-be48-7182bf139157";
        public const string User_EXPORT = "ebb7d7f0-ca3a-4dfe-be48-7182bf139158";
        public const string User_DELETE_TEMP = "ebb7d7f0-ca3a-4dfe-be48-7182bf139159";
        public const string User_RESTORE = "ebb7d7f0-ca3a-4dfe-be48-7182bf139160";
        public const string User_HISTORY = "ebb7d7f0-ca3a-4dfe-be48-7182bf139161";
    }
}
