using Baocao2.Models;

namespace Baocao2.Services
{
    public class UserService
    {
        public IEnumerable<User> GetListQuery()
        {
            var query = Users.UserList.AsEnumerable();
            return query;
        }
        public ResultModel GetList()
        {
            var query = GetListQuery();
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res};
        }

    }
}
