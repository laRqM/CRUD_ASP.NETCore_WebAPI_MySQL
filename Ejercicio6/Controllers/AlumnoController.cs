using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.Annotations;

namespace Ejercicio6.Controllers;

[Route("api/alumnos")]
[SwaggerTag("Todo lo relacionado a los alumnos")]
[ApiController]
public class AlumnoController : ControllerBase
{
    // Almacenamos una instancia de la interfaz IDb y/o declaramos un campo llamado _db de tipo privado,
    // solo lectura y con un tipo de dato IDb.
    private readonly IDb _db;
    
    // Inicializamos una variable de la clase Encriptacion.
    private Encriptacion _encrypt;

    // Constructor de la clase que espera que se le pase una instancia de un objeto que implemente la interfaz IDb.
    // Este parámetro se utiliza para inyectar la dependencia en la clase AlumnoController.
    public AlumnoController(IDb db)
    {
        _db = db;
        _encrypt = new Encriptacion(); // Creamos una nueva instancia de la clase Encriptacion.
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Endpoint para crear un alumno",
        Description = "Requiere permisos de administrador",
        OperationId = "CrearAlumno",
        Tags = new[] { "Alumno" }
    )]
    public async Task<IActionResult> InsertarAlumno(Alumno nuevoAlumno)
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Verificamos si el modelo es válido según las reglas de validación definidas en el modelo.
                if (ModelState.IsValid)
                {
                    nuevoAlumno.nombre_uno = _encrypt.Encriptar(nuevoAlumno.nombre_uno); // Encriptamos el primer nombre.
                    
                    // Creamos la SQL Query que insertará los datos a la tabla persona.
                    var sqlInsertPersona = "INSERT INTO persona (nombre_uno, nombre_dos, apellido_uno, apellido_dos, D_nacimiento, tipo_rol) " +
                                           "VALUES (@nombre_uno, @nombre_dos, @apellido_uno, @apellido_dos, @D_nacimiento, @tipo_rol);";
                    
                    // Creamos la SQL Query que insertará los datos a la tabla alumno.
                    var sqlInsertAlumno = "INSERT INTO alumno (id_persona, matricula, carrera, semestre, especialidad) " +
                                          "VALUES (@id_persona, @matricula, @carrera, @semestre, @especialidad);";

                    // Abrimos la conexión a la base de datos.
                    await conexion.OpenAsync();

                    // Iniciamos una transacción para asegurar la integridad de los datos.
                    using (var transaction = await conexion.BeginTransactionAsync())
                    {
                        // Insertamos en la tabla persona.
                        await conexion.ExecuteAsync(sqlInsertPersona, nuevoAlumno, transaction);

                        // Obtenemos el ID asignado automáticamente por la base de datos.
                        nuevoAlumno.id_persona = await conexion.ExecuteScalarAsync<uint>("SELECT LAST_INSERT_ID();", null, transaction);

                        // Insertamos en la tabla "alumno".
                        await conexion.ExecuteAsync(sqlInsertAlumno, nuevoAlumno, transaction);

                        // Completamos la transacción.
                        await transaction.CommitAsync();
                    }

                    // Cerramos la conexión.
                    conexion.Close();

                    return Ok("Alumno insertado exitosamente.");
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
        Summary = "Endpoint para listar los alumnos",
        Description = "No se requieren permisos de administrador",
        OperationId = "ListarAlumnos",
        Tags = new[] { "Alumno" }
    )]
    public async Task<IActionResult> GetAlumnos()
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Creamos la SQL Query que llamará a los datos a la tabla persona y alumno mediante un INNER JOIN.
                var sql = "SELECT persona.id_persona, nombre_uno, nombre_dos, apellido_uno, apellido_dos," +
                          " D_nacimiento, tipo_rol, alumno.matricula, alumno.carrera, alumno.semestre, alumno.especialidad" +
                          " FROM persona INNER JOIN alumno ON persona.id_persona = alumno.id_persona";

                // Abrimos la conexión a la base de datos.
                await conexion.OpenAsync();

                // Ejecutamos la consulta y mapeamos los resultados a la lista de alumnos.
                var resultados = await conexion.QueryAsync<Alumno>(sql);

                foreach (var alumno in resultados)
                {
                    alumno.nombre_uno = _encrypt.Desencriptar(alumno.nombre_uno); // Por cada resultado, desencriptamos el primer nombre.
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
        Summary = "Endpoint para actualizar un alumno",
        Description = "Requiere permisos de administrador",
        OperationId = "ActualizarAlumno",
        Tags = new[] { "Alumno" }
    )]
    public async Task<IActionResult> ActualizarAlumno(int id, Alumno alumnoActualizado)
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

                    // Creamos la SQL Query que actualizará los datos en la tabla alumno.
                    var sqlUpdateAlumno = "UPDATE alumno SET " +
                                          "matricula = @Matricula, carrera = @Carrera, " +
                                          "semestre = @Semestre, especialidad = @Especialidad " +
                                          "WHERE id_persona = @IDPersona;";

                    // Abrimos la conexión a la base de datos.
                    await conexion.OpenAsync();

                    // Iniciamos una transacción para asegurar la integridad de los datos.
                    using (var transaction = await conexion.BeginTransactionAsync())
                    {
                        // Actualizamos en la tabla persona.
                        await conexion.ExecuteAsync(sqlUpdatePersona, new
                        {
                            // Las variables en la SQL Query creada arriba(lado izquierdo) serán igual a lo recibido en el modelo(lado derecho).
                            NombreUno = _encrypt.Encriptar(alumnoActualizado.nombre_uno),
                            NombreDos = alumnoActualizado.nombre_dos,
                            ApellidoUno = alumnoActualizado.apellido_uno,
                            ApellidoDos = alumnoActualizado.apellido_dos,
                            FechaNacimiento = alumnoActualizado.D_nacimiento,
                            Rol = alumnoActualizado.tipo_rol,
                            IDPersona = id
                        }, transaction);

                        // Actualizamos en la tabla alumno.
                        await conexion.ExecuteAsync(sqlUpdateAlumno, new
                        {
                            // Las variables en la SQL Query creada arriba(lado izquierdo) serán igual a lo recibido en el modelo(lado derecho).
                            Matricula = alumnoActualizado.Matricula,
                            Carrera = alumnoActualizado.Carrera,
                            Semestre = alumnoActualizado.Semestre,
                            Especialidad = alumnoActualizado.Especialidad,
                            IDPersona = id
                        }, transaction);

                        // Completamos la transacción.
                        await transaction.CommitAsync();
                    }

                    // Cerramos la conexión.
                    conexion.Close();

                    return Ok("Alumno actualizado exitosamente."); // Retornamos un Success pasándole una string.
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
            // Manejamos cualquier excepción y retornar un error.
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Endpoint para eliminar un alumno",
        Description = "Requiere permisos de administrador",
        OperationId = "EliminarAlumno",
        Tags = new[] { "Alumno" }
    )]
    public async Task<IActionResult> DeleteAlumno(int id)
    {
        try
        {
            using (MySqlConnection conexion = _db.Conexion())
            {
                // Inicia una transacción para asegurar la integridad de los datos.
                await conexion.OpenAsync();
                using (var transaction = await conexion.BeginTransactionAsync())
                {
                    // Eliminamos al alumno de la tabla alumno.
                    var sqlDeleteAlumno = "DELETE FROM alumno WHERE id_persona = @IDPersona";
                    await conexion.ExecuteAsync(sqlDeleteAlumno, new { IDPersona = id }, transaction);

                    // Elimina a la persona de la tabla persona.
                    // En la base de datos, la relación de alumno a persona tiene ON DELETE CASCADE, por lo que esto puede no ser necesario.
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