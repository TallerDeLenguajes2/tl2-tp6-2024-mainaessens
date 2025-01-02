public class ProductoSeleccionado{
    public int ProductoId {get; set;}
    public int Cantidad {get; set;}

    public class PresupuestoViewModel{
        public List<Productos> productos {get; set; } = new List<Productos>(); 
        public List<Cliente> clientes {get; set; } = new List<Cliente>(); 
        public int ClienteIdSeleccionado {get; set; }

        // lista para almacenar productos seleccionados y sus cantidades
        public List<ProductoSeleccionado> productoSeleccionados {get; set; } = new List<ProductoSeleccionado>(); 
    }
}