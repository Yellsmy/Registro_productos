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
    public class RollController : Controller
    {
        /*Declaracion de variable de tipo privado solamente de lectura que será una cadena de texto*/
        private readonly string cadenaSQL;

        /*Creación del constructor */
        public RollController(IConfiguration config)
        {
            /*Se almacena la cadena de conexión a la base de datos*/
            cadenaSQL = config.GetConnectionString("CadenaSQL2");

        }

        [HttpGet]
        [Route("Lista")]
        public IActionResult Lista()
        {
            //List<Roll> lista = new List<Roll>(); //Borrar
            
            try
            {
                using var conexion = new SqlConnection(cadenaSQL);
                conexion.Open();
                var cmd = new SqlCommand("crud_rol", conexion);
                /*Se le asigna un tipo a la variable cmd*/
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@opcion",4);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet setter = new DataSet();
                /*using (var rd = cmd.ExecuteReader()) //En vez de ExecuteReader el ExecuteNonQuery
                {
                    while (rd.Read())
                    {
                        lista.Add(new Roll()
                        {
                            id = Convert.ToInt32(rd["id"]),
                            nombre = rd["nombre"].ToString(),
                            estado = Convert.ToInt32(rd["estado"]),
                            creado = rd["creado"].ToString()
                        });
                    }
                }*/

                adapter.Fill(setter, "tabla");

                //return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
                return Ok(setter.Tables["tabla"]);


            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = "" });
            }
        }

    }
}
