using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp6_2024_mainaessens.Models;

namespace tl2_tp6_2024_mainaessens.Controllers;

public class PresupuestosController : Controller
{
    private readonly ILogger<PresupuestosController> _logger;
    private readonly IPresupuestoRepository _presupuestosRepository;
    private readonly IClientesRepository _clientesRepository;
    private readonly IProductoRepository _productosRepository;

    public PresupuestosController(ILogger<PresupuestosController> logger, IPresupuestoRepository presupuestosRepo, IClientesRepository clientesRepo, IProductoRepository productosRepo)
    {
        _logger = logger;
        _presupuestosRepository = presupuestosRepo;
        _clientesRepository = clientesRepo;
        _productosRepository = productosRepo;
    }

    [HttpGet]
    public IActionResult ListarPresupuestos()
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

            var presupuestos = _presupuestosRepository.ListarPresupuestos();
            return View(presupuestos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el listado de presupuestos.";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpGet]
    public IActionResult ListarDetalles(int id)
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

            var presupuesto = _presupuestosRepository.ObtenerPresupuestoPorId(id);
            return View(presupuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo mostrar el presupuesto.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult CrearPresupuesto()
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

            List<Cliente> clientes = _clientesRepository.ObtenerClientes();
            ViewData["Clientes"] = clientes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre
            }).ToList();

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el formulario de alta de presupuesto.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearPresupuesto(PresupuestoViewModel viewModel)
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

            if (ModelState.IsValid)
            {
                if (viewModel.ClienteIdSeleccionado == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un cliente antes de agregar productos.");
                    viewModel.Clientes = _clientesRepository.ObtenerClientes();
                    viewModel.Productos = _productosRepository.ListarProductos();
                    return View(viewModel);
                }

                var cliente = _clientesRepository.ObtenerCliente(viewModel.ClienteIdSeleccionado);
                var nuevoPresupuesto = new Presupuestos
                {
                    Cliente = cliente,
                    FechaCreacion = DateTime.Now,
                    Detalle = new List<PresupuestoDetalle>()
                };

                foreach (var productoSeleccionado in viewModel.ProductosSeleccionados)
                {
                    if (productoSeleccionado.ProductoId > 0 && productoSeleccionado.Cantidad > 0)
                    {
                        var producto = _productosRepository.ObtenerProductoPorId(productoSeleccionado.ProductoId);
                        nuevoPresupuesto.Detalle.Add(new PresupuestoDetalle
                        {
                            Producto = producto,
                            Cantidad = productoSeleccionado.Cantidad
                        });
                    }
                }

                _presupuestosRepository.CrearNuevo(nuevoPresupuesto);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Clientes = _clientesRepository.ObtenerClientes();
            viewModel.Productos = _productosRepository.ListarProductos();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "Error al crear el presupuesto.";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult ModificarPresupuesto(int id)
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

            var presupuesto = _presupuestosRepository.ObtenerPresupuestoPorId(id);
            var clientes = _clientesRepository.ObtenerClientes();
            var productos = _productosRepository.ListarProductos();

            var viewModel = new ModificarPresupuestoViewModel
            {
                Clientes = clientes,
                Productos = productos,
                Presupuesto = presupuesto,
                ClienteIdSeleccionado = presupuesto.Cliente.Id
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "Error al cargar el formulario de modificación de presupuesto.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult ModificarPresupuesto(ModificarPresupuestoViewModel viewModel)
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
            {
                viewModel.Clientes = _clientesRepository.ObtenerClientes();
                viewModel.Productos = _productosRepository.ListarProductos();
                return View(viewModel);
            }

            if (viewModel.ClienteIdSeleccionado == 0)
            {
                ModelState.AddModelError("ClienteIdSeleccionado", "Debe seleccionar un cliente válido.");
                viewModel.Clientes = _clientesRepository.ObtenerClientes();
                viewModel.Productos = _productosRepository.ListarProductos();
                return View(viewModel);
            }

            var presupuestoExistente = _presupuestosRepository.ObtenerPresupuestoPorId(viewModel.Presupuesto.IdPresupuesto);
            if (presupuestoExistente == null)
            {
                return NotFound();
            }

            presupuestoExistente.Cliente = _clientesRepository.ObtenerCliente(viewModel.ClienteIdSeleccionado);
            presupuestoExistente.Detalle.Clear();

            foreach (var detalle in viewModel.Presupuesto.Detalle)
            {
                if (detalle.Producto?.IdProducto > 0 && detalle.Cantidad > 0)
                {
                    var producto = _productosRepository.ObtenerProductoPorId(detalle.Producto.IdProducto);
                    if (producto != null)
                    {
                        presupuestoExistente.Detalle.Add(new PresupuestoDetalle
                        {
                            Producto = producto,
                            Cantidad = detalle.Cantidad
                        });
                    }
                }
            }

            _presupuestosRepository.ModificarPresupuestoQ(presupuestoExistente);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ModelState.AddModelError("", "Error al guardar los cambios: " + ex.Message);
            viewModel.Clientes = _clientesRepository.ObtenerClientes();
            viewModel.Productos = _productosRepository.ListarProductos();
            return View(viewModel);
        }
    }

    [HttpGet]
    public IActionResult EliminarPresupuesto(int id)
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

            var presupuesto = _presupuestosRepository.ObtenerPresupuestoPorId(id);
            if (presupuesto == null)
            {
                return NotFound();
            }
            return View(presupuesto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "Error al intentar eliminar el presupuesto.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarPresupuestoConfirmado(int id)
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

            _presupuestosRepository.EliminarPresupuesto(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "Error al intentar eliminar el presupuesto.";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Index()
    {
        try
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
                return RedirectToAction("Index", "Login");

            ViewData["EsAdmin"] = HttpContext.Session.GetString("AccessLevel") == "Admin";
            return View(_presupuestosRepository.ListarPresupuestos());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            ViewBag.ErrorMessage = "No se pudo cargar el listado de presupuestos.";
            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
