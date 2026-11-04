// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using System.Security.Claims;
using DoeA.Data.Constants;
using DoeA.Data.Enums;
using DoeA.Data.Helper;
using DoeA.Data.Mapper.Einstellungen;
using DoeA.Data.Models.DB;
using DoeA.Data.Models.DTO.Einstellungen;
using DoeA.Interfaces;
using DoeA.Interfaces.Einstellungen;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ILogger = Serilog.ILogger;

namespace DoeA.Services.Einstellungen;

/// <summary>
/// Benutzer aus dem eIAM: Abgleich beim Login, Statusabfragen für Cookie und Circuit sowie die Verwaltung auf der
/// Benutzerseite. Eigenständig statt BaseService, weil T_User keine Audit-Spalten hat.
/// </summary>
public class BenutzerService : IBenutzerService
{
    private readonly IAktuellerBenutzer _aktuellerBenutzer;
    private readonly IDbContextFactory<DoeAContext> _dbFactory;
    private readonly BenutzerMapper _benutzerMapper;
    private readonly ILogger _logger;

    public BenutzerService(IDbContextFactory<DoeAContext> dbFactory, BenutzerMapper benutzerMapper, ILogger logger, IAktuellerBenutzer aktuellerBenutzer)
    {
        _dbFactory = dbFactory;
        _benutzerMapper = benutzerMapper;
        _logger = logger;
        _aktuellerBenutzer = aktuellerBenutzer;
    }

    /// <summary>Gleicht den Benutzer beim Login mit T_User ab (Unbekannte inaktiv anlegen) und liefert die DoeA-Claims fürs Cookie.</summary>
    public async Task<IReadOnlyList<Claim>> SynchronisiereLogin(ClaimsPrincipal principal)
    {
        // Der Identifikator wird unverändert gespeichert: alle späteren Lookups (Cookie-Abgleich, Audit, Selbstsperre)
        // vergleichen mit dem rohen Claim aus dem Cookie.
        var extId = principal.FindFirst(AuthKonstanten.ClaimSub)?.Value;
        if (string.IsNullOrWhiteSpace(extId))
        {
            throw new InvalidOperationException($"{GeneralConstants.NullReference}?{AuthKonstanten.ClaimSub}");
        }

        var vorname = ClaimWert(principal, AuthKonstanten.ClaimVorname);
        var nachname = ClaimWert(principal, AuthKonstanten.ClaimNachname);
        var email = ClaimWert(principal, AuthKonstanten.ClaimEmail);

        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        var benutzer = await dbContext.TUser.SingleOrDefaultAsync(u => u.UsrExtId == extId);

        if (benutzer == null)
        {
            benutzer = new TUser
            {
                UsrId = Guid.NewGuid(),
                UsrExtId = extId,
                UsrVorname = vorname,
                UsrNachname = nachname,
                UsrEMail = email,
                UsrSprache = SprachHelper.AusClaim(ClaimWert(principal, AuthKonstanten.ClaimSprache)),
                UsrLastLogin = null,
                UsrAktiv = false
            };
            dbContext.TUser.Add(benutzer);

            try
            {
                await dbContext.SaveChangesAsync();
                _logger.Warning("Neuer Benutzer {ExtId} ({Email}) angelegt, wartet auf Freischaltung", extId, email);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                // Paralleler Erstlogin (z.B. zwei Tabs): der andere Request war schneller, dessen Zeile gilt.
                dbContext.Entry(benutzer).State = EntityState.Detached;
                benutzer = await dbContext.TUser.AsNoTracking().SingleAsync(u => u.UsrExtId == extId);
            }

            return ErzeugeClaims(Status(benutzer));
        }

        // Die Claims überschreiben die gespeicherten Personendaten; die Sprache bleibt Sache des Benutzers.
        var geaendert = UebernehmePersonendaten(benutzer, vorname, nachname, email);
        if (benutzer.UsrAktiv)
        {
            benutzer.UsrLastLogin = DateTime.Now.Ticks;
            geaendert = true;
        }

        if (geaendert)
        {
            await dbContext.SaveChangesAsync();
        }

        return ErzeugeClaims(Status(benutzer));
    }

    /// <summary>Aktueller DB-Stand (Aktiv, Sprache) zum Abgleich mit den Cookie-Claims; null, wenn der Benutzer nicht existiert.</summary>
    public async Task<BenutzerStatus?> HoleStatus(string extId)
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        return await dbContext.TUser.AsNoTracking()
            .Where(u => u.UsrExtId == extId)
            .Select(u => new BenutzerStatus(u.UsrId, u.UsrAktiv, u.UsrSprache))
            .SingleOrDefaultAsync();
    }

    /// <summary>Stempelt die letzte Anmeldung, wenn ein Benutzer mit bestehendem Cookie freigeschaltet wurde.</summary>
    public async Task MarkiereLogin(Guid id)
    {
        var jetzt = DateTime.Now.Ticks;
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        await dbContext.TUser
            .Where(u => u.UsrId == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.UsrLastLogin, jetzt));
    }

    /// <summary>DoeA-Claims (Sprache, Autorisierung) aus dem DB-Stand; bestimmt allein, welche Claims der Cookie-Abgleich ersetzt.</summary>
    public IReadOnlyList<Claim> ErzeugeClaims(BenutzerStatus status) =>
    [
        new Claim(AuthKonstanten.ClaimUsrSprache, status.Sprache),
        new Claim(AuthKonstanten.ClaimAutorisiert, status.Aktiv ? AuthKonstanten.ClaimWertTrue : AuthKonstanten.ClaimWertFalse)
    ];

    /// <summary>Benutzerliste nach vordefiniertem Filter und optionalem Suchtext (ExtId, Vorname, Nachname, E-Mail).</summary>
    public async Task<List<Benutzer>> Suche(EnumBenutzerFilter filter, string? suchtext)
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        var queryable = filter switch
        {
            EnumBenutzerFilter.Aktive => dbContext.TUser.Where(u => u.UsrAktiv),
            EnumBenutzerFilter.Inaktive => dbContext.TUser.Where(u => !u.UsrAktiv && u.UsrLastLogin != null),
            EnumBenutzerFilter.Neue => dbContext.TUser.Where(u => !u.UsrAktiv && u.UsrLastLogin == null),
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
        };

        if (!string.IsNullOrWhiteSpace(suchtext))
        {
            var suche = suchtext.Trim().ToLower();
            queryable = queryable.Where(u =>
                u.UsrExtId.ToLower().Contains(suche) ||
                u.UsrVorname.ToLower().Contains(suche) ||
                u.UsrNachname.ToLower().Contains(suche) ||
                u.UsrEMail.ToLower().Contains(suche));
        }

        var entities = await queryable.AsNoTracking()
            .OrderBy(u => u.UsrNachname)
            .ThenBy(u => u.UsrVorname)
            .ToListAsync();
        return entities.Select(_benutzerMapper.ZuModel).ToList();
    }

    /// <summary>Schaltet einen Benutzer frei oder deaktiviert ihn; den eigenen Status kann niemand ändern.</summary>
    public async Task SetzeAktiv(Guid id, bool aktiv)
    {
        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        var benutzer = await dbContext.TUser.SingleOrDefaultAsync(u => u.UsrId == id)
                       ?? throw new InvalidOperationException($"{GeneralConstants.NotFoundError}?{nameof(Benutzer)} {id}");

        // Der Circuit prüft die Freischaltung nur alle 5 Minuten. Wer in dieser Zeit deaktiviert wurde, darf weder sich
        // noch andere freischalten — darum zählt der DB-Stand des Aufrufers, nicht sein Claim.
        var aufruferAktiv = await dbContext.TUser.AnyAsync(u => u.UsrExtId == _aktuellerBenutzer.ExtId && u.UsrAktiv);

        // Den eigenen Status ändert niemand: Deaktivieren sperrt aus, Aktivieren höbe eine Sperre auf.
        if (!aufruferAktiv || benutzer.UsrExtId == _aktuellerBenutzer.ExtId)
        {
            throw new InvalidOperationException(GeneralConstants.ForbiddenError);
        }

        benutzer.UsrAktiv = aktiv;
        await dbContext.SaveChangesAsync();
        // Warning statt Information: T_User hat keine Audit-Spalten, und produktiv ist Warning das Mindest-Loglevel —
        // die Freischaltung wäre sonst nirgends nachvollziehbar.
        _logger.Warning("Benutzer {ExtId} durch {Admin} {Aktion}", benutzer.UsrExtId, _aktuellerBenutzer.ExtId, aktiv ? "aktiviert" : "deaktiviert");
    }

    /// <summary>Persistiert die vom Benutzer gewählte UI-Sprache.</summary>
    public async Task SetzeSprache(string extId, string sprache)
    {
        if (!SprachHelper.IstUnterstuetzt(sprache))
        {
            throw new InvalidOperationException($"{GeneralConstants.ValidationException}?{sprache}");
        }

        await using var dbContext = await _dbFactory.CreateDbContextAsync();
        await dbContext.TUser
            .Where(u => u.UsrExtId == extId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.UsrSprache, sprache));
    }

    private static string ClaimWert(ClaimsPrincipal principal, string claimTyp) => principal.FindFirst(claimTyp)?.Value?.Trim() ?? string.Empty;

    private static BenutzerStatus Status(TUser benutzer) => new(benutzer.UsrId, benutzer.UsrAktiv, benutzer.UsrSprache);

    private static bool UebernehmePersonendaten(TUser benutzer, string vorname, string nachname, string email)
    {
        var geaendert = false;
        if (benutzer.UsrVorname != vorname)
        {
            benutzer.UsrVorname = vorname;
            geaendert = true;
        }

        if (benutzer.UsrNachname != nachname)
        {
            benutzer.UsrNachname = nachname;
            geaendert = true;
        }

        if (benutzer.UsrEMail != email)
        {
            benutzer.UsrEMail = email;
            geaendert = true;
        }

        return geaendert;
    }
}
