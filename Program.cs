using AdminGateway.Managers.Abstract;
using AdminGateway.Managers.Concrete;
using AdminGateway.Middlewares;
using AdminGateway.Models;
using AdminGateway.Repositories.Abstract;
using AdminGateway.Repositories.Concrete;
using AdminGateway.Services;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddRequiredAppSettings();

// Add services to the container.

builder.Services.AddAuthentication(
    CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();
builder.Services.AddSingleton(sp =>
{
    return new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AdminAuth:JWT:SigningKey"])),
        SecurityAlgorithms.HmacSha256);
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddCustomSwagger(builder.Configuration);

builder.Services.AddTransient<IUserManager, UserManager>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddDbContexts(builder.Configuration);

builder.Services.AddServicesOptions(builder.Configuration);

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCertificateForwarding();
app.UseCustomSwagger(builder.Configuration);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("corsapp");

app.MapHealthChecks("/healthcheck");
app.MapControllers();


app.Run();
