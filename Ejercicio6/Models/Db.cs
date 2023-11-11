using System;
using MySql.Data.MySqlClient;

namespace Ejercicio6
{
    // Esta clase maneja la conexión a la base de datos en MySQL.
    public class Db : IDb // Implementamos la interfaz IDb.
    {
        public MySqlConnection Conexion() // Implementamos el método Conexion() de la interfaz IDb.
        {
            string server = "localhost";
            string port = "3306";
            string user = "root";
            string password = "";
            string dbname = "practica_cSharp";
            string fullString = "Database=" + dbname + "; Data Source=" + server + "; Port=" + port + "; User Id=" + user + "; Password=" + password + ";";

            try
            {
                MySqlConnection conn = new MySqlConnection(fullString);
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error al intentar conectarse a la base de datos: " + ex.Message);
                return null;
            }
        }
    }
}