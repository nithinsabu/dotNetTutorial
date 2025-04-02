// problems faced in docker: frontend always refers from the system of the user. hence expose the backend on the server and that is the api address. in this case api url is localhost 8080 with port 8080:8080.
// backend was not connecting because of environment variable overriding. worked when manually hardcoded the db connection url.
//backend image build was caching previous build. my bad i was editing the ./out/appsettings.json .
//final: frontend : localhost:8080 where backend is hosted in my own system. backend: mongodb://mongo:27017 in the network of the container.
//docker run --network application --name backend -p 8080:8080 --network-alias backend dotnet-app
// docker run --network application --name mongo mongo
//docker run -p 5173:80 --name frontend react-app
using WebAPI.Services;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);
// builder.Configuration
//     .SetBasePath(Directory.GetCurrentDirectory())
//     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//     .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("MongoDB");

// if (string.IsNullOrEmpty(connectionString))
// {
//     throw new ArgumentNullException("MongoDB connection string is missing.");
// }
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .WithOrigins("http://localhost:5173");
    });
});
Console.WriteLine(connectionString);
builder.Services.AddSingleton<MongoClient>(sp => 
    new MongoClient(connectionString));

builder.Services.AddSingleton<IMongoDatabase>(sp => 
    sp.GetRequiredService<MongoClient>().GetDatabase("UserIp"));

builder.Services.AddScoped<ImageService>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
builder.Services.AddControllers(); 

var app = builder.Build();

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

// app.UseHttpsRedirection();
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// app.MapSwagger().RequireAuthorization();
app.MapControllers(); 

app.MapGet("/", () => {Console.WriteLine("Hell"); return "Hello, World!";});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.UseCors("AllowSpecificOrigin");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}