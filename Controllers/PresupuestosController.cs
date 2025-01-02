using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;

    private readonly PresupuestosRepository _presupuestosRepository;

    public PresupuestosController(ILogger<PresupuestosController> logger)
    {
        _logger = logger;
        _presupuestosRepository = new PresupuestosRepository();
    }

    [HttpGet]
    public IActionResult ListarPresupuestos()
    {
        var presupuestos = _presupuestosRepository.ListarPresupuestos();
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
        return View();
    }

    [HttpPost] // guardado del presupuesto
    [ValidateAntiForgeryToken]
    public IActionResult CrearPresupuesto(Presupuestos presupuesto){
        if (ModelState.IsValid) // se utiliza para verificar si los datos enviados en un formulario cumplen con todas las reglas de validación definidas en el modelo de datos.
        {
            _presupuestosRepository.CrearNuevo(presupuesto);
            return RedirectToAction(nameof(Index));
        }
        return View(presupuesto); 
    }

    [HttpGet] //formulario de edicion
    public IActionResult ModificarPresupuesto(int id){
        var presupuesto = _presupuestosRepository.ObtenerPresupuestoPorId(id);
        if (presupuesto == null)
        {
            return NotFound(); 
        }
        return View(presupuesto); 
    }

    [HttpPost] //guardo los cambios
    public IActionResult ModificarPresupuesto(Presupuestos presupuesto){
        if (ModelState.IsValid)
        {
            _presupuestosRepository.ModificarPresupuestoQ(presupuesto);
            return RedirectToAction(nameof(Index)); 
        }
        return View(presupuesto); 
    }

    [HttpGet] //confirmacion de eliminacion
    public IActionResult EliminarPresupuesto(int id){
        var presupuesto = _presupuestosRepository.ObtenerPresupuestoPorId(id); 
        if (presupuesto == null)
        {
            return NotFound(); 
        }
        return View(presupuesto); // retorna vista de confirmacion con los datos del cliente
    }

    [HttpPost] //eliminacion confirmada
    [ValidateAntiForgeryToken] //Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarClienteConfirmado(int id){
        //En este caso no es necesario el ModelState.IsValid porque solo recibo un dato simple(id)
        _presupuestosRepository.EliminarPresupuesto(id); 
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index(){ // Muestra la lista de clientes como la página principal del controlador.
        return View(_presupuestosRepository.ListarPresupuestos()); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() // Maneja las excepciones y muestra una vista personalizada de error.
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
