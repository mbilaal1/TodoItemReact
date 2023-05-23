using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("TodoCorsPolicy", policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddTransient<ITodoRepository, TodoRepository>();
builder.Services.AddDbContext<TodoContext>((_, dbContextBuilder) =>
    dbContextBuilder.UseInMemoryDatabase("Todo"));
builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining(typeof(Program)));
builder.Services.AddScoped<DbInitializer>();

var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();
builder.Services.AddCors(options =>
{   
    var frontendURL = configuration.GetValue<string>("frontend_url");

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(frontendURL).AllowAnyMethod().AllowAnyHeader();
    });

});
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("TodoCorsPolicy");
app.UseAuthorization();
app.MapControllers();


await using (var scope = app.Services.CreateAsyncScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    dbInitializer.Seed(scope.ServiceProvider);
}

app.Run();

