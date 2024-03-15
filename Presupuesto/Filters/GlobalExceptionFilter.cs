using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace Presupuesto.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) 
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetCurrentClassLogger();

            //Registra la excepcion con NLog
            logger.Error(context.Exception, "Excepcion no manejada");

            //Devuelve una respuesta de error generica al cliente
            var result = new ObjectResult(new { error = "Ocurrio un error interno en el servidor" })
            {
                StatusCode = 500
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
