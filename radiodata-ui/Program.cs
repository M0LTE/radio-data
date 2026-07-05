using radiodata_ui;
using ukrepeaterlib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bounded-timeout HTTP client for the ETCC upstream so we fail fast instead of hanging
// for the HttpClient default of 100 seconds when it is unreachable.
builder.Services.AddHttpClient("etcc", client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Caching repository (singleton) that keeps serving the last-good repeater list through
// upstream outages. Set ETCC_CACHE_PATH to a writable file to persist it across restarts.
builder.Services.AddSingleton(sp => new EtccRepository(
    ct => new EtccApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("etcc")).GetAll(ct),
    Environment.GetEnvironmentVariable("ETCC_CACHE_PATH"),
    sp.GetRequiredService<ILogger<EtccRepository>>()));

builder.Services.AddTransient<EtccDataService>();
builder.Services.AddHostedService<EtccRefreshService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Surface upstream-data outages as a clean 503 (with Retry-After) instead of a 500.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (EtccDataUnavailableException)
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        context.Response.Headers.RetryAfter = "300";
        await context.Response.WriteAsync("Upstream ETCC repeater data is temporarily unavailable. Please try again shortly.");
    }
});

app.MapRazorPages();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
