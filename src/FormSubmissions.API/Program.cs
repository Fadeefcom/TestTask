using FluentValidation;
using FormSubmissions.API.Configuration;
using FormSubmissions.API.Filters;
using FormSubmissions.API.Validation;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers(o =>
{
    o.Filters.Add<FluentValidationActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("vite", p =>
    {
        p.WithOrigins("http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields =
        HttpLoggingFields.RequestMethod |
        HttpLoggingFields.RequestPath |
        HttpLoggingFields.RequestQuery |
        HttpLoggingFields.RequestHeaders |
        HttpLoggingFields.RequestBody |
        HttpLoggingFields.ResponseStatusCode |
        HttpLoggingFields.ResponseHeaders |
        HttpLoggingFields.ResponseBody;

    o.RequestBodyLogLimit = 64 * 1024;
    o.ResponseBodyLogLimit = 64 * 1024;

    o.MediaTypeOptions.AddText("application/json");
    o.MediaTypeOptions.AddText("text/plain");
});

builder.Services.AddFormSubmissions(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<CreateFormDefinitionRequestValidator>();

var app = builder.Build();

app.UseHttpLogging();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("vite");

app.MapControllers();

app.Run();
