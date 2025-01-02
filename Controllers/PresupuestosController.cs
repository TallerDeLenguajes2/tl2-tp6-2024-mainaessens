using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;

    private readonly PresupuestosRepository _presupuestosRepository;
    private readonly ClientesRepository _clienteRepository; 
    private readonly ProductoRepository _productoRepository; 

    public PresupuestosController(ILogger<PresupuestosController> logger)
    {
        _logger = logger;
        _presupuestosRepository = new PresupuestosRepository();
        _clienteRepository = new ClientesRepository(); 
        _productoRepository = new ProductoRepository(); 
    }

    [HttpGet]
    public IActionResult ListarPresupuestos()
    {
        var presupuestos = _presupuestosRepository.ListasPresupuestos();
        return View(presupuestos); 
    }

    [HttpGet]
    public IActionResult ListarDetalles(int id)
    {
        var presupuestos = _presupuestosRepository.ObtenerPresupuestoPorId(id);
        return View(presupuestos); 
    }

    [HttpGet] // formulario de creacion
    public IActionResult CrearPresupuesto()
    {
        var viewModel = new PresupuestoViewModel
        return View();
    }

    [HttpPost] // guardado del cliente
    [ValidateAntiForgeryToken]
    public IActionResult CrearCliente(Cliente cliente){
        if (ModelState.IsValid)
        {
            _clienteRepository.CrearCliente(cliente); 
            return RedirectToAction(nameof(Index));
        }
        return View(cliente); 
    }

    [HttpGet] //formulario de edicion
    public IActionResult ModificarCliente(int id){
        var cliente = _clienteRepository.ObtenerCliente(id);
        if (cliente == null)
        {
            return NotFound(); 
        }
        return View(cliente); 
    }

    [HttpPost] //guardo los cambios
    public IActionResult ModificarCliente(int id, Cliente cliente){
        if (ModelState.IsValid)
        {
            _clienteRepository.ModificarCliente(id, cliente); 
            return RedirectToAction(nameof(Index)); 
        }
        return View(cliente); 
    }

    [HttpGet] //confirmacion de eliminacion
    public IActionResult EliminarCliente(int id){
        var cliente = _clienteRepository.ObtenerCliente(id); 
        if (cliente == null)
        {
            return NotFound(); 
        }
        return View(cliente); // retorna vista de confirmacion con los datos del cliente
    }

    [HttpPost] //eliminacion confirmada
    [ValidateAntiForgeryToken] //Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarClienteConfirmado(int id){
        //En este caso no es necesario el ModelState.IsValid porque solo recibo un dato simple(id)
        _clienteRepository.EliminarCliente(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index(){ // Muestra la lista de clientes como la página principal del controlador.
        return View(_clienteRepository.ObtenerClientes()); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() // Maneja las excepciones y muestra una vista personalizada de error.
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
