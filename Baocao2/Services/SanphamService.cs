using Baocao2.Models;

namespace Baocao2.Services
{
    public class SanphamService
    {
        public IEnumerable<Vw_Sanpham> GetVListQuery(Sanpham_Search? search)
        {
            var query = from sp in Sanphams.list
                        select new Vw_Sanpham
                        {
                            SanphamId = sp.SanphamId,
                            DanhmucName = null,
                            Sku = sp.Sku,
                            Name = sp.Name,
                            CostPrice = sp.CostPrice,
                        };
            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.search))
                {
                    query = query.Where(sp => sp.Name.Contains(search.search, StringComparison.OrdinalIgnoreCase) || sp.Sku.Contains(search.search, StringComparison.OrdinalIgnoreCase));
                }
            }
            return query;
        }

        public IEnumerable<Sanpham> GetListQuery(Sanpham_Search? search)
        {
            var query = Sanphams.list.AsEnumerable();
            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.search))
                {
                    query = query.Where(sp => sp.Name.Contains(search.search, StringComparison.OrdinalIgnoreCase) || sp.Sku.Contains(search.search, StringComparison.OrdinalIgnoreCase));
                }
            }
            return query;
        }

        public ResultModel GetVList(Sanpham_Search? search)
        {
            return new ResultModel
            {
                IsSuccess = true,
                Code = ResultModel.ResultCode.Ok,
                Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok),
                Id = null,
                Object = GetVListQuery(search).ToList()
            };
        }
        public ResultModel GetList(Sanpham_Search? search)
        {
            return new ResultModel
            {
                IsSuccess = true,
                Code = ResultModel.ResultCode.Ok,
                Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok),
                Id = null,
                Object = GetListQuery(search).ToList()
            };
        }

        public ResultModel insert(Sanpham sanpham)
        {
            if (sanpham == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }
            if (string.IsNullOrEmpty(sanpham.Name))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Tên sản phẩm không được để trống", Id = null, Object = null };
            }
            if (string.IsNullOrEmpty(sanpham.UnitOfMeasure))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Đơn vị tính không được để trống", Id = null, Object = null };
            }
            var newObject = new Sanpham
            {
                SanphamId = Guid.NewGuid(),
                Sku = sanpham.Sku,
                Name = sanpham.Name,
                Barcode = sanpham.Barcode,
                Price = sanpham.Price,
                Currency = sanpham.Currency,
                CostPrice = sanpham.CostPrice,
                UnitOfMeasure = sanpham.UnitOfMeasure,
                StockOnHand = sanpham.StockOnHand,
                Active = sanpham.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = sanpham.UpdatedAt
            };
            Sanphams.list.Add(newObject);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = newObject.SanphamId, Object = newObject };
        }

        public ResultModel GetById(Guid? id)
        {
            if (id == Guid.Empty || id == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }
            var found = Sanphams.list.FirstOrDefault(sp => sp.SanphamId == id);
            if (found == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Sản phẩm không tồn tại", Id = null, Object = null };
            }
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = id, Object = found };
        }

        public ResultModel Update(Sanpham? sanpham)
        {
            if (sanpham == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }
            if (string.IsNullOrEmpty(sanpham.Name))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Tên sản phẩm không được để trống", Id = null, Object = null };
            }
            if (string.IsNullOrEmpty(sanpham.UnitOfMeasure))
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.NotOK, Message = "Đơn vị tính không được để trống", Id = null, Object = null };
            }
            var found = Sanphams.list.FirstOrDefault(sp => sp.SanphamId == sanpham.SanphamId);
            if (found == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Sản phẩm không tồn tại", Id = null, Object = null };
            }
            found.Sku = sanpham.Sku;
            found.Name = sanpham.Name;
            found.Barcode = sanpham.Barcode;
            found.Price = sanpham.Price;
            found.Currency = sanpham.Currency;
            found.CostPrice = sanpham.CostPrice;
            found.UnitOfMeasure = sanpham.UnitOfMeasure;
            found.StockOnHand = sanpham.StockOnHand;
            found.Active = sanpham.Active;
            found.UpdatedAt = DateTime.UtcNow;
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = sanpham.SanphamId, Object = found };
        }

        public ResultModel Delete(Guid? id)
        {
            if (id == Guid.Empty || id == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.InvalidateData, Message = "Thông tin không hợp lệ", Id = null, Object = null };
            }
            var found = Sanphams.list.FirstOrDefault(sp => sp.SanphamId == id);
            if (found == null)
            {
                return new ResultModel { IsSuccess = false, Code = ResultModel.ResultCode.Does_Not_Exists, Message = "Sản phẩm không tồn tại", Id = null, Object = null };
            }
            Sanphams.list.Remove(found);
            return new ResultModel { IsSuccess = true, Code = ResultModel.ResultCode.Ok, Message = ResultModel.BuildMessage(ResultModel.ResultCode.Ok), Id = id, Object = null };
        }
    }
}
