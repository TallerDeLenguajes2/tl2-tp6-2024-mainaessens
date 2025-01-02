public class ProductoSeleccionado
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class PresupuestoViewModel
{
    public List<Productos> Productos { get; set; } = new List<Productos>();
    public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    public int ClienteIdSeleccionado { get; set; }
    
    // Lista para almacenar productos seleccionados y sus cantidades
    public List<ProductoSeleccionado> ProductosSeleccionados { get; set; } = new List<ProductoSeleccionado>();
}
