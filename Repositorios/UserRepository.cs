using Microsoft.Data.Sqlite; 

public class UserRepository : IUserRepository{
    private readonly string cadenaConexion; 
    public UserRepository(string cadenaconexion){
        cadenaConexion = cadenaconexion; 
    }

    public User GetUser(string username, string password){
        User user = null; 
        string query = @"SELECT * FROM Usuario WHERE usuario = @username AND password = contra"; 

        using(SqliteConnection connection = new SqliteConnection(cadenaConexion)){
            connection.Open(); 
            SqliteCommand command = new SqliteCommand(query, connection); 
            command.Parameters.AddWithValue("@username", username); 
            command.Parameters.AddWithValue("@contra", password); 
            using(SqliteDataReader reader = command.ExecuteReader()){
                while(reader.Read()){
                    user = new User(); 
                    user.Id = Convert.ToInt32(reader["id"]); 
                    user.Nombre = reader["nombre"].ToString(); 
                    user.Username = reader["usuario"].ToString(); 
                    user.Password = reader["password"].ToString(); 
                    user.AccessLevel = (AccessLevel)Convert.ToInt32(reader["id_rol"]);
                }
                connection.Close(); 
            }
            return user; 
        }
    }

    public void AltaUsuario(User usuario)
{
    // Cadena de la consulta SQL
    string query = @"INSERT INTO Usuario (nombre, usuario, password, id_rol) VALUES (@nombre, @usu, @contra, @rol)";

    // Usar un bloque 'using' para asegurar el cierre correcto de la conexión
    using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
    {
        try
        {
            // Abrir la conexión
            connection.Open();

            // Crear el comando SQL con los parámetros necesarios
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                // Añadir los parámetros de forma segura con los tipos correctos
                command.Parameters.Add("@nombre", SqliteType.Text).Value = usuario.Nombre;
                command.Parameters.Add("@usu", SqliteType.Text).Value = usuario.Username;
                command.Parameters.Add("@contra", SqliteType.Text).Value = usuario.Password;
                command.Parameters.Add("@rol", SqliteType.Integer).Value = (int)usuario.AccessLevel;

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            // Registrar o manejar el error en caso de una excepción
            // _logger.LogError("Error al insertar usuario: " + ex.Message);
            Console.WriteLine($"Error al insertar usuario: {ex.Message}");
        }
        finally
        {
            // Cerrar la conexión (aunque el bloque 'using' ya se encarga de ello)
            connection.Close();
        }
    }
}

}