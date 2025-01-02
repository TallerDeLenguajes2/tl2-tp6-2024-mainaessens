using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class PresupuestosRepository
{
    private string cadenaConexion = @"Data Source=C:\Users\naess\Documents\1. FACET\3° año\Segundo cuatrimestre\Taller de lenguaje II\tl2-tp5-2024-mainaessens\tienda.db";

    // Método para crear un nuevo presupuesto
    public void CrearNuevo(Presupuestos pres)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            var query = "INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@NombreDestinatario, @FechaCreacion)";
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@NombreDestinatario", pres.NombreDestinatario));
                command.Parameters.Add(new SqliteParameter("@FechaCreacion", pres.FechaCreacion));
                command.ExecuteNonQuery();
            }
        }
    }

    // Método para listar todos los presupuestos registrados
    public List<Presupuestos> ListarPresupuestos()
    {
        List<Presupuestos> listaPres = new List<Presupuestos>();
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            string query = "SELECT * FROM Presupuestos;";
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(query, connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var pres = new Presupuestos
                    {
                        IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                        NombreDestinatario = reader["nombreDestinatario"].ToString(),
                        Detalle = ObtenerDetalles(Convert.ToInt32(reader["idPresupuesto"]))
                    };
                    listaPres.Add(pres);
                }
            }
        }
        return listaPres;
    }

    // Método para obtener un presupuesto por ID
    public Presupuestos ObtenerPresupuestoPorId(int id)
    {
        Presupuestos presupuesto = null;
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            string query = "SELECT * FROM Presupuestos WHERE idPresupuesto = @idPresupuesto;";
            connection.Open();
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        presupuesto = new Presupuestos
                        {
                            IdPresupuesto = Convert.ToInt32(reader["idPresupuesto"]),
                            NombreDestinatario = reader["nombreDestinatario"].ToString(),
                            Detalle = ObtenerDetalles(id)
                        };
                    }
                }
            }
        }
        return presupuesto;
    }

    // Método para agregar un producto a un presupuesto
    public void AgregarProductoAPresupuesto(int idPresupuesto, Productos producto, int cantidad)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            string query = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad)";
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@cantidad", cantidad);
                command.ExecuteNonQuery();
            }
        }
    }

    // Método para eliminar un presupuesto por ID
    public void EliminarPresupuesto(int id)
    {
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            // Eliminar el presupuesto
            string query = "DELETE FROM Presupuestos WHERE idPresupuesto = @idPresupuesto";
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                command.ExecuteNonQuery();
            }

            // Eliminar detalles del presupuesto
            query = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idPresupuesto", id));
                command.ExecuteNonQuery();
            }
        }
    }

    // Método auxiliar para obtener detalles de un presupuesto
    private List<PresupuestoDetalle> ObtenerDetalles(int idPresupuesto)
    {
        List<PresupuestoDetalle> detalles = new List<PresupuestoDetalle>();
        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
        {
            string query = "SELECT * FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto";
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.Add(new SqliteParameter("@idPresupuesto", idPresupuesto));
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var detalle = new PresupuestoDetalle
                        {
                            Producto = ObtenerProductoPorId(Convert.ToInt32(reader["idProducto"])),
                            Cantidad = Convert.ToInt32(reader["cantidad"])
                        };
                        detalles.Add(detalle);
                    }
                }
            }
        }
        return detalles;
    }

    // Método auxiliar para obtener un producto por ID
    private Productos ObtenerProductoPorId(int idProducto)
    {
        Productos producto = null;
        using (var connection = new SqliteConnection(cadenaConexion))
        {
            string query = "SELECT * FROM Productos WHERE IdProducto = @IdProducto";
            connection.Open();
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdProducto", idProducto);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        producto = new Productos
                        {
                            IdProducto = Convert.ToInt32(reader["IdProducto"]),
                            Descripcion = reader["Descripcion"].ToString(),
                            Precio = Convert.ToInt32(reader["Precio"])
                        };
                    }
                }
            }
        }
        return producto;
    }

    public void ModificarPresupuestoQ(Presupuestos presupuesto)
    {
        using (var connection = new SqliteConnection(cadenaConexion))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction()) //Preguntar como funciona esto
            {

                // Actualiza el presupuesto en la tabla Presupuestos
                string query = @"UPDATE Presupuestos 
                                 SET NombreDestinatario = @destinatario, FechaCreacion = @fecha
                                 WHERE idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@destinatario", presupuesto.NombreDestinatario);
                    command.Parameters.AddWithValue("@fecha", presupuesto.FechaCreacion);
                    command.Parameters.AddWithValue("@id", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }

                // Actualiza la tabla PresupuestosDetalle
                if (presupuesto.Detalle != null)
                {
                    foreach (var detalle in presupuesto.Detalle)
                    {

                        string updateDetalleQuery = @"UPDATE PresupuestosDetalle 
                                                       SET Cantidad = @cant
                                                       WHERE idPresupuesto = @idPr AND idProducto = @idP";

                        using (var command = new SqliteCommand(updateDetalleQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@cant", detalle.Cantidad);
                            command.Parameters.AddWithValue("@idP", detalle.Producto.IdProducto);
                            command.Parameters.AddWithValue("@idPr", presupuesto.IdPresupuesto);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                // Si todo sale bien, se confirma la transacción
                transaction.Commit();
            }
        }
    }

}
