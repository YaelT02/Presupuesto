using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuesto.Filters;
using Presupuesto.Infraestructure;
using Presupuesto.Models;

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

            return RedirectToAction("Index");
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

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET
                              Nombre=@Nombre WHERE id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre,
                            Orden FROM TiposCuentas WHERE Id=@Id AND UsuarioId = @UsuarioId",
                            new { id, usuarioId });
        }

    }


}
