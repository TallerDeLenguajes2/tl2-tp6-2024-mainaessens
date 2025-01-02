using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class ProductosController : Controller
{
    private readonly ILogger<ProductosController> _logger;

    private readonly ProductoRepository _productosRepository; 

    public ProductosController(ILogger<ProductosController> logger)
    {
        _logger = logger;
        _productosRepository = new ProductoRepository(); 
    }

    [HttpGet]
    public IActionResult ListarProductos()
    {
        var productos = _productosRepository.ListarProductos();
        return View(productos); // obtiene una lista de productos desde el repositorio y la envía a la vista.
    }

    [HttpGet] // formulario de creacion
    public IActionResult CrearProducto()
    {
        return View(); // muestra una vista vacia para crear producto
    }

    [HttpPost] // guardado del producto
    [ValidateAntiForgeryToken]
    public IActionResult CrearProducto(Productos producto){
        if (ModelState.IsValid)
        {
            _productosRepository.CrearNuevo(producto); 
            return RedirectToAction(nameof(Index));
        }
        return View(producto); 
    }

    [HttpGet] //formulario de edicion
    public IActionResult ModificarProducto(int id){
        var producto = _productosRepository.ObtenerProductoPorId(id);
        if (producto == null)
        {
            return NotFound(); 
        }
        return View(producto); 
    }

    [HttpPost] //guardo los cambios
    public IActionResult ModificarProducto(int id, Productos producto){
        if (ModelState.IsValid)
        {
            _productosRepository.ModificarProducto(id, producto); 
            return RedirectToAction(nameof(Index)); 
        }
        return View(producto); 
    }

    [HttpGet] //confirmacion de eliminacion
    public IActionResult EliminarProducto(int id){
        var producto = _productosRepository.ObtenerProductoPorId(id); 
        if (producto == null)
        {
            return NotFound(); 
        }
        return View(producto); // retorna vista de confirmacion con los datos del producto
    }

    [HttpPost] //eliminacion confirmada
    [ValidateAntiForgeryToken] //Es una buena práctica proteger las acciones POST con tokens antifalsificación para prevenir ataques Cross-Site Request Forgery (CSRF).
    public IActionResult EliminarProductoConfirmado(int id){
        //En este caso no es necesario el ModelState.IsValid porque solo recibo un dato simple(id)
        _productosRepository.EliminarProducto(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index(){ // Muestra la lista de productos como la página principal del controlador.
        return View(_productosRepository.ListarProductos()); 
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() // Maneja las excepciones y muestra una vista personalizada de error.
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
