using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuesto.Filters;
using Presupuesto.Infraestructure;
using Presupuesto.Models;
using Presupuesto.Services;

namespace Presupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {

        #region Codigo_Temporal

        private readonly string connectionString;
        private readonly ITiposCuentasServices _tiposCuentaServices;
        private readonly IUsuariosServices _usuariosServices;

        #endregion

        public TiposCuentasController(ITiposCuentasServices tiposCuentaServices, IConfiguration configuration, IUsuariosServices usuariosServices)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            _tiposCuentaServices = tiposCuentaServices;
            _usuariosServices = usuariosServices;
        }

        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TipoCuenta tipoCuenta)
        {
            if(ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            var ExistAccount = await
                _tiposCuentaServices.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if(ExistAccount) 
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                    $"El nombre {tipoCuenta} ya existe");
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = _usuariosServices.ObtenerUsuarioId();
            //await _tiposCuentaServices.CrearAsync(tipoCuenta);
            //return View(tipoCuenta);
            await _tiposCuentaServices.CrearAsync(tipoCuenta);

            return RedirectToAction("ObtenerCuentas");
            //Hardcodeamos el usuario con su Id
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var Exist = await _tiposCuentaServices.Existe(nombre, usuarioId);
            if (Exist)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tiposCuentas = await _tiposCuentaServices.Obtener(usuarioId);
            return View(tiposCuentas);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCuentas()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioid = _usuariosServices.ObtenerUsuarioId();
            var tipoCuenta = await _tiposCuentaServices.ObtenerPorId(id, usuarioid);
            if (tipoCuenta is null)
            {
                RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuentaExiste = await _tiposCuentaServices.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await _tiposCuentaServices.Actualizar(tipoCuenta);
            return RedirectToAction("ObtenerCuentas");
        }

        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuenta = await _tiposCuentaServices.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuenta = await _tiposCuentaServices.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado" , "Home");
            }
            await _tiposCuentaServices.Borrar(id);
            return RedirectToAction("ObtenerCuentas");
        }


        [HttpGet]
        public async Task<ActionResult<PagedResults<TipoCuenta>>> GetTipoCuentas(int start, int length, int draw)
        {
            try
            {
                var pageNumber = start / length + 1;
                var pageSize = length;

                var result = await _tiposCuentaServices.ObtenerTodos(pageNumber, pageSize, draw);

                return Json(new
                {
                    draw,
                    recordsTotal = result.TotalItems,
                    recordsFiltered = result.TotalItems,
                    data = result.Items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
