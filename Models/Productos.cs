using System.ComponentModel.DataAnnotations;
public class Productos{
    private int idProducto; 
    private string descripcion; 
    private int precio;

    public int IdProducto { get => idProducto; set => idProducto = value; }
    public string Descripcion { get => descripcion; set => descripcion = value; }
    [StringLength(200, ErrorMessage = "La descripcion debe tener menos de 2000 caracteres")]
    public int Precio { get => precio; set => precio = value; }
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
}
