public interface IProductoRepository{
    void CrearNuevo(Productos producto); 
    void ModificarProducto(int id, Productos producto); 
    List<Productos> ListarProductos();
    Productos ObtenerProductoPorId(int id);
    void EliminarProducto(int id);  
}