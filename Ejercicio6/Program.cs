using Ejercicio6;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDb, Db>(); // Registramos la implementación de IDb y Db.

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ejercicio 6 de ASP.NET",
        Description = "# Web API creada con ASP.NET Core, MySQL y Dapper 🚀" +
                      "\n\n" +
                      "## Alumno\n" +
                      "Contiene la información de los alumnos. Un alumno en la base de datos es la unión de la tabla persona y la tabla alumno. La tabla persona contiene los datos que son comúnes para ambos tipos de personas. Como nombres y apellidos." +
                      "\n Permite:" +
                      "\n- ✏️ Crear alumno" +
                      "\n- 🔎 Ver alumnos" +
                      "\n- 🔄 Actualizar alumno" +
                      "\n- 🗑️ Eliminar alumno" +
                      "\n\n" +
                      "## Instructor\n" +
                      "Contiene la información de los instructores. Un instructor en la base de datos es la unión de la tabla persona y la tabla instructor. La tabla persona contiene los datos que son comúnes para ambos tipos de personas. Como fecha de nacimiento y tipo de rol." +
                      "\n Permite:" +
                      "\n- ✏️ Crear instructor" +
                      "\n- 🔎 Ver instructores" +
                      "\n- 🔄 Actualizar instructor" +
                      "\n- 🗑️ Eliminar instructor" +
                      "\n\n\n" +
                      "*Developed with ❤️ by Luis Ronquillo* 🫠😬🥴",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "📪 Contacto(próximamente)",
            Url = new Uri("https://example.com/contact")
        }
    });  
    
    c.EnableAnnotations();
});

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

app.Run();