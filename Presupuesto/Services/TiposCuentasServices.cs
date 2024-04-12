using Dapper;
using Microsoft.Data.SqlClient;
using Presupuesto.Infraestructure;
using Presupuesto.Models;

namespace Presupuesto.Services
{
    public class TiposCuentasServices : ITiposCuentasServices
    {
        private readonly string connectionString;

        public TiposCuentasServices(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = connection.QuerySingle<int>($@"INSERT INTO TiposCuenta (Nombre,Usuario,Orden)
                Values(@Nombre, @UsuarioId, 0)
                SELECT SCOPE_IDENTITY();", tipoCuenta);

            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection
                .QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM TiposCuenta
                                        WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection (connectionString); 
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                           FROM TiposCuenta
                                            WHERE UsuarioId = @UsuarioId", new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuenta SET
                              Nombre=@Nombre WHERE id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre,
                            Orden FROM TiposCuenta WHERE Id=@Id",
                            new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TiposCuenta Where Id=@Id", new { id });
        }


        public async Task CrearAsync(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection
                .QuerySingleAsync<int>($@"INSERT INTO TiposCuenta (Nombre,UsuarioId,Orden)
                Values (@Nombre, @UsuarioId, @Orden)
                SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<PagedResults<TipoCuenta>> ObtenerTodos(int pageNumber, int pageSize, int draw)
        {
            using var connection = new SqlConnection(connectionString);

            var totalItems = await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM TiposCuenta");

            var query = @"SELECT * FROM TiposCuenta ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var offset = (pageNumber - 1) * pageSize;

            var items = await connection.QueryAsync<TipoCuenta>(query, new { Offset = offset, PageSize = pageSize });

            var result = new PagedResults<TipoCuenta>(items, totalItems, pageNumber, pageSize, draw)
            {
                Items = items.ToList()
            };

            return result;
        }

    }
}
