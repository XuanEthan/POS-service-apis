using Baocao2.Models;
using Baocao2.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Baocao2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanphamsController : ControllerBase
    {
        private readonly SanphamService _sanphamService;
        public SanphamsController(SanphamService sanphamService)
        {
            _sanphamService = sanphamService;
        }

        // GET: api/<SanphamController>
        [HttpGet]
        public ResultModel GetList([FromQuery] Sanpham_Search sanpham_Search)
        {
            return _sanphamService.GetVList(sanpham_Search);
        }

        // GET api/<SanphamController>/5
        [HttpGet("{id}")]
        public ResultModel GetById(Guid id)
        {
            return _sanphamService.GetById(id);
        }

        // POST api/<SanphamController>
        [HttpPost]
        public ResultModel Post([FromBody] Sanpham sanpham)
        {
            return _sanphamService.insert(sanpham);
        }

        // PUT api/<SanphamController>/5
        [HttpPut("{id}")]
        public ResultModel Put(Guid id, [FromBody] Sanpham sanpham)
        {
            return _sanphamService.Update(sanpham);
        }

        // DELETE api/<SanphamController>/5
        [HttpDelete("{id}")]
        public ResultModel Delete(Guid id)
        {
            return _sanphamService.Delete(id);
        }
    }
}
