using Presupuesto.Filters;
using Presupuesto.Infraestructure;
using Presupuesto.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLogging(loggin =>
{
    loggin.ClearProviders();
    loggin.SetMinimumLevel(LogLevel.Trace);
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//Registramos el filtro personalizado
builder.Services.AddScoped<GlobalExceptionFilter>();

//Se registro el filtro dentro del controlador
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

//Configuramos todos los servicios del middleaware
builder.Services.AddTransient<ITiposCuentasServices, TiposCuentasServices>();
builder.Services.AddTransient<IUsuariosServices, ServiciosUsuarios>();

builder.Services.AddMvc();

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
