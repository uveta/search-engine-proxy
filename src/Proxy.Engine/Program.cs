using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.Playwright;
using Proxy.Engine.Playwright;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProblemDetails(ConfigureProblemDetails);
builder.Services
    .AddControllers()
    // Adds MVC conventions to work better with the ProblemDetails middleware.
    .AddProblemDetailsConventions();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPlaywright();

var app = builder.Build();

app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapGet("google", async (string searchFor, IBrowser browser) =>
{
    var page = await browser.NewPageAsync();
    await page.GotoAsync("https://www.google.com/?hl=en");
    var acceptButton = page.Locator("button", new PageLocatorOptions { HasTextString = "Accept all" });
    if (acceptButton is null)
    {
        throw new InvalidOperationException("Could not find accept button");
    }
    await acceptButton.ClickAsync();
    var searchInput = page.Locator("input[title=\"Search\"]");
    if (searchInput is null)
    {
        throw new InvalidOperationException("Could not find search input");
    }
    await searchInput.TypeAsync(searchFor);
    var searchButton = page.Locator("input", new PageLocatorOptions { HasTextString = "Google Search" }).Last;
    if (searchButton is null)
    {
        throw new InvalidOperationException("Could not find search button");
    }
    await searchButton.ClickAsync();
    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    var buffer = await page.ScreenshotAsync();
    return Results.File(buffer, "image/png");
    // var html = await page.ContentAsync();
    // return Results.Ok(new { Content = html });
});

app.Run();

void ConfigureProblemDetails(ProblemDetailsOptions options)
{
    // Only include exception details in a development environment. There's really no nee
    // to set this as it's the default behavior. It's just included here for completeness :)
    options.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment();

    // Custom mapping function for FluentValidation's ValidationException.
    // options.MapFluentValidationException();

    // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
    // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
    // options.Rethrow<NotSupportedException>();

    // This will map NotImplementedException to the 501 Not Implemented status code.
    options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

    // This will map HttpRequestException to the 503 Service Unavailable status code.
    options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

    // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
    // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
}
