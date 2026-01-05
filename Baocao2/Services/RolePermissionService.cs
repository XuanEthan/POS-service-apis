using Baocao2.Models;

namespace Baocao2.Services
{
    public class RolePermissionService
    {
        public IEnumerable<RolePermission> GetListQuery(RolePermission_Search? filter)
        {
            var query = RolePermissions.List.Where(rp=>rp.RoleId.ToString() != Role_Fix.ADMIN && rp.IsDelete != IsDelete.Xoa).AsEnumerable();
           if(filter != null)
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

        public ResultModel Insert(RolePermission rolePermission)
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

            var permissionExists = Permissions.permissions.FirstOrDefault(p => p.PermissionId == rolePermission.PermissionId);
            if (permissionExists == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Quyền không tồn tại", Id = null, Object = null };
            }

            var exists = RolePermissions.List.FirstOrDefault(rp => rp.RoleId == rolePermission.RoleId && rp.PermissionId == rolePermission.PermissionId);
            if (exists != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Exists, Message = "Phân quyền đã tồn tại", Id = null, Object = null };
            }

            var newRolePermission = new RolePermission
            {
                RolePermissionId = Guid.NewGuid(),
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId,
                IsDelete = rolePermission.IsDelete,
                UseridCreated = rolePermission.UseridCreated,
                DateCreated = rolePermission.DateCreated,
            };

            RolePermissions.List.Add(newRolePermission);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Thêm phân quyền thành công", Id = newRolePermission.RolePermissionId, Object = newRolePermission };
        }

        public ResultModel Update(Guid rolePermissionId, RolePermission rolePermission)
        {
            if (rolePermission == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }

            var existingRolePermission = RolePermissions.List.FirstOrDefault(rp => rp.RolePermissionId == rolePermissionId);
            if (existingRolePermission == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Phân quyền không tồn tại", Id = null, Object = null };
            }

            var roleExists = Roles.roles.FirstOrDefault(r => r.RoleId == rolePermission.RoleId);
            if (roleExists == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.RoleId_Error, Message = "Vai trò không tồn tại", Id = null, Object = null };
            }

            var permissionExists = Permissions.permissions.FirstOrDefault(p => p.PermissionId == rolePermission.PermissionId);
            if (permissionExists == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Quyền không tồn tại", Id = null, Object = null };
            }

            var duplicate = RolePermissions.List.FirstOrDefault(rp => rp.RoleId == rolePermission.RoleId && rp.PermissionId == rolePermission.PermissionId && rp.RolePermissionId != rolePermissionId);
            if (duplicate != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Exists, Message = "Phân quyền đã tồn tại", Id = null, Object = null };
            }

            existingRolePermission.RoleId = rolePermission.RoleId;
            existingRolePermission.PermissionId = rolePermission.PermissionId;
            existingRolePermission.IsDelete = rolePermission.IsDelete;
            existingRolePermission.UseridEdited = rolePermission.UseridEdited;
            existingRolePermission.DateEdited = rolePermission.DateEdited;

            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Cập nhật phân quyền thành công", Id = existingRolePermission.RolePermissionId, Object = existingRolePermission };
        }

        public ResultModel Delete(Guid rolePermissionId)
        {
            var existingRolePermission = RolePermissions.List.FirstOrDefault(rp => rp.RolePermissionId == rolePermissionId);
            if (existingRolePermission == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Phân quyền không tồn tại", Id = null, Object = null };
            }

            RolePermissions.List.Remove(existingRolePermission);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Xóa phân quyền thành công", Id = rolePermissionId, Object = null };
        }

        //public ResultModel GetPermissionsByRole(Guid roleId)
        //{
        //    var roleExists = Roles.roles.FirstOrDefault(r => r.RoleId == roleId);
        //    if (roleExists == null)
        //    {
        //        return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.RoleId_Error, Message = "Vai trò không tồn tại", Id = null, Object = null };
        //    }

        //    var rolePermissions = RolePermissions.List.Where(rp => rp.RoleId == roleId).ToList();
        //    var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();
        //    var permissions = Permissions.permissions.Where(p => permissionIds.Contains(p.PermissionId)).ToList();

        //    return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = roleId, Object = permissions };
        //}

        //public ResultModel AssignPermissionsToRole(Guid roleId, List<Guid> permissionIds)
        //{
        //    //var roleExists = Roles.roles.FirstOrDefault(r => r.RoleId == roleId);
        //    var roleExists = _roleService.GetById(roleId);
        //    if (roleExists == null)
        //    {
        //        return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.RoleId_Error, Message = "Vai trò không tồn tại", Id = null, Object = null };
        //    }

        //    var existingRolePermissions = RolePermissions.List.Where(rp => rp.RoleId == roleId).ToList();
        //    foreach (var rp in existingRolePermissions)
        //    {
        //        RolePermissions.List.Remove(rp);
        //    }

        //    foreach (var permissionId in permissionIds)
        //    {
        //        var permissionExists = _permissionService.GetById(permissionId);
        //        if (permissionExists != null)
        //        {
        //            var newRolePermission = new RolePermission
        //            {
        //                RolePermissionId = Guid.NewGuid(),
        //                RoleId = roleId,
        //                PermissionId = permissionId,
        //                IsDelete = 0,
        //                StatusId = 1
        //            };
        //            RolePermissions.List.Add(newRolePermission);
        //        }
        //    }

        //    return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Gán quyền cho vai trò thành công", Id = roleId, Object = null };
        //}
    }
}
