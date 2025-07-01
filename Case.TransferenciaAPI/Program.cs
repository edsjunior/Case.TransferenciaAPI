using Case.TransferenciaAPI.Data;
using Case.TransferenciaAPI.Interfaces;
using Case.TransferenciaAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Controllers
builder.Services.AddControllers();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Banco de dados em memória
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseInMemoryDatabase("DbCase"));

//Injeção de dependências
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransferenciaService, TransferenciaService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

