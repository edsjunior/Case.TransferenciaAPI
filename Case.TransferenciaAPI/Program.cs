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
		Title = "API de Transferências",
		Version = "v1",
		Description = "API REST para cadastro de clientes, transferências bancárias e consulta de histórico.",
		Contact = new Microsoft.OpenApi.Models.OpenApiContact
		{
			Name = "Esdras Marcelino",
		}
	});
});

//Banco de dados em memória
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseInMemoryDatabase("DbCase"));

//Injeção de dependências
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
		options.DocumentTitle = "API de Transferências";
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
	});
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
