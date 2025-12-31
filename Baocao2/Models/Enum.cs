namespace Baocao2.Models
{
    public static class Role_Fix
    {
        public const string ADMIN = "9ff33dec-0671-40d7-aba9-6c8060b7f0b2";
    }
    public static class IsDelete
    {
        public const int All = 0;
        public const int SuDung = 1;
        public const int Xoa = 2;
    }

    public static class User_StatusId
    {
        public const int All = 0;
        public const int Active = 1;
        public const int UnActive = 2;
        public const int Block = 3;
    }
    public static class Common_StatusId
    {
        public const int All = 0;
        public const int Approval = 1;
        public const int UnApproval = 2;
    }   

}
