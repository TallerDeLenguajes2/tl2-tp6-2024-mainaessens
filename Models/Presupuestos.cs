public class Presupuestos
{
    private int idPresupuesto;
    private DateTime fechaCreacion;
    private Cliente cliente;
    private List<PresupuestoDetalle> detalle;


    public int IdPresupuesto { get => idPresupuesto; set => idPresupuesto = value; }
    public DateTime FechaCreacion { get => fechaCreacion; set => fechaCreacion = value; }
    public Cliente Cliente { get => cliente; set => cliente = value; }
    public List<PresupuestoDetalle> Detalle { get => detalle; set => detalle = value; }

    public Presupuestos(int idPresupuesto, DateTime fechaCreacion, Cliente cliente)
    {
        this.idPresupuesto = idPresupuesto;
        this.fechaCreacion = fechaCreacion;
        this.cliente = cliente;
        detalle = new List<PresupuestoDetalle>();
    }

    public Presupuestos()
    {
        FechaCreacion = DateTime.Today;
        detalle = new List<PresupuestoDetalle>();
    }

    public double MontoPresupuesto()
    {
        int monto = detalle.Sum(d => d.Cantidad * d.Producto.Precio);
        return monto;
    }
    public double MontoPresupuestoConIva()
    {
        return MontoPresupuesto() * 1.21;
    }
    public int CantidadProductos()
    {
        return detalle.Sum(d => d.Cantidad);
    }


}