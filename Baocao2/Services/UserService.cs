using Baocao2.Models;

namespace Baocao2.Services
{
    public class UserService
    {
        public IEnumerable<Vw_User> GetVwListQuery()
        {
            var query =
                from u in Users.UserList
                join r in Roles.roles
                    on u.RoleId equals r.RoleId
                    into ur
                from r in ur.DefaultIfEmpty()   // LEFT JOIN
                where
                    u.RoleId == Guid.Empty
                   || u.RoleId.ToString() != "9ff33dec-0671-40d7-aba9-6c8060b7f0b2"
                select new Vw_User
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Password = u.Password,
                    RoleId = u.RoleId,
                    RoleTiTle = r != null ? r.Title : ""
                };
            return query;
        }

        public ResultModel GetVwList()
        {
            var query = GetVwListQuery();
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res };
        }

        public IEnumerable<User> GetListQuery()
        {
            var query = Users.UserList.Where(u => u.RoleId.ToString() != "9ff33dec-0671-40d7-aba9-6c8060b7f0b2");
            return query;
        }

        public ResultModel GetList()
        {
            var query = GetListQuery();
            var res = query.ToList();
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = null, Object = res };
        }

        public ResultModel GetById(Guid userId)
        {
            var user = Users.UserList.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Người dùng không tồn tại", Id = null, Object = null };
            }
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = userId, Object = user };
        }

        public ResultModel Insert(User user)
        {
            if (user == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }

            if (string.IsNullOrEmpty(user.UserName))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Tên đăng nhập không được để trống", Id = null, Object = null };
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Mật khẩu không được để trống", Id = null, Object = null };
            }

            var existsUsername = Users.UserList.FirstOrDefault(u => u.UserName == user.UserName);
            if (existsUsername != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.UserName_Exists, Message = "Tên đăng nhập đã tồn tại", Id = null, Object = null };
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                UserName = user.UserName,
                Password = user.Password,
                RoleId = Guid.Equals(user.RoleId, Guid.Empty) ? Guid.Empty : user.RoleId,
            };

            Users.UserList.Add(newUser);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Thêm người dùng thành công", Id = newUser.UserId, Object = newUser };
        }

        public ResultModel Update(Guid userId, User user)
        {
            if (user == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }
            var existingUser = Users.UserList.FirstOrDefault(u => u.UserId == userId);
            if (existingUser == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Người dùng không tồn tại", Id = null, Object = null };
            }
            if (string.IsNullOrEmpty(user.UserName))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Tên đăng nhập không được để trống", Id = null, Object = null };
            }

            var duplicateUsername = Users.UserList.FirstOrDefault(u => u.UserName == user.UserName && u.UserId != userId);
            if (duplicateUsername != null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.UserName_Exists, Message = "Tên đăng nhập đã tồn tại", Id = null, Object = null };
            }

            existingUser.UserName = user.UserName;
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = user.Password;
            }

            existingUser.RoleId = (user.RoleId.ToString() == Guid.Empty.ToString()) ? Guid.Empty : user.RoleId;

            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Cập nhật người dùng thành công", Id = existingUser.UserId, Object = existingUser };
        }

        public ResultModel Delete(Guid userId)
        {
            var existingUser = Users.UserList.FirstOrDefault(u => u.UserId == userId);
            if (existingUser == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Người dùng không tồn tại", Id = null, Object = null };
            }

            Users.UserList.Remove(existingUser);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = "Xóa người dùng thành công", Id = userId, Object = null };
        }
    }
}
