using Baocao2.Models;

namespace Baocao2.Services
{
    public class RoleService
    {

        public IEnumerable<Role> GetListQuery()
        {
            var query = Roles.roles.Where(r=>r.RoleId.ToString() != Role_Fix.ADMIN && r.IsDelete != IsDelete.Xoa).AsEnumerable();
            return query;
        }
        public ResultModel GetList()
        {
            var query = GetListQuery();
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res };
        }

        public ResultModel GetById(Guid roleId)
        {
            var role = Roles.roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Vai trò không tồn tại", Id = null, Object = null };
            }
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = roleId, Object = role };
        }

        public ResultModel Insert(Role role)
        {
            if (role == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }

            if (string.IsNullOrEmpty(role.Code))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Mã vai trò không được để trống", Id = null, Object = null };
            }

            var exists = Roles.roles.FirstOrDefault(r => r.Code == role.Code);
            if (exists != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Exists, Message = "Mã vai trò đã tồn tại", Id = null, Object = null };
            }

            if (role.ParentId != Guid.Empty)
            {
                var parentExists = Roles.roles.FirstOrDefault(r => r.RoleId == role.ParentId);
                if (parentExists == null)
                {
                    return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Vai trò cha không tồn tại", Id = null, Object = null };
                }
            }

            var newObject = new Role
            {
                RoleId = Guid.NewGuid(),
                ParentId = role.ParentId,
                Code = role.Code,
                Title = role.Title,
                StatusId = role.StatusId,
                IsDelete = role.IsDelete,
                UseridCreated = role.UseridCreated,
                DateCreated = role.DateCreated,
            };

            Roles.roles.Add(newObject);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Thêm vai trò thành công", Id = newObject.RoleId, Object = newObject };
        }

        public ResultModel Update(Guid roleId, Role role)
        {
            if (role == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }

            var existingRole = Roles.roles.FirstOrDefault(r => r.RoleId == roleId);
            if (existingRole == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Vai trò không tồn tại", Id = null, Object = null };
            }

            if (string.IsNullOrEmpty(role.Code))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Mã vai trò không được để trống", Id = null, Object = null };
            }

            var duplicateCode = Roles.roles.FirstOrDefault(r => r.Code == role.Code && r.RoleId != roleId);
            if (duplicateCode != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Exists, Message = "Mã vai trò đã tồn tại", Id = null, Object = null };
            }

            if (role.ParentId != Guid.Empty)
            {
                var parentExists = Roles.roles.FirstOrDefault(r => r.RoleId == role.ParentId);
                if (parentExists == null)
                {
                    return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Vai trò cha không tồn tại", Id = null, Object = null };
                }

                if (IsChildRole(roleId, role.ParentId))
                {
                    return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Không thể đặt vai trò cha là chính nó hoặc vai trò con của nó", Id = null, Object = null };
                }
            }

            existingRole.ParentId = role.ParentId;
            existingRole.Code = role.Code;
            existingRole.Title = role.Title;
            existingRole.StatusId = role.StatusId;
            existingRole.IsDelete = role.IsDelete;
            existingRole.UseridEdited = role.UseridEdited;

            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Cập nhật vai trò thành công", Id = existingRole.RoleId, Object = existingRole };
        }
        // delete temp
        public ResultModel Delete(Guid roleId)
        {
            var existingRole = Roles.roles.FirstOrDefault(r => r.RoleId == roleId);
            if (existingRole == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Vai trò không tồn tại", Id = null, Object = null };
            }

            var hasChildren = Roles.roles.Any(r => r.ParentId == roleId);
            if (hasChildren)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Không thể xóa vai trò có vai trò con", Id = null, Object = null };
            }

            Roles.roles.Remove(existingRole);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Xóa vai trò thành công", Id = roleId, Object = null };
        }

        private bool IsChildRole(Guid roleId, Guid potentialParentId)
        {
            if (roleId == potentialParentId)
            {
                return true;
            }

            var children = Roles.roles.Where(r => r.ParentId == roleId);
            foreach (var child in children)
            {
                if (IsChildRole(child.RoleId, potentialParentId))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
