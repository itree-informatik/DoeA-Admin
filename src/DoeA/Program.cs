// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

global using DoeA.Data.Helper;
using DoeA.Components;
using DoeA.Components.Functions;
using DoeA.Configuration;
using DoeA.Data.Constants;
using DoeA.Data.Mapper;
using DoeA.Data.Models.DB;
using DoeA.Endpoints;
using DoeA.Interfaces;
using DoeA.Interfaces.Auswertungen;
using DoeA.Interfaces.Einstellungen;
using DoeA.Interfaces.Stammdaten;
using DoeA.Services;
using DoeA.Services.Auswertungen;
using DoeA.Services.Einstellungen;
using DoeA.Services.Stammdaten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DoeA.Services.Ausschreibungen;
using ItreeMud.Extensions;
using ItreeMud.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("APP");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidDataException("ConnectionStrings:APP ist nicht konfiguriert");
}

builder.Services.AddDbContextFactory<DoeAContext>(opt =>

    opt.UseNpgsql(connectionString)
);

SerilogConfig.AddSerilogServices(builder.Services, builder.Configuration);

// Authentifizierung über eIAM (OpenIdConnect:Authority|ClientId|ClientSecret, lokal via User Secrets)
AuthConfig.AddAuthServices(builder.Services, builder.Configuration, builder.Environment);
builder.AddSicherheitsHeader();

builder.Services.AddScoped<IAktuellerBenutzer, AktuellerBenutzer>();
builder.Services.AddScoped<IBenutzerService, BenutzerService>();
builder.Services.AddScoped<IAmtService, AmtService>();
builder.Services.AddScoped<IAnbieterService, AnbieterService>();
builder.Services.AddScoped<IBkpNummerService, BkpNummerService>();
builder.Services.AddScoped<ICpvNummerService, CpvNummerService>();
builder.Services.AddScoped<IDepartementService, DepartementService>();
builder.Services.AddScoped<IDokumentartService, DokumentartService>();
builder.Services.AddScoped<IKantonService, KantonService>();
builder.Services.AddScoped<ILandService, LandService>();
builder.Services.AddScoped<ILeistungsartService, LeistungsartService>();
builder.Services.AddScoped<INavigationService, NavigationService>();
builder.Services.AddScoped<IOffertezustandService, OffertezustandService>();
builder.Services.AddScoped<IOrganisationszugehoerigkeitService, OrganisationszugehoerigkeitService>();
builder.Services.AddScoped<IOrtschaftService, OrtschaftService>();
builder.Services.AddScoped<IProjektartService, ProjektartService>();
builder.Services.AddScoped<IRessortService, RessortService>();
builder.Services.AddScoped<IRolleInAusschreibungService, RolleInAusschreibungService>();
builder.Services.AddScoped<ISachbearbeiterService, SachbearbeiterService>();
builder.Services.AddScoped<ISpracheService, SpracheService>();
builder.Services.AddScoped<IVerfahrensartService, VerfahrensartService>();
builder.Services.AddScoped<IVerfahrensschrittService, VerfahrensschrittService>();
builder.Services.AddScoped<IAusschreibungService, AusschreibungService>();
builder.Services.AddScoped<IErrorHandlerService, ErrorHandlerService>();
builder.Services.AddScoped<IExportToExcelService, ExportToExcelService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IKonfigurationService, KonfigurationService>();
builder.Services.AddScoped<IAuswertungService, AuswertungService>();
builder.Services.AddScoped<IExportgruppeService, ExportgruppeService>();
builder.Services.AddScoped<IBaseFunctionService, BaseFunctionService>();
builder.Services.AddScoped<IGemBoxDocumentService, GemBoxDocumentService>();
builder.Services.AddScoped<IAusschreibungVerfahrensschrittService, AusschreibungVerfahrensschrittService>();
builder.Services.AddScoped<ILosService, LosService>();
builder.Services.AddScoped<IOfferteService, OfferteService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<AusschreibungSucheState>();
builder.Services.AddScoped<AusschreibungEditState>();
builder.Services.AddScoped<AusschreibungHerkunftState>();
builder.Services.AddScoped<AusschreibungVorlageState>();

builder.Services.AddSingleton<ITranslationProvider, DoeATranslationProvider>();
builder.Services.AddSingleton<IDownloadStorageService, DownloadStorageService>();
// MudBlazor-Dienste, ItreeMudLocalizer (MUD_-Schlüssel) sowie LanguageService und ConfirmService pro Circuit
builder.Services.AddItreeMud();
SprachHelper.KonfiguriereSprachen();

builder.Services.RegistriereMapper();

// Liveness-Check ohne eigene Prüfungen, siehe HealthEndpoints
builder.Services.AddHealthChecks();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = builder.Environment.IsDevelopment(); });

var app = builder.Build();

// Übersetzungen beim Start einmalig laden
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DoeAContext>>();
    Task.Run(() => TranslationHelper.LoadTranslations(dbFactory)).Wait();
}

// Logtabelle bei jedem Start um abgelaufene Einträge bereinigen (Aufbewahrungsdauer aus der
// Konfiguration, NaturalId "AnzahlTageLog"). Ein Fehler dabei darf den Start nicht verhindern.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var geloescht = await scope.ServiceProvider.GetRequiredService<ILogService>().Delete();
        Serilog.Log.Information("Logbereinigung beim Start: {Anzahl} Einträge entfernt.", geloescht);
    }
    catch (Exception ex)
    {
        Serilog.Log.Error(ex, "Logbereinigung beim Start fehlgeschlagen.");
    }
}

app.UseSicherheitsHeader();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/download/{id}", (string id, [FromServices] IDownloadStorageService downloadStorage) =>
{
    var file = downloadStorage.RetrieveFile(id);
    if (file == null)
    {
        return Results.NotFound();
    }
    var (content, fileName) = file.Value;
    return Results.File(content, "application/octet-stream", fileName);
}).RequireAuthorization(AuthKonstanten.PolicyAutorisiert);

app.MapAnmeldung();
app.MapSitzung();
app.MapHealth();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("de-CH");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("de-CH");

CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberGroupSeparator = "'";
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.NumberGroupSeparator = "'";
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalDigits = 1;
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.NumberDecimalDigits = 1;
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.PercentDecimalDigits = 1;
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.PercentDecimalDigits = 1;
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.PercentNegativePattern = 1;
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.PercentPositivePattern = 1;
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture.NumberFormat.CurrencyGroupSeparator = "'";
CultureInfo.DefaultThreadCurrentUICulture.NumberFormat.CurrencyGroupSeparator = "'";

app.Run();
