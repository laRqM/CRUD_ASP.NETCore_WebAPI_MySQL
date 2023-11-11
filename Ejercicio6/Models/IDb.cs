using MySql.Data.MySqlClient;

namespace Ejercicio6;

// Esta interfaz será implementada en la clase Db. Esto permitirá llamar a la cadena de conexión
// desde los controladores sin necesidad de implementar la clase Db en algún controlador.
// Puesto que ControllerBase debe ser implementado en cada controlador.
public interface IDb
{
    MySqlConnection Conexion(); // Definimos el método Conexion().
}