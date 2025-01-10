using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class ClientesController : Controller
{
    private readonly ILogger<ClientesController> _logger;

    private IClientesRepository _clienteRepository;

    public ClientesController(ILogger<ClientesController> logger, IClientesRepository _clienteRepos)
    {
        _logger = logger;
        _clienteRepository = _clienteRepos;
    }

    public IActionResult Index()
    {
    try
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

        ViewData["EsAdmin"] = HttpContext.Session.GetString("AccessLevel") == "Admin";
        return View(_clienteRepository.ObtenerClientes());
    }
    catch (Exception ex)
    {
        _logger.LogError(ex.ToString());
        ViewBag.ErrorMessage = "No se pudo cargar la lista de clientes";
        return RedirectToAction("Index");
    }
    }

    [HttpGet]
    public IActionResult ListarClientes()
    {
        try
        {
            var clientes = _clienteRepository.ObtenerClientes;
            return View(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo obtener la lista de clientes.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] // formulario de creacion
    public IActionResult CrearCliente()
    {
        try
        {
           if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

        if (HttpContext.Session.GetString("AccessLevel") != "Admin")
        {
            TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
            return RedirectToAction("Index");
        }

        return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el formulario de creación de cliente.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost] // guardado del cliente
    [ValidateAntiForgeryToken]
    public IActionResult CrearCliente(AltaClienteViewModel clienteVM)
    {
        try
        {
           if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("AccessLevel") != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid) 
                return RedirectToAction("Index");

            var clien = new Cliente(clienteVM);
            _clienteRepository.CrearCliente(clien);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo crear el cliente.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] // formulario de edicion
    public IActionResult ModificarCliente(int id)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

        if (HttpContext.Session.GetString("AccessLevel") != "Admin")
        {
            TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
            return RedirectToAction("Index");
        }

        var cliente = _clienteRepository.ObtenerCliente(id);
        var clienteVM = new ModificarClienteViewModel(cliente);
        return View(clienteVM);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el formulario de edición de cliente.";
            return RedirectToAction("Error");
        }
    }

    [HttpPost] // guardo los cambios
    public IActionResult ModificarCliente(Cliente cliente)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");
            if (HttpContext.Session.GetString("AccessLevel") != "Admin")
        {
            TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid) 
            return RedirectToAction("Index");

        _clienteRepository.ModificarCliente(cliente);
        return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo modificar el cliente.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] // confirmacion de eliminacion
    public IActionResult EliminarCliente(int id)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("AccessLevel") != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
                return RedirectToAction("Index");
            }

            return View(_clienteRepository.ObtenerCliente(id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el cliente para eliminar.";
            return RedirectToAction("Error");
        }
    }

    [HttpPost] // eliminacion confirmada
    [ValidateAntiForgeryToken] // Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarClienteConfirmado(int id)
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("AccessLevel") != "Admin")
            {
                TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
                return RedirectToAction("Index");
            }

            _clienteRepository.EliminarCliente(id);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo eliminar el cliente.";
            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() // Maneja las excepciones y muestra una vista personalizada de error.
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
