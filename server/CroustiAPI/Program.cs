using System.Text;
using CroustiAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GameStateService>();
builder.Services.AddScoped<QrCodeService>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<TokenProviderService>();
builder.Services.AddSingleton<DuckDnsService>();
builder.Services.AddSingleton<LetsEncryptService>();

builder.Services.AddHostedService<HostedService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidAudience = builder.Configuration.GetValue<string>("CroustiTabletop:Tokens:Audience"),
        ValidIssuer = builder.Configuration.GetValue<string>("CroustiTabletop:Tokens:Issuer"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("CroustiTabletop:Tokens:Secret")))
    };

    options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              var accessToken = context.Request.Query["access_token"];

              // If the request is for our hub...
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(PlayerHub.HubUrl))
              {
                  // Read the token out of the query string
                  context.Token = accessToken;
              }
              return Task.CompletedTask;
          }
      };
});

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .SetIsOriginAllowed(_ => true));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PlayerHub>(PlayerHub.HubUrl);
app.MapControllers();

var duckDnsService = app.Services.GetService<DuckDnsService>();
var letsEncryptService = app.Services.GetService<LetsEncryptService>();

if(!File.Exists(letsEncryptService.GetCertLocation()))
{
    await duckDnsService.UpdateDns();
    await letsEncryptService.GenerateCert();
}

app.Run();
