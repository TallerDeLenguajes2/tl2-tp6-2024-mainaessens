using System.ComponentModel.DataAnnotations;
public class Cliente
{
    private int id;
    private string nombre;
    private string email;
    private string telefono;

    public int Id { get => id; set => id = value; }
    public string Nombre { get => nombre; set => nombre = value; }
    public string Email { get => email; set => email = value; }
    public string Telefono { get => telefono; set => telefono = value; }

    public Cliente() { }

    public Cliente(int id, string nombre, string email, string telefono)
    {
        this.Id = id;
        Nombre = nombre;
        Email = email;
        Telefono = telefono;
    }

    public Cliente(AltaClienteViewModel clienteVM)
    {
        Nombre = clienteVM.Nombre;
        Email = clienteVM.Email;
        Telefono = clienteVM.Telefono;
    }

    public Cliente(ModificarClienteViewModel clienteVM)
    {
        Id = clienteVM.ClienteId;
        Nombre = clienteVM.Nombre;
        Email = clienteVM.Email;
        Telefono = clienteVM.Telefono;
    }
}