using Microsoft.Data.Sqlite; 
public class ProductoRepository{
    private string cadenaConexion = @"Data Source=C:\Users\naess\Documents\1. FACET\3° año\Segundo cuatrimestre\Taller de lenguaje II\tl2-tp6-2024-mainaessens\tienda.db";
    public void CrearNuevo(Productos prod)
    {
            using ( SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                var query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@Descripcion", prod.Descripcion));
                command.Parameters.Add(new SqliteParameter("@Precio", prod.Precio));
                command.ExecuteNonQuery();
                connection.Close();
            }
    }

    public void ModificarProducto(int id, Productos prod){
        using ( SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                var query = "UPDATE Productos SET Descripcion = @Descripcion, Precio = @Precio WHERE idProducto = @IdProducto";
                connection.Open();
                var command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@Descripcion", prod.Descripcion));
                command.Parameters.Add(new SqliteParameter("@Precio", prod.Precio));
                command.Parameters.Add(new SqliteParameter("@IdProducto", id));
                command.ExecuteNonQuery();
                connection.Close();
            }
    }

    public List<Productos> ListarProductos()
    {
            List<Productos> listaProd = new List<Productos>();
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                string query = "SELECT * FROM Productos;";
                SqliteCommand command = new SqliteCommand(query, connection);
                connection.Open();
                using(SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var prod = new Productos();
                        prod.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        prod.Descripcion = reader["Descripcion"].ToString();
                        prod.Precio = Convert.ToInt32(reader["Precio"]);
                        listaProd.Add(prod);
                    }
                }
                connection.Close();

            }
            return listaProd;
    }

    public Productos ObtenerProductoPorId(int id){
        Productos prod = null; 
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                string query = "SELECT * FROM Productos WHERE IdProducto = @IdProducto;";
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.Add(new SqliteParameter("@IdProducto", id));
                connection.Open();
                using(SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prod = new Productos();
                        prod.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        prod.Descripcion = reader["Descripcion"].ToString();
                        prod.Precio = Convert.ToInt32(reader["Precio"]);
                    }
                }
                connection.Close();

            }
        return prod; 
    }

    public void EliminarProducto(int id){
     using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                string query = "DELETE FROM Productos WHERE idProducto = @IdProducto"; 
                connection.Open();
                var command = new SqliteCommand(query, connection); 
                command.Parameters.Add(new SqliteParameter("@IdProducto", id)); 
                command.ExecuteNonQuery();
                connection.Close();
            }
    }
}