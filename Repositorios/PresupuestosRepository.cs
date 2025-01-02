using Microsoft.Data.Sqlite;

class PresupuestosRepository
{
    string connectionString = @"Data Source = Tienda.db;Cache=Shared";
    public void CrearNuevo(Presupuestos presupuesto)
    {
        string query1 = @"INSERT INTO Presupuestos (FechaCreacion, idCliente) VALUES (@fechaPre, @idC)";
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Inserta el presupuesto
            using (SqliteCommand command = new SqliteCommand(query1, connection))
            {
                command.Parameters.Add(new SqliteParameter("@fechaPre", presupuesto.FechaCreacion));
                command.Parameters.Add(new SqliteParameter("@idC", presupuesto.Cliente.Id));
                command.ExecuteNonQuery();
            }

            // Obtiene el último ID generado por la conexión
            string queryGetId = "SELECT last_insert_rowid()";
            using (SqliteCommand getIdCommand = new SqliteCommand(queryGetId, connection))
            {
                presupuesto.IdPresupuesto = Convert.ToInt32(getIdCommand.ExecuteScalar());
            }

            // Inserta los detalles usando el mismo ID y conexión
            foreach (var item in presupuesto.Detalle)
            {
                string query2 = @"INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPre, @idProdu, @canti)";
                using (SqliteCommand command = new SqliteCommand(query2, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@idPre", presupuesto.IdPresupuesto));
                    command.Parameters.Add(new SqliteParameter("@idProdu", item.Producto.IdProducto));
                    command.Parameters.Add(new SqliteParameter("@canti", item.Cantidad));
                    command.ExecuteNonQuery();
                }
            }
        }
    }


    public List<Presupuestos> ListarPresupuestos()
    {
        string queryDetalle = @"SELECT * 
                        FROM Presupuestos
                        LEFT JOIN Clientes USING (idCliente)";
        List<Presupuestos> presupuestos = new List<Presupuestos>();

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(queryDetalle, connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int idpres = Convert.ToInt32(reader["idPresupuesto"]);
                    DateTime fecha = Convert.ToDateTime(reader["FechaCreacion"]);
                    int idC = Convert.ToInt32(reader["idCliente"]);
                    string nombreC = Convert.ToString(reader["Nombre"]);
                    string emailC = Convert.ToString(reader["Email"]);
                    string telC = Convert.ToString(reader["Telefono"]);

                    Cliente nuevoCliente = new Cliente(idC, nombreC, emailC, telC);

                    Presupuestos presu1 = new Presupuestos(idpres, fecha, nuevoCliente);
                    presupuestos.Add(presu1);
                }
            }
        }
        return presupuestos;
    }

    public List<PresupuestoDetalle> ObtenerDetalle(int id)
    {
        string query = @"SELECT p.idProducto, p.Descripcion, p.Precio, pd.Cantidad 
                         FROM Productos AS p
                         INNER JOIN PresupuestosDetalle AS pd USING (idProducto)
                         WHERE pd.idPresupuesto = @idquery";

        List<PresupuestoDetalle> lista = new List<PresupuestoDetalle>();

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idquery", id));

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PresupuestoDetalle Pd = new PresupuestoDetalle();
                        Productos nuevoProducto = new Productos();

                        nuevoProducto.IdProducto = Convert.ToInt32(reader["idProducto"]);
                        nuevoProducto.Descripcion = Convert.ToString(reader["Descripcion"]);
                        nuevoProducto.Precio = Convert.ToInt32(reader["Precio"]);
                        Pd.Cantidad = Convert.ToInt32(reader["Cantidad"]);

                        Pd.Producto = nuevoProducto;

                        lista.Add(Pd);
                    }
                }
            }
        }
        return lista;
    }

    public Presupuestos ObtenerPresupuestoPorId(int id)
    {
        Presupuestos presupuesto = null;

        string query = @"SELECT 
            P.idPresupuesto,
            P.FechaCreacion,
            C.idCliente,
            C.Nombre,
            C.Email,
            C.Telefono,
            PR.idProducto,
            PR.Descripcion AS Producto,
            PR.Precio,
            PD.Cantidad
        FROM 
            Presupuestos P
        JOIN 
            Clientes C USING (idCliente)
        JOIN 
            PresupuestosDetalle PD ON P.idPresupuesto = PD.idPresupuesto
        JOIN 
            Productos PR ON PD.idProducto = PR.idProducto
        WHERE 
            P.idPresupuesto = @id;";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            int cont = 1;
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (cont == 1)
                    {
                        Cliente cliente = new Cliente(Convert.ToInt32(reader["idCliente"]), reader["Nombre"].ToString(), reader["Email"].ToString(), reader["Telefono"].ToString());
                        presupuesto = new Presupuestos(Convert.ToInt32(reader["idPresupuesto"]), Convert.ToDateTime(reader["FechaCreacion"]), cliente);
                    }
                    Productos producto = new Productos(Convert.ToInt32(reader["idProducto"]), reader["Producto"].ToString(), Convert.ToInt32(reader["Precio"]));
                    PresupuestoDetalle detalle = new PresupuestoDetalle(producto, Convert.ToInt32(reader["Cantidad"]));
                    presupuesto.Detalle.Add(detalle);
                    cont++;
                }
            }
            connection.Close();
        }
        return presupuesto;
    }

    public void EliminarPresupuesto(int idPresupuesto)
    {
        string query = @"DELETE FROM Presupuestos WHERE idPresupuesto = @IdP;";
        string query2 = @"DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @Id;";
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand(query, connection);
            SqliteCommand command2 = new SqliteCommand(query2, connection);
            command.Parameters.AddWithValue("@IdP", idPresupuesto);
            command2.Parameters.AddWithValue("@Id", idPresupuesto);
            command2.ExecuteNonQuery();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public void ModificarPresupuestoQ(Presupuestos presupuesto)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // 1. Actualiza el presupuesto principal
                    string queryPresupuesto = @"UPDATE Presupuestos 
                                          SET FechaCreacion = @fecha, idCliente = @idC
                                          WHERE idPresupuesto = @id";

                    using (var command = new SqliteCommand(queryPresupuesto, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);
                        command.Parameters.AddWithValue("@idC", presupuesto.Cliente.Id);
                        command.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
                        command.ExecuteNonQuery();
                    }

                    // 2. Elimina todos los detalles existentes
                    string deleteDetallesQuery = @"DELETE FROM PresupuestosDetalle 
                                             WHERE idPresupuesto = @idPr";

                    using (var command = new SqliteCommand(deleteDetallesQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@idPr", presupuesto.IdPresupuesto);
                        command.ExecuteNonQuery();
                    }

                    // 3. Inserta los nuevos detalles
                    if (presupuesto.Detalle != null && presupuesto.Detalle.Any())
                    {
                        string insertDetalleQuery = @"INSERT INTO PresupuestosDetalle 
                                                (idPresupuesto, idProducto, Cantidad)
                                                VALUES (@idPr, @idP, @cant)";

                        foreach (var detalle in presupuesto.Detalle)
                        {
                            using (var command = new SqliteCommand(insertDetalleQuery, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@idPr", presupuesto.IdPresupuesto);
                                command.Parameters.AddWithValue("@idP", detalle.Producto.IdProducto);
                                command.Parameters.AddWithValue("@cant", detalle.Cantidad);
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    // Si todo sale bien, confirma la transacción
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Si algo sale mal, deshace todos los cambios
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}




