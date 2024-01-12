using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using PixelBoard.MainServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IColorDbService, RedisColorDbService>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // All details will be fetched automatically according to OIDC
        options.Authority = $"{builder.Configuration["Keycloak:Url"]}/realms/{builder.Configuration["Keycloak:Realm"]}";
        options.RequireHttpsMetadata = false;

        // this is to help student debugging their client implementations
        options.IncludeErrorDetails = true;
        options.Events = new JwtBearerEvents()
        {
            OnChallenge = ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ctx.Response.ContentType = "application/json";

                if (ctx.AuthenticateFailure is SecurityTokenExpiredException authenticationException)
                {
                    ctx.Response.Headers["x-token-expired"] = authenticationException.Expires.ToString("o");
                    ctx.ErrorDescription = $"Your JWT has expired on {authenticationException.Expires.ToString("o")}";
                }

                var newResponse = JsonSerializer.Serialize(
                    new
                    {
                        error = ctx.Error ?? "invalid_token",
                        error_description = ctx.ErrorDescription ?? "Missing a valid JWT"
                    }
                );
                return ctx.Response.WriteAsync(newResponse);
            }
        };
    });
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pixel Board API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "Pixel Board API V1");
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
