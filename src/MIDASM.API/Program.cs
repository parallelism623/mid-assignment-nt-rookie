using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MIDASM.API;
using MIDASM.API.Middlewares;
using MIDASM.Application.Commons.Options;
using QuestPDF.Infrastructure;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureDependencyLayers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
QuestPDF.Settings.License = LicenseType.Community;
var corsSection = builder.Configuration.GetSection("Cors");
var policyName = corsSection.GetValue<string>("PolicyName");
var origins = corsSection.GetSection("AllowedOrigins").Get<string[]>();
var allowCreds = corsSection.GetValue<bool>("AllowCredentials");
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(policyName!, p =>
    {
        p.WithOrigins(origins!)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .WithExposedHeaders("Token-Expired");
        if (allowCreds)
            p.AllowCredentials();
        else
            p.DisallowCredentials();
    });
});
builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    var jwtTokenOptions = new JwtTokenOptions();
    builder.Configuration.GetRequiredSection(nameof(JwtTokenOptions)).Bind(jwtTokenOptions);
    var rsa = RSA.Create();
    rsa.ImportRSAPublicKey(Convert.FromBase64String(jwtTokenOptions.PublicKey), out _);

    options.RequireHttpsMetadata = jwtTokenOptions.RequireHttpsMetadata;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = jwtTokenOptions.ValidateIssuer,
        ValidateAudience = jwtTokenOptions.ValidateAudience,
        ValidIssuer = jwtTokenOptions.Issuer,
        ValidAudience = jwtTokenOptions.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new RsaSecurityKey(rsa),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {

            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                var refreshTokenHeader = context.Request.Path.Value?.Contains("/token-refresh") ?? false;
                if (!refreshTokenHeader)
                {
                    context.Response?.Headers?.Append("Token-Expired", "true");
                }
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseExceptionHandler((_) => { });
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitializeDatabaseAsync();
}

app.UseCors(policyName!);
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExecutionContextMiddleware>();
app.MapControllers();

await app.RunAsync();