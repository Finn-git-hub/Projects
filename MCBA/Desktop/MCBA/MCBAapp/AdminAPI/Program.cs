using MCBAapp.Data;
using MCBAapp.Models.DataManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AdminApiContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("AdminApiContext"));
    },
    ServiceLifetime.Scoped
    );

builder.Services.AddScoped<CustomerManager>();

builder.Services.AddControllers();

// Add services to the container.
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

app.UseAuthorization();
app.MapControllers();

app.Run();
