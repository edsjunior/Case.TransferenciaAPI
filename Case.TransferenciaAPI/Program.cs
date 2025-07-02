using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.Interfaces;
using Case.TransferenciaAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "API de Transfer�ncias",
		Version = "v1",
		Description = "API REST para cadastro de clientes, transfer�ncias banc�rias e consulta de hist�rico.",
		Contact = new Microsoft.OpenApi.Models.OpenApiContact
		{
			Name = "Esdras Marcelino",
		}
	});
});

//Banco de dados em mem�ria
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseInMemoryDatabase("DbCase"));

//Inje��o de depend�ncias
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	context.Database.EnsureCreated();
	DbInit.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.DocumentTitle = "API de Transfer�ncias";
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
	});
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
