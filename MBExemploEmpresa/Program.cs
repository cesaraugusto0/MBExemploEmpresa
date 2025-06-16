using MBExemploEmpresa.Components;
using MBExemploEmpresa.Servico;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Registro da string de conexao com SQL

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Inje��o de dependencia - registrando os servi�os

builder.Services.AddScoped(provider=> new DepartamentoService(connectionString));
builder.Services.AddScoped(provider => new CargoService(connectionString));
builder.Services.AddScoped(provider => new FuncionarioService(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
