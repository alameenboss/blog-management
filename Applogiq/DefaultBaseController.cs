using Applogiq.LoggerService;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Applogiq
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultBaseController : ControllerBase
    {
        private IMapper? _mapperInstance;
        private ILogService? _logService;

        protected IMapper _mapper => _mapperInstance ??= HttpContext.RequestServices.GetService<IMapper>();
        protected ILogService _logger => _logService ??= HttpContext.RequestServices.GetService<ILogService>();
    }
}
