using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using ApiGateway.Middlewares;
using ApiGateway.Aggregators;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddOcelot()
    .AddCacheManager(x => x.WithDictionaryHandle())
    .AddSingletonDefinedAggregator<ProductAggregator>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.Map("/swagger.json", b =>
{
    b.Run(async x => {
        var json = File.ReadAllText("swagger.json");
        await x.Response.WriteAsync(json);
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger.json", "swagger");
});

app.UseOcelot().Wait();

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
