using Baocao2.Models;

namespace Baocao2.Services
{
    public class RolePermissionService
    {
        public IEnumerable<Vw_Rolepermission> GetVListQuery(RolePermission_Search? filter)
        {
            var query = from rp in RolePermissions.List
                        join p in Permissions.permissions on rp.PermissionId equals p.PermissionId
                        where rp.RoleId.ToString() != Role_Fix.ADMIN && rp.IsDelete != IsDelete.Xoa
                        select new Vw_Rolepermission
                        {
                            RolePermissionId = rp.RolePermissionId,
                            RoleId = rp.RoleId,
                            PermissionId = rp.PermissionId,
                            IsDelete = rp.IsDelete,
                            StatusId = rp.StatusId,
                            UseridCreated = rp.UseridCreated,
                            UseridEdited = rp.UseridEdited,
                            DateCreated = rp.DateCreated,
                            DateEdited = rp.DateEdited,
                            Permission_Tilte = p.Title
                        };
            if (filter != null)
            {
                if (filter.RoleId != Guid.Empty)
                {
                    query = query.Where(rp => rp.RoleId == filter.RoleId);
                }
                if (filter.PermissionId != Guid.Empty)
                {
                    query = query.Where(rp => rp.PermissionId == filter.PermissionId);
                }
            }
            return query;
        }

        public ResultModel GetVList(RolePermission_Search? filter)
        {
            var query = GetVListQuery(filter);
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res };
        }

        public IEnumerable<RolePermission> GetListQuery(RolePermission_Search? filter)
        {
            var query = RolePermissions.List.Where(rp => rp.RoleId.ToString() != Role_Fix.ADMIN && rp.IsDelete != IsDelete.Xoa).AsEnumerable();
            if (filter != null)
            {
                if (filter.RoleId != Guid.Empty)
                {
                    query = query.Where(rp => rp.RoleId == filter.RoleId);
                }
                if (filter.PermissionId != Guid.Empty)
                {
                    query = query.Where(rp => rp.PermissionId == filter.PermissionId);
                }
            }

            return query;
        }

        public ResultModel GetList(RolePermission_Search? filter)
        {
            var query = GetListQuery(filter);
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res };
        }

        public ResultModel GetById(Guid rolePermissionId)
        {
            var rolePermission = RolePermissions.List.FirstOrDefault(rp => rp.RolePermissionId == rolePermissionId);
            if (rolePermission == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Phân quyền không tồn tại", Id = null, Object = null };
            }
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = rolePermissionId, Object = rolePermission };
        }

        public ResultModel PhanQuyen(RolePermission_Save rolePermission) // logic phân quyền
        {
            if (rolePermission == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }

            var roleExists = Roles.roles.FirstOrDefault(r => r.RoleId == rolePermission.RoleId);
            if (roleExists == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.RoleId_Error, Message = "Vai trò không tồn tại", Id = null, Object = null };
            }

            var existingRolePermissions = RolePermissions.List.Where(rp => rp.RoleId == rolePermission.RoleId).ToList();
            foreach (var item in rolePermission.PermissionIds)
            {
                if (existingRolePermissions.Any(rp => rp.PermissionId == item))
                {
                    continue;
                }
                var newObject = new RolePermission
                {
                    RolePermissionId = Guid.NewGuid(),
                    RoleId = rolePermission.RoleId,
                    PermissionId = item,
                };
                RolePermissions.List.Add(newObject);
            } 
            RolePermissions.List.RemoveAll(rp => rp.RoleId == rolePermission.RoleId && !rolePermission.PermissionIds.Contains(rp.PermissionId));
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Thao tác thành công", Id = null, Object = null };
        }

        public ResultModel Delete(Guid RoleId)
        {
            if(RoleId == Guid.Empty)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.RoleId_Error, Message = "RoleId không hợp lệ", Id = null, Object = null };
            }
            var rolePermissionsExists = RolePermissions.List.Any(rp => rp.RoleId == RoleId);
            if(!rolePermissionsExists)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Phân quyền không tồn tại", Id = null, Object = null };
            }
            RolePermissions.List.RemoveAll(rp => rp.RoleId == RoleId); // await
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Xóa phân quyền thành công", Id = null, Object = null };
        }
    }
}
