using COMPUESTOS_API_CS_SQL.Contexts;
using COMPUESTOS_API_CS_SQL.Interfaces;
using COMPUESTOS_API_CS_SQL.Repositories;
using COMPUESTOS_API_CS_SQL.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


//El DBContext a utilizar
builder.Services.AddSingleton<PgsqlContext>();

//Los repositorios
builder.Services.AddScoped<IElementoRepository, ElementoRepository>();
builder.Services.AddScoped<ICompuestoRepository, CompuestoRepository>();
//builder.Services.AddScoped<IRazaRepository, RazaRepository>();
//builder.Services.AddScoped<ICaracteristicaRepository, CaracteristicaRepository>();

////Aqui agregamos los servicios asociados para cada ruta
builder.Services.AddScoped<ElementoService>();
builder.Services.AddScoped<CompuestoService>();


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "COMPUESTOS QUIMICOS - Versión en PostgreSQL",
        Description = "API para la gestión de Compuestos químicos"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Modificamos el encabezado de las peticiones para ocultar el web server utilizado
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Server", "CompuestosServer");
    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();