using Microsoft.Extensions.Hosting;
using SanityTests.Models;
using Services;
using System.Net;
using System.Net.Http.Headers;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<HttpService>();

var host = builder.Build();

var userToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI2NDY2NzNjYWI0NTkxNTA4ZTE0MzlkMTgiLCJQZXJtaXNzaW9ucyI6IkFETUlOLFJFQUQsV1JJVEUiLCJuYmYiOjE3Mzg0NDY4MjgsImV4cCI6MTczODQ0NzcyOCwiaWF0IjoxNzM4NDQ2ODI4LCJpc3MiOiJib290Y29tLmNvLnVrIiwiYXVkIjoiQk9PVENPTV9IT01FIn0.ohICndXWKoMAAgeu4Y10S4JJFivmqV8Cw1fnYJHK00-FO6AsGrNgtK92TdvbEcAUq4Iku_rqrs55lV3EcwaAPne0286Pfyf1Sx6fmP4ApVyYDoupc4T9tjkPxTK-Sfd5TKCwwS2V9vLUvAdg8TQsXpszLLXcvodBEd2mLASw8dXcQZppu6iilZY4HWrnwJq-ldejTb89npDd_tK45j_LWpZ4XGC-NaPWsrLZOaDxtB0GupkNXR1ZPs25cY-sLmBlwaxiXSdNbW1gyfYNKuVnvdNFw7yTkDV3BSYEl6pBnmHvuLSC8zwbJjaHq-lBvPFaDJ3nhBRGvFGZf1v0GpjgAA";
var refreshToken = "QmvJLCgkQDqpRWvubtHPEcdsR6IHgH6Xs1ZyKDG8iY71HfXcBpim0HHXBZCpq+Hl5ybDEuc7JGBQW9ZPyklDsA==";
var deviceId = Guid.Parse("b86cde51-6973-424c-aae8-4160fe922086");

await PerformRetryCheck();

await host.RunAsync();

async Task<AppSyncMapping> PerformRetryCheck()
{

    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    HttpService _httpService = provider.GetRequiredService<HttpService>();

    // https://bootcomhomesync.azurewebsites.net
    var response = await _httpService.CreateBuilder(new Uri($"https://localhost:7000/api/DataSync/Collect"), HttpMethod.Post)
                    .WithFormContent(new()
                    {
                    { "AppName", "BOOTCOM_HOME" }
                    })
                    .OnStatus(System.Net.HttpStatusCode.Unauthorized, (request) => CollectRefreshToken(request))
                    .WithRetry(3)
                    .SendAsync<AppSyncMapping>();

    if (response is null) throw new NullReferenceException("The request failed and returned no response");

    return response.Result;
}

async Task CollectRefreshToken(HttpRequestMessage request)
{

    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    HttpService _httpService = provider.GetRequiredService<HttpService>();

    var refreshTokenHttpBuilder = _httpService.CreateBuilder(new Uri("https://bootcomidentity.azurewebsites.net/AuthenticationV2/refresh-token"), HttpMethod.Post)
        .WithHeader("Authorization", $"bearer {userToken}")
        .WithFormContent(new()
        {
                    { "deviceId", deviceId.ToString() },
                    { "refreshToken", refreshToken }
        });

    var refreshTokenResponse = await refreshTokenHttpBuilder
        .SendAsync<Dictionary<string, string>>();

    if (!refreshTokenResponse.Success)
    {
        return;
    }

    userToken = refreshTokenResponse.Result!["JwtToken"];
    refreshToken = refreshTokenResponse.Result["RefreshToken"];

    request.Headers.Authorization = new AuthenticationHeaderValue("bearer", userToken);

}