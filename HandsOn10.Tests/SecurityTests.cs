using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HandsOn10.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HandsOn10.Tests;
public sealed class SecurityTests {

    private sealed class Factory : WebApplicationFactory<Program>
    {


        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            var config = Services.GetRequiredService<IConfiguration>();

            Console.WriteLine(
                $"JWT configurado nos testes: " +
                $"{!string.IsNullOrWhiteSpace(config["Jwt:Key"])}");

            Console.WriteLine(
                $"JWT Issuer: {config["Jwt:Issuer"]}");

            Console.WriteLine(
                $"JWT Audience: {config["Jwt:Audience"]}");
        }

        private readonly string _databaseName = $"HandsOn10_Test_{Guid.NewGuid():N}";
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Key"] =
                            "OnlyForIntegrationTestsKey12345678901234567890",

                        ["Jwt:Issuer"] = "HandsOn10.Api",

                        ["Jwt:Audience"] = "HandsOn10.Client",

                        ["Lab:EnableVulnerableEndpoint"] = "true"
                    });
            });

            builder.ConfigureTestServices(services =>
            {
                // Remover as opções do DbContext original
                services.RemoveAll<DbContextOptions<AppDbContext>>();

                // Remover a configuração do fornecedor anterior
                services.RemoveAll<
                    IDbContextOptionsConfiguration<AppDbContext>>();

                // Configurar exclusivamente InMemory
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));

                // Configurar a validação JWT nos testes
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        var testKey =
                            "OnlyForIntegrationTestsKey12345678901234567890";

                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = "HandsOn10.Api",

                                ValidateAudience = true,
                                ValidAudience = "HandsOn10.Client",

                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.FromSeconds(30),

                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(testKey))
                            };
                    });
            });

        }
    }


    private static async Task<string> Token(HttpClient client,string username) {
  var response=await client.PostAsJsonAsync("/api/auth/login",new {username,password="Demo123!ChangeMe"});
  response.EnsureSuccessStatusCode();
  using var json=JsonDocument.Parse(await response.Content.ReadAsStringAsync());
  return json.RootElement.GetProperty("access_token").GetString()!;
 }
 [Fact] public async Task Health_IsPublic() { using var f=new Factory();using var c=f.CreateClient();Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/api/health")).StatusCode); }
 [Fact] public async Task Requests_WithoutToken_Return401() { using var f=new Factory();using var c=f.CreateClient();Assert.Equal(HttpStatusCode.Unauthorized,(await c.GetAsync("/api/pedidos/101")).StatusCode); }
 [Fact] public async Task WrongPassword_Return401() { using var f=new Factory();using var c=f.CreateClient();Assert.Equal(HttpStatusCode.Unauthorized,(await c.PostAsJsonAsync("/api/auth/login",new {username="ana",password="errada"})).StatusCode); }
 [Fact] public async Task Owner_CanReadOwnRequest() { using var f=new Factory();using var c=f.CreateClient();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",await Token(c,"ana"));Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/api/pedidos/101")).StatusCode); }
 [Fact] public async Task Bola_Attempt_IsForbidden() { using var f=new Factory();using var c=f.CreateClient();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",await Token(c,"ana"));Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/pedidos/102")).StatusCode); }
 [Fact] public async Task VulnerableLab_IllustratesBola() { using var f=new Factory();using var c=f.CreateClient();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",await Token(c,"ana"));Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/api/pedidos/lab/vulneravel/102")).StatusCode); }
 [Fact] public async Task NonAdmin_CannotListAll() { using var f=new Factory();using var c=f.CreateClient();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",await Token(c,"ana"));Assert.Equal(HttpStatusCode.Forbidden,(await c.GetAsync("/api/pedidos/admin/todos")).StatusCode); }
 [Fact] public async Task Admin_CanListAll() { using var f=new Factory();using var c=f.CreateClient();c.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",await Token(c,"admin"));Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/api/pedidos/admin/todos")).StatusCode); }
}
