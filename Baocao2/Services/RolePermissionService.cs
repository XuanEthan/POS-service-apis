using Baocao2.Models;

namespace Baocao2.Services
{
    public class RolePermissionService
    {
        public IEnumerable<RolePermission> GetListQuery(string? roleId)
        {
            var query = RolePermissions.List.AsEnumerable();
            if (!string.IsNullOrEmpty(roleId)) query = query.Where(rp => Guid.Equals(rp.RoleId, Guid.Parse(roleId)));
            return query;
        }
        public ResultModel GetList(string? roleId)
        {
            var query = GetListQuery(roleId);
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res};
        }
    }
}
