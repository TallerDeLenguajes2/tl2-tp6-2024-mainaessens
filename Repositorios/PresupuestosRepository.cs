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
                        Detalle = ObtenerDetalle(Convert.ToInt32(reader["idPresupuesto"]))
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
                            Detalle = ObtenerDetalle(id)
                        };
                    }
                }
            }
        }
        return presupuesto;
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
    public List<PresupuestoDetalle> ObtenerDetalle(int id)
    {
        string query = @"SELECT p.idProducto, p.Descripcion, p.Precio, pd.Cantidad 
                         FROM Productos AS p
                         INNER JOIN PresupuestosDetalle AS pd USING (idProducto)
                         WHERE pd.idPresupuesto = @idquery";

        List<PresupuestoDetalle> lista = new List<PresupuestoDetalle>();

        using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
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
