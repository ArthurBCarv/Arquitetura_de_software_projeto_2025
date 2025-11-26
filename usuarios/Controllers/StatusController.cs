using Microsoft.AspNetCore.Mvc;

namespace Usuarios.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Microsserviço de Usuários ativo!";
    }
}
