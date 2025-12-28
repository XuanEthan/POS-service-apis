using Baocao2.Models;

namespace Baocao2.Services
{
    public class PermissionService
    {
        public ResultModel GetList()
        {
            var result = Permissions.permissions;
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok , Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok) , Id = null , Object = result };
        }
    }
}
