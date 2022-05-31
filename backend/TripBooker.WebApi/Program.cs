using AspNetCore.Authentication.Basic;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TripBooker.WebApi.Hubs;
using TripBooker.WebApi.Infrastructure;
using TripBooker.WebApi.Repositories;
using TripBooker.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserRepository, InMemoryUserRepository>();

builder.Services.AddAuthentication(BasicDefaults.AuthenticationScheme)
    .AddBasic<BasicUserValidationService>(options =>
    {
        options.Realm = "TripBooker";
        options.IgnoreAuthenticationIfAllowAnonymous = true;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(x => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services
    .AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddControllers()
    .AddNewtonsoftJson(x =>
    {
        x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        x.SerializerSettings.Converters.Add(new StringEnumConverter());
    });


builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<PurchasedOfferNotificationHub>("/purchasedNotification")
    .AllowAnonymous();
app.MapHub<DestinationStatisticsHub>("/destinationsHub")
    .AllowAnonymous();
app.MapHub<HotelStatisticsHub>("/hotelsHub")
    .AllowAnonymous();
app.MapHub<OfferUpdatesHub>("/offerUpdatedNotifications")
    .AllowAnonymous();


app.MapControllers();

app.Run();