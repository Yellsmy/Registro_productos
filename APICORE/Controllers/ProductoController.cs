using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICORE.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Cors;

namespace APICORE.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        /*Declaracion de variable de tipo privado solamente de lectura que será una cadena de texto*/
        private readonly string cadenaSQL;

        /*Creación del constructor */
        public ProductoController (IConfiguration config)
        {
            /*Se almacena la cadena de conexión a la base de datos*/
            cadenaSQL = config.GetConnectionString("CadenaSQL");
            
        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            List<Productos> lista = new List<Productos>(); //Opcional cambiar a sqlDataApter y DataSet
            /*SQLDataAdapter genera las líneas y DataSet ingresa los datos a cada fila*/
            /*try solo para la conexión*/
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();  // try Solo para este

                /*try opcional desde aquí*/
                var cmd = new SqlCommand("sp_lista_productos", conexion);
                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;
                using(var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Productos()
                        {
                            IdProducto = Convert.ToInt32(rd["IdProducto"]),
                            CodigoBarra = rd["CodigoBarra"].ToString(),
                            Nombre = rd["Nombre"].ToString(),
                            Marca = rd["Marca"].ToString(),
                            Categoria = rd["Categoria"].ToString(),
                            Precio = Convert.ToDecimal(rd["Precio"])
                        });
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });


            }
            catch(Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = lista });
            }
        }

        [HttpGet]
        [Route("Obtener/{idProducto:int}")]
        public IActionResult Obtener(int idProducto)
        {
            List<Productos> lista = new List<Productos>();
            Productos producto = new Productos();
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                var cmd = new SqlCommand("sp_lista_productos", conexion);
                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new Productos()
                        {
                            IdProducto = Convert.ToInt32(rd["IdProducto"]),
                            CodigoBarra = rd["CodigoBarra"].ToString(),
                            Nombre = rd["Nombre"].ToString(),
                            Marca = rd["Marca"].ToString(),
                            Categoria = rd["Categoria"].ToString(),
                            Precio = Convert.ToDecimal(rd["Precio"])
                        });
                    }


                }


                /*De la lista extrae el producto si el id coincide con el id que se pasa como parametro, sino devuelve
                 un valor nulo (Con FirstOrDefault()*/
                producto = lista.Where(item => item.IdProducto == idProducto).FirstOrDefault();


                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = producto });


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = producto });
            }
        }

        [HttpPost]
        [Route("Guardar")]
        public IActionResult Guardar([FromBody] Productos objeto)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                var cmd = new SqlCommand("sp_guardar_producto", conexion);

                /*Definimos los paramtros de entrada*/
                cmd.Parameters.AddWithValue("codigoBarra", objeto.CodigoBarra);
                cmd.Parameters.AddWithValue("nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("marca", objeto.Marca);
                cmd.Parameters.AddWithValue("categoria", objeto.Categoria);
                cmd.Parameters.AddWithValue("precio", objeto.Precio);

                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;

                /*ExecuteNonQuery sirve para detener la ejecución del procedimiento alamcenado*/
                cmd.ExecuteNonQuery();
            
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok"});


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message});
            }
        }

        [HttpPut]
        [Route("Editar")]
        public IActionResult Editar([FromBody] Productos objeto)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                var cmd = new SqlCommand("sp_editar_producto", conexion);

                /*Definimos los parametros de entrada
                 Se hace la vaidación porque si el usuario no ingresa un dato, debe continuar el dato anterior
                para enteros lo que devuelve es un cero, en el caso de string devuelve null*/
                cmd.Parameters.AddWithValue("idProducto", objeto.IdProducto == 0 ? DBNull.Value : objeto.IdProducto);
                cmd.Parameters.AddWithValue("codigoBarra", objeto.CodigoBarra is null ? DBNull.Value : objeto.CodigoBarra);
                cmd.Parameters.AddWithValue("nombre", objeto.Nombre is null ? DBNull.Value : objeto.Nombre);
                cmd.Parameters.AddWithValue("marca", objeto.Marca is null ? DBNull.Value : objeto.Marca); 
                cmd.Parameters.AddWithValue("categoria", objeto.Categoria is null ? DBNull.Value : objeto.Categoria);
                cmd.Parameters.AddWithValue("precio", objeto.Precio == 0 ? DBNull.Value : objeto.Precio);

                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "editado" });


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }

        [HttpDelete]
        [Route("Eliminar/{idProducto:int}")]
        public IActionResult Eliminar(int idProducto)
        {
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                var cmd = new SqlCommand("sp_eliminar_producto", conexion);

                
                cmd.Parameters.AddWithValue("idProducto", idProducto);

                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Eliminado" });


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
