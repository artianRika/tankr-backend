using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TankR.Data;
using TankR.Mappings;
using TankR.Repos.Implementations;
using TankR.Repos.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IMapper>(sp =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<UserProfile>();
    });

    return config.CreateMapper();
});


builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserAddressRepo, UserAddressRepo>();
builder.Services.AddScoped<IStationRepo, StationRepo>();
builder.Services.AddScoped<IStationAddressRepo, StationAddressRepo>();
builder.Services.AddScoped<IFuelTypeRepo, FuelTypeRepo>();


// builder.Services.AddScoped<IFuelTypeRepo, FuelTypeRepo>();
// builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();

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