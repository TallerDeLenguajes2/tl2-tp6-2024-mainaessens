using System.ComponentModel.DataAnnotations;

public class Cliente
{
    private int id;
    private string nombre;
    private string email;
    private string telefono;

    public Cliente() { }

    public Cliente(int id, string nombre, string email, string telefono)
    {
        this.id = id;
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    public int Id { get => id; set => id = value; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre debe tener menos de 100 caracteres")]
    public string Nombre { get => nombre; set => nombre = value; }

    [EmailAddress(ErrorMessage = "Debe tener formato de correo electronico")]
    [Required(ErrorMessage = "El email es obligatorio")]
    public string Email { get => email; set => email = value; }

    [Phone(ErrorMessage = "Debe tener formato de número de telefono")]
    [Required(ErrorMessage = "El telefono es obligatorio")]
    public string Telefono { get => telefono; set => telefono = value; }
}