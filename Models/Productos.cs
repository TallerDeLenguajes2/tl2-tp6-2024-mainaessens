using System.ComponentModel.DataAnnotations;

public class Productos
{
    private int idProducto;
    private string descripcion;
    private int precio;

    public Productos() {}

    public Productos(int idProducto, string descripcion, int precio)
    {
        this.idProducto = idProducto;
        Descripcion = descripcion;
        Precio = precio;
    }

    public int IdProducto { get => idProducto; set => idProducto = value; }

    [Required(ErrorMessage = "La descripcion es obligatoria")]
    [StringLength(250, ErrorMessage = "La descripcion no puede tener más de 250 caracteres.")]
    public string Descripcion { get => descripcion; set => descripcion = value; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "El precio debe ser un valor positivo")]
    public int Precio { get => precio; set => precio = value; }
}
