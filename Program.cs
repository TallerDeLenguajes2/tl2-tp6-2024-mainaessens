var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClientesRepository,ClientesRepository>();
builder.Services.AddSingleton<IPresupuestoRepository, PresupuestosRepository>();
builder.Services.AddSingleton<IProductoRepository, ProductoRepository>();

var cadenaDeConexion = builder.Configuration.GetConnectionString("SqliteConexion")!.ToString(); 
builder.Services.AddSingleton(cadenaDeConexion);
//builder.Services.AddScoped<IUserRepository, UserRepository>();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
