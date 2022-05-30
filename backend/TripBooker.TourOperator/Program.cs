using TripBooker.TourOperator.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

CreateDbIfNotExists(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

static void CreateDbIfNotExists(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var TOContext = services.GetRequiredService<TourOperatorDbContext>();

        SqlDbInitializer.Initialize(TOContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}