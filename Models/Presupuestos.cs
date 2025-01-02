public class Presupuestos{
    private int idPresupuesto; 
    private string nombreDestinatario; 
    private List<PresupuestoDetalle> detalle;
    private DateTime fechaCreacion;

    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public string NombreDestinatario { get => nombreDestinatario; set => nombreDestinatario = value; }
    public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }
    public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }

    public double MontoPresupuesto(){
        return detalle.Sum(d => d.Producto.Precio * d.Cantidad);
    }

    public double MontoPresupuestoConIva(){
        double iva = 1.21; 
        return MontoPresupuesto() * iva; 
    }

    public int CantidadProductos(){
        return Detalle.Sum(d => d.Cantidad); 
    }

    public void AgregarProducto(Productos prod, int cant){
        PresupuestoDetalle pd = new PresupuestoDetalle(); 
        pd.CargaProducto(prod); 
        pd.Cantidad = cant; 
        detalle.Add(pd);
    }
}