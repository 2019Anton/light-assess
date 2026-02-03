using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var appConfigEndpoint = builder.Configuration["AzureAppConfig:Endpoint"];
var appConfigConnectionString = builder.Configuration["AzureAppConfig:ConnectionString"];

if (!string.IsNullOrWhiteSpace(appConfigConnectionString))
{
    // Local/dev fallback (for example from User Secrets or env var).
    builder.Configuration.AddAzureAppConfiguration(appConfigConnectionString);
}
else if (!string.IsNullOrWhiteSpace(appConfigEndpoint))
{
    // Preferred for Azure: Managed Identity / workload identity via DefaultAzureCredential.
    builder.Configuration.AddAzureAppConfiguration(options =>
        options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential()));
}
else
{
    throw new InvalidOperationException(
        "Azure App Configuration is not configured. Set AzureAppConfig:Endpoint or AzureAppConfig:ConnectionString.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
