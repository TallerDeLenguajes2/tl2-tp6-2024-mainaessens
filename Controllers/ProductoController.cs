using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;
    private readonly ProductoRepository _productosRepository; 

    public ProductosController(ILogger<ProductosController> logger, ProductoRepository _productosRepo)
    {
        _logger = logger;
        _productosRepository = _productosRepo; 
    }

    private bool VerificarPermisos()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
        {
            return false;
        }

        if (HttpContext.Session.GetString("AccessLevel") != "Admin")
        {
            TempData["ErrorMessage"] = "No tienes permisos para realizar esta acción.";
            return false;
        }

        return true;
    }

    [HttpGet]
    public IActionResult ListarProductos()
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            var productos = _productosRepository.ListarProductos();
            return View(productos); // obtiene una lista de productos desde el repositorio y la envía a la vista.
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al listar productos: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar listar los productos.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] // formulario de creacion
    public IActionResult CrearProducto()
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            return View(); // muestra una vista vacia para crear producto
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cargar formulario de creación de producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar acceder al formulario de creación.";
            return RedirectToAction("Error");
        }
    }

    [HttpPost] // guardado del producto
    [ValidateAntiForgeryToken]
    public IActionResult CrearProducto(Productos producto)
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _productosRepository.CrearNuevo(producto); 
                return RedirectToAction(nameof(Index));
            }
            return View(producto); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar crear el producto.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] //formulario de edicion
    public IActionResult ModificarProducto(int id)
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            var producto = _productosRepository.ObtenerProductoPorId(id);
            if (producto == null)
            {
                return NotFound(); 
            }
            return View(producto); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al modificar producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar acceder al formulario de modificación.";
            return RedirectToAction("Error");
        }
    }

    [HttpPost] //guardo los cambios
    public IActionResult ModificarProducto(int id, Productos producto)
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _productosRepository.ModificarProducto(id, producto); 
                return RedirectToAction(nameof(Index)); 
            }
            return View(producto); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al modificar producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar modificar el producto.";
            return RedirectToAction("Error");
        }
    }

    [HttpGet] //confirmacion de eliminacion
    public IActionResult EliminarProducto(int id)
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            var producto = _productosRepository.ObtenerProductoPorId(id); 
            if (producto == null)
            {
                return NotFound(); 
            }
            return View(producto); // retorna vista de confirmacion con los datos del producto
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el producto.";
            return RedirectToAction("Error");
        }
    }

    [HttpPost] //eliminacion confirmada
    [ValidateAntiForgeryToken] //Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarProductoConfirmado(int id)
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            _productosRepository.EliminarProducto(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al confirmar eliminación de producto: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el producto.";
            return RedirectToAction("Error");
        }
    }

    public IActionResult Index() 
    {
        try
        {
            if (!VerificarPermisos()) 
                return RedirectToAction("Index", "Login");

            return View(_productosRepository.ListarProductos()); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al cargar la página principal: {ex.Message}");
            TempData["ErrorMessage"] = "Ocurrió un error al intentar cargar la página principal.";
            return RedirectToAction("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
