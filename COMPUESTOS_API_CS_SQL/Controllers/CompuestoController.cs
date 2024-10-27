using COMPUESTOS_API_CS_SQL.Services;
using Microsoft.AspNetCore.Mvc;

namespace COMPUESTOS_API_CS_SQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompuestoController(CompuestoService compuestoService): Controller
    {

    }
}
