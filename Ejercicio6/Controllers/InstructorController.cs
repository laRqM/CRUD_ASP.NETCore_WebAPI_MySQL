using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.Annotations;

namespace Ejercicio6.Controllers;

[Route("api/instructores")]
[SwaggerTag("Todo lo relacionado a los instructores")]
[ApiController]
public class InstructorController : ControllerBase
{
    // Almacenamos una instancia de la interfaz IDb y/o declaramos un campo llamado _db de tipo privado,
    // solo lectura y con un tipo de dato IDb.
    private readonly IDb _db;
    
    // Inicializamos una variable de la clase Encriptacion.
    private Encriptacion _encrypt;

    // Constructor de la clase que espera que se le pase una instancia de un objeto que implemente la interfaz IDb.
    // Este parámetro se utiliza para inyectar la dependencia en la clase InstructorController.
    public InstructorController(IDb db)
    {
        _db = db;
        _encrypt = new Encriptacion(); // Creamos una nueva instancia de la clase Encriptacion.
    }
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Endpoint para crear un instructor",
        Description = "Requiere permisos de administrador",
        OperationId = "CrearInstructor",
        Tags = new[] { "Instructor" }
    )]
    public async Task<IActionResult> InsertarInstructor(Instructor nuevoInstructor)
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Verificamos si el modelo es válido según las reglas de validación definidas en el modelo.
                if (ModelState.IsValid)
                {
                    nuevoInstructor.nombre_uno = _encrypt.Encriptar(nuevoInstructor.nombre_uno); // Encriptamos el primer nombre.
                    
                    // Creamos la SQL Query que insertará los datos a la tabla persona.
                    var sqlInsertPersona = "INSERT INTO persona (nombre_uno, nombre_dos, apellido_uno, apellido_dos, D_nacimiento, tipo_rol) " +
                                           "VALUES (@nombre_uno, @nombre_dos, @apellido_uno, @apellido_dos, @D_nacimiento, @tipo_rol);";
                    
                    // Creamos la SQL Query que insertará los datos a la tabla instructor.
                    var sqlInsertInstructor = "INSERT INTO instructor (id_persona, folio) " +
                                          "VALUES (@id_persona, @folio);";

                    // Abrimos la conexión a la base de datos.
                    await conexion.OpenAsync();

                    // Iniciamos una transacción para asegurar la integridad de los datos.
                    using (var transaction = await conexion.BeginTransactionAsync())
                    {
                        // Insertamos en la tabla persona.
                        await conexion.ExecuteAsync(sqlInsertPersona, nuevoInstructor, transaction);

                        // Obtenemos el ID asignado automáticamente por la base de datos.
                        nuevoInstructor.id_persona = await conexion.ExecuteScalarAsync<uint>("SELECT LAST_INSERT_ID();", null, transaction);

                        // Insertamos en la tabla instructor.
                        await conexion.ExecuteAsync(sqlInsertInstructor, nuevoInstructor, transaction);

                        // Completamos la transacción.
                        await transaction.CommitAsync();
                    }

                    // Cerramos la conexión.
                    conexion.Close();

                    return Ok("Instructor insertado exitosamente.");
                }
                else
                {
                    // Si el modelo no es válido, retornamos un BadRequest.
                    return BadRequest(ModelState);
                }
            }
        }
        catch (Exception ex)
        {
            // Manejamos cualquier excepción y retornamos un error.
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Endpoint para listar los instructores",
        Description = "No se requieren permisos de administrador",
        OperationId = "ListarInstructors",
        Tags = new[] { "Instructor" }
    )]
    public async Task<IActionResult> GetInstructores()
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Creamos la SQL Query que llamará a los datos a la tabla persona e instructor mediante un INNER JOIN.
                var sql = "SELECT persona.id_persona, nombre_uno, nombre_dos, apellido_uno, apellido_dos," +
                          " D_nacimiento, tipo_rol, instructor.folio" +
                          " FROM persona INNER JOIN instructor ON persona.id_persona = instructor.id_persona";

                // Abrimos la conexión a la base de datos.
                await conexion.OpenAsync();

                // Ejecutamos la consulta y mapeamos los resultados a la lista de instructores.
                var resultados = await conexion.QueryAsync<Instructor>(sql);

                foreach (var instructor in resultados)
                {
                    instructor.nombre_uno = _encrypt.Desencriptar(instructor.nombre_uno); // Por cada resultado, desencriptamos el primer nombre.
                }

                // Cerramos la conexión.
                conexion.Close();

                return Ok(resultados); // Retornamos un Sucess pasándole lo obtenido en la variable resultados.
            }
        }
        catch (Exception ex)
        {
            // Manejamos cualquier excepción y retornamos un error.
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
    
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Endpoint para actualizar un instructor",
        Description = "Requiere permisos de administrador",
        OperationId = "ActualizarInstructor",
        Tags = new[] { "Instructor" }
    )]
    public async Task<IActionResult> ActualizarInstructor(int id, Instructor instructorActualizado)
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Verificamos si el modelo es válido según las reglas de validación definidas en el modelo.
                if (ModelState.IsValid)
                {
                    // Creamos la SQL Query que actualizará los datos en la tabla persona.
                    var sqlUpdatePersona = "UPDATE persona SET " +
                                           "nombre_uno = @NombreUno, nombre_dos = @NombreDos, " +
                                           "apellido_uno = @ApellidoUno, apellido_dos = @ApellidoDos, " +
                                           "D_nacimiento = @FechaNacimiento, tipo_rol = @Rol " +
                                           "WHERE id_persona = @IDPersona;";

                    // Creamos la SQL Query que actualizará los datos en la tabla instructor.
                    var sqlUpdateInstructor = "UPDATE instructor SET " +
                                          "folio = @Folio " +
                                          "WHERE id_persona = @IDPersona;";

                    // Abrimos la conexión a la base de datos.
                    await conexion.OpenAsync();

                    // Iniciamos una transacción para asegurar la integridad de los datos.
                    using (var transaction = await conexion.BeginTransactionAsync())
                    {
                        // Actualiza en la tabla persona.
                        await conexion.ExecuteAsync(sqlUpdatePersona, new
                        {
                            // Las variables en la SQL Query creada arriba(lado izquierdo) serán igual a lo recibido en el modelo(lado derecho).
                            NombreUno = _encrypt.Encriptar(instructorActualizado.nombre_uno),
                            NombreDos = instructorActualizado.nombre_dos,
                            ApellidoUno = instructorActualizado.apellido_uno,
                            ApellidoDos = instructorActualizado.apellido_dos,
                            FechaNacimiento = instructorActualizado.D_nacimiento,
                            Rol = instructorActualizado.tipo_rol,
                            IDPersona = id
                        }, transaction);

                        // Actualizamos en la tabla instructor.
                        await conexion.ExecuteAsync(sqlUpdateInstructor, new
                        {
                            Folio = instructorActualizado.folio,
                            IDPersona = id
                        }, transaction);

                        // Completamos la transacción.
                        await transaction.CommitAsync();
                    }

                    // Cerramos la conexión.
                    conexion.Close();

                    return Ok("Instructor actualizado exitosamente."); // Retornamos un Success pasándole una string.
                }
                else
                {
                    // Si el modelo no es válido, retornamos un BadRequest.
                    return BadRequest(ModelState);
                }
            }
        }
        catch (Exception ex)
        {
            // Manejamos cualquier excepción y retornamos un error.
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Endpoint para eliminar un instructor",
        Description = "Requiere permisos de administrador",
        OperationId = "EliminarInstructor",
        Tags = new[] { "Instructor" }
    )]
    public async Task<IActionResult> DeleteInstructor(int id)
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Inicia una transacción para asegurar la integridad de los datos.
                await conexion.OpenAsync();
                using (var transaction = await conexion.BeginTransactionAsync())
                {
                    // Elimina al instructor de la tabla instructor.
                    var sqlDeleteInstructor = "DELETE FROM instructor WHERE id_persona = @IDPersona";
                    await conexion.ExecuteAsync(sqlDeleteInstructor, new { IDPersona = id }, transaction);

                    // Elimina a la persona de la tabla persona.
                    // En la base de datos, la relación de instructor a persona tiene ON DELETE CASCADE, por lo que esto puede no ser necesario.
                    var sqlDeletePersona = "DELETE FROM persona WHERE id_persona = @IDPersona";
                    // Realizamos la eliminación de manera asíncrona usando Dapper.
                    await conexion.ExecuteAsync(sqlDeletePersona, new { IDPersona = id }, transaction);
                    
                    // await Indica que la ejecución del método espera de forma asincrónica hasta que la tarea se complete.
                    // conexion: El objeto de conexión a la base de datos.
                    // ExecuteAsync: Método proporcionado por Dapper que ejecuta una consulta SQL o un procedimiento almacenado en la base de datos y devuelve el número de filas afectadas.
                    
                    // sqlDeletePersona contiene la instrucción SQL para eliminar registros de la tabla "persona" en la base de datos.
                    // new { IDPersona = id }: Esto crea un objeto anónimo en C# con una propiedad llamada IDPersona y un valor asociado id.
                    // Dapper utiliza este objeto para asignar parámetros en la consulta SQL de manera segura para evitar la inyección de SQL.
                    // transaction: se utiliza para agrupar operaciones de base de datos en una transacción. El método ExecuteAsync se ejecutará dentro de esta transacción.

                    // Completamos la transacción.
                    await transaction.CommitAsync();
                }
            }

            return NoContent(); // Retornamos un NoContent. O un 204 pero sin datos que devolver.
        }
        catch (Exception ex)
        {
            // Manejamos cualquier excepción y retornamos un error.
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}