/*using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Aqui agregamos los servicios requeridos

//el dbcontext a utilizar
//builder.services.addsingleton<pgsqldbcontext>();

//los repositorios
//builder.services.addscoped<iresumenrepository, resumenrepository>();
//builder.services.addscoped<ipaisrepository, paisrepository>();
//builder.services.addscoped<irazarepository, razarepository>();
//builder.services.addscoped<icaracteristicarepository, caracteristicarepository>();

////aqui agregamos los servicios asociados para cada ruta
//builder.services.addscoped<resumenservice>();
//builder.services.addscoped<paisservice>();
//builder.services.addscoped<razaservice>();
//builder.services.addscoped<caracteristicaservice>();

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
        Title = "Compuestos Quimicos - Versión en PostgreSQL",
        Description = "API para la gestión de Información sobre compuestos químicos"
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