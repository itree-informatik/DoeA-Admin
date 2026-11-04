// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoeA.Data.Models.DB;

/// <summary>EF-Core-Kontext der Applikation inkl. DataProtection-Schlüsselspeicher; OnModelCreating setzt alle von EF generierten Namen (Tabellen, Spalten, Keys, Indizes) für PostgreSQL auf lowercase.</summary>
public partial class DoeAContext : DbContext, IDataProtectionKeyContext
{
    /// <summary>Schema aller Applikationstabellen; auch von Komponenten ausserhalb von EF zu verwenden, die selbst SQL absetzen (Serilog-Sink).</summary>
    public const string Schema = "dbo";

    public DoeAContext()
    {
    }

    public DoeAContext(DbContextOptions<DoeAContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAmt> TAmt { get; set; }

    public virtual DbSet<TAnbieter> TAnbieter { get; set; }

    public virtual DbSet<TAusschreibung> TAusschreibung { get; set; }

    public virtual DbSet<TAusschreibungCpvNummer> TAusschreibungCpvNummer { get; set; }

    public virtual DbSet<TAusschreibungSprache> TAusschreibungSprache { get; set; }

    public virtual DbSet<TAusschreibungVerfahrensschritt> TAusschreibungVerfahrensschritt { get; set; }

    public virtual DbSet<TBerichtigung> TBerichtigung { get; set; }

    public virtual DbSet<TBkpNummer> TBkpNummer { get; set; }

    public virtual DbSet<TKonfiguration> TKonfiguration { get; set; }

    public virtual DbSet<TCpvNummer> TCpvNummer { get; set; }

    public virtual DbSet<TDepartement> TDepartement { get; set; }

    public virtual DbSet<TDokumentart> TDokumentart { get; set; }

    public virtual DbSet<TExportgruppe> TExportgruppe { get; set; }

    public virtual DbSet<TExportgruppeAmt> TExportgruppeAmt { get; set; }


    public virtual DbSet<TExportgruppeLeistungsart> TExportgruppeLeistungsart { get; set; }

    public virtual DbSet<TExportgruppeKanton> TExportgruppeKanton { get; set; }

    public virtual DbSet<TExportgruppeProjektart> TExportgruppeProjektart { get; set; }

    public virtual DbSet<TExportgruppeRessort> TExportgruppeRessort { get; set; }

    public virtual DbSet<TExportgruppeRolleInAusschreibung> TExportgruppeRolleInAusschreibung { get; set; }

    public virtual DbSet<TExportgruppeSachbearbeiter> TExportgruppeSachbearbeiter { get; set; }

    public virtual DbSet<TExportgruppeVerfahrensart> TExportgruppeVerfahrensart { get; set; }

    public virtual DbSet<TKanton> TKanton { get; set; }

    public virtual DbSet<TLand> TLand { get; set; }

    public virtual DbSet<TLeistungsart> TLeistungsart { get; set; }

    public virtual DbSet<TLog> TLog { get; set; }

    public virtual DbSet<TLos> TLos { get; set; }

    public virtual DbSet<TLosBkpNummer> TLosBkpNummer { get; set; }

    public virtual DbSet<TOfferte> TOfferte { get; set; }

    public virtual DbSet<TOfferteOffertezustand> TOfferteOffertezustand { get; set; }

    public virtual DbSet<TOffertezustand> TOffertezustand { get; set; }

    public virtual DbSet<TOrganisationszugehoerigkeit> TOrganisationszugehoerigkeit { get; set; }

    public virtual DbSet<TOrtschaft> TOrtschaft { get; set; }

    public virtual DbSet<TProjektart> TProjektart { get; set; }

    public virtual DbSet<TRelease> TRelease { get; set; }

    public virtual DbSet<TReleaseContent> TReleaseContent { get; set; }

    public virtual DbSet<TRessort> TRessort { get; set; }

    public virtual DbSet<TRolleInAusschreibung> TRolleInAusschreibung { get; set; }

    public virtual DbSet<TSachbearbeiter> TSachbearbeiter { get; set; }

    public virtual DbSet<TSprache> TSprache { get; set; }

    public virtual DbSet<TTranslation> TTranslation { get; set; }

    public virtual DbSet<TUser> TUser { get; set; }

    public virtual DbSet<TVerfahrensart> TVerfahrensart { get; set; }

    public virtual DbSet<TVerfahrensschritt> TVerfahrensschritt { get; set; }

    /// <summary>Schlüsselspeicher der ASP.NET Data Protection (Auth-Cookie), Tabelle dbo.t_dataprotectionkey.</summary>
    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        // Fremde Entity ohne [Table]-Attribut: Tabellenname muss vor der Lowercase-Schleife gesetzt werden,
        // sonst bleibt der PascalCase-Name stehen und die Abfragen laufen ins Leere.
        modelBuilder.Entity<DataProtectionKey>().ToTable("T_DataProtectionKey");

        // Alles lowercase für PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetSchema(entity.GetSchema()?.ToLower());
            entity.SetTableName(entity.GetTableName()?.ToLower());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName()?.ToLower());

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()?.ToLower());

            foreach (var key in entity.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName()?.ToLower());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()?.ToLower());
        }
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TAmt>(entity =>
        {
            entity.HasKey(e => e.AmtId).HasName("PK_Amt");

            entity.Property(e => e.AmtId).ValueGeneratedNever();

            entity.HasOne(d => d.AmtDep).WithMany(p => p.TAmt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Amt_Departement");
        });

        modelBuilder.Entity<TAnbieter>(entity =>
        {
            entity.HasKey(e => e.AnbId).HasName("PK_Anbieter");

            entity.Property(e => e.AnbId).ValueGeneratedNever();

            entity.HasOne(d => d.AnbOrt).WithMany(p => p.TAnbieter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Anbieter_Ortschaft");
        });

        modelBuilder.Entity<TAusschreibung>(entity =>
        {
            entity.HasKey(e => e.AusId).HasName("PK_Ausschreibung");

            entity.Property(e => e.AusId).ValueGeneratedNever();

            entity.HasOne(d => d.AusAmt).WithMany(p => p.TAusschreibung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ausschreibung_Amt");

            entity.HasOne(d => d.AusAus).WithMany(p => p.InverseAusAus).HasConstraintName("FK_Ausschreibung_Ausschreibung");

            entity.HasOne(d => d.AusVorankuendigung).WithMany(p => p.InverseAusVorankuendigung).HasConstraintName("FK_Ausschreibung_Vorankuendigung");

            entity.HasOne(d => d.AusRfi).WithMany(p => p.InverseAusRfi).HasConstraintName("FK_Ausschreibung_Rfi");

            entity.HasOne(d => d.AusLea).WithMany(p => p.TAusschreibung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ausschreibung_Leistungsart");

            entity.HasOne(d => d.AusPra).WithMany(p => p.TAusschreibung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ausschreibung_Projektart");

            entity.HasOne(d => d.AusRes).WithMany(p => p.TAusschreibung).HasConstraintName("FK_Ausschreibung_Ressort");

            entity.HasOne(d => d.AusRia).WithMany(p => p.TAusschreibung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ausschreibung_Rolle");

            entity.HasOne(d => d.AusSacBbl).WithMany(p => p.TAusschreibungAusSacBbl).HasConstraintName("FK_Ausschreibung_SachbearbeiterBBL");

            entity.HasOne(d => d.AusSacDoeA).WithMany(p => p.TAusschreibungAusSacDoeA).HasConstraintName("FK_Ausschreibung_SachbearbeiterDoeA");

            entity.HasOne(d => d.AusSacRd).WithMany(p => p.TAusschreibungAusSacRd).HasConstraintName("FK_Ausschreibung_SachbearbeiterRD");

            entity.HasOne(d => d.AusVea).WithMany(p => p.TAusschreibung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ausschreibung_Verfahrensart");
        });

        modelBuilder.Entity<TAusschreibungCpvNummer>(entity =>
        {
            entity.HasKey(e => e.AcnId).HasName("PK_AusschreibungCpvNummer");

            entity.Property(e => e.AcnId).ValueGeneratedNever();

            entity.HasOne(d => d.AcnAus).WithMany(p => p.TAusschreibungCpvNummer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungCpvNummer_Ausschreibung");

            entity.HasOne(d => d.AcnCpv).WithMany(p => p.TAusschreibungCpvNummer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungCpvNummer_CpvNummer");
        });

        modelBuilder.Entity<TAusschreibungSprache>(entity =>
        {
            entity.HasKey(e => e.AspId).HasName("PK_AusschreibungSprache");

            entity.Property(e => e.AspId).ValueGeneratedNever();

            entity.HasOne(d => d.AspAus).WithMany(p => p.TAusschreibungSprache)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungSprache_Ausschreibung");

            entity.HasOne(d => d.AspDoa).WithMany(p => p.TAusschreibungSprache)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungSprache_Dokumentart");

            entity.HasOne(d => d.AspSpr).WithMany(p => p.TAusschreibungSprache)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungSprache_Sprache");
        });

        modelBuilder.Entity<TAusschreibungVerfahrensschritt>(entity =>
        {
            entity.HasKey(e => e.AvsId).HasName("PK_AusschreibungVerfahrensschritt");

            entity.Property(e => e.AvsId).ValueGeneratedNever();

            entity.HasOne(d => d.AvsAus).WithMany(p => p.TAusschreibungVerfahrensschritt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungVerfahrensschritt_Ausschreibung");

            entity.HasOne(d => d.AvsVes).WithMany(p => p.TAusschreibungVerfahrensschritt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AusschreibungVerfahrensschritt_Verfahrensschritt");
        });

        modelBuilder.Entity<TBerichtigung>(entity =>
        {
            entity.HasKey(e => e.BerId).HasName("PK_Berichtigung");

            entity.Property(e => e.BerId).ValueGeneratedNever();

            entity.HasOne(d => d.BerAus).WithMany(p => p.TBerichtigung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Berichtigung_Ausschreibung");
        });

        modelBuilder.Entity<TBkpNummer>(entity =>
        {
            entity.HasKey(e => e.BkpId).HasName("PK_BkpNummer");

            entity.Property(e => e.BkpId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TKonfiguration>(entity =>
        {
            entity.HasKey(e => e.KonId).HasName("PK_Konfiguration");

            entity.Property(e => e.KonId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TCpvNummer>(entity =>
        {
            entity.HasKey(e => e.CpvId).HasName("PK_CpvNummer");

            entity.Property(e => e.CpvId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TDepartement>(entity =>
        {
            entity.HasKey(e => e.DepId).HasName("PK_Department");

            entity.Property(e => e.DepId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TDokumentart>(entity =>
        {
            entity.HasKey(e => e.DoaId).HasName("PK_Dokumentart");

            entity.Property(e => e.DoaId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TExportgruppe>(entity =>
        {
            entity.HasKey(e => e.ExpId).HasName("PK_Exportgruppe");

            entity.Property(e => e.ExpId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TExportgruppeAmt>(entity =>
        {
            entity.HasKey(e => e.ExaId).HasName("PK_ExportgruppeAmt");

            entity.Property(e => e.ExaId).ValueGeneratedNever();

            entity.HasOne(d => d.ExaAmt).WithMany(p => p.TExportgruppeAmt)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeAmt_Amt");

            entity.HasOne(d => d.ExaExp).WithMany(p => p.TExportgruppeAmt)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeAmt_Exportgruppe");
        });

        modelBuilder.Entity<TExportgruppeLeistungsart>(entity =>
        {
            entity.HasKey(e => e.ExlId).HasName("PK_ExportgruppeLeistungsart");

            entity.Property(e => e.ExlId).ValueGeneratedNever();

            entity.HasOne(d => d.ExlExp).WithMany(p => p.TExportgruppeLeistungsart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeLeistungsart_Exportgruppe");

            entity.HasOne(d => d.ExlLea).WithMany(p => p.TExportgruppeLeistungsart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeLeistungsart_Leistungsart");
        });

        modelBuilder.Entity<TExportgruppeKanton>(entity =>
        {
            entity.HasKey(e => e.EkaId).HasName("PK_ExportgruppeKanton");

            entity.Property(e => e.EkaId).ValueGeneratedNever();

            entity.HasOne(d => d.EkaExp).WithMany(p => p.TExportgruppeKanton)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeKanton_Exportgruppe");

            entity.HasOne(d => d.EkaKan).WithMany(p => p.TExportgruppeKanton)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeKanton_Kanton");
        });

        modelBuilder.Entity<TExportgruppeProjektart>(entity =>
        {
            entity.HasKey(e => e.EprId).HasName("PK_ExportgruppeProjektart");

            entity.Property(e => e.EprId).ValueGeneratedNever();

            entity.HasOne(d => d.EprExp).WithMany(p => p.TExportgruppeProjektart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeProjektart_Exportgruppe");

            entity.HasOne(d => d.EprPra).WithMany(p => p.TExportgruppeProjektart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeProjektart_Projektart");
        });

        modelBuilder.Entity<TExportgruppeRessort>(entity =>
        {
            entity.HasKey(e => e.EreId).HasName("PK_ExportgruppeRessort");

            entity.Property(e => e.EreId).ValueGeneratedNever();

            entity.HasOne(d => d.EreExp).WithMany(p => p.TExportgruppeRessort)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeRessort_Exportgruppe");

            entity.HasOne(d => d.EreRes).WithMany(p => p.TExportgruppeRessort)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeRessort_Ressort");
        });

        modelBuilder.Entity<TExportgruppeRolleInAusschreibung>(entity =>
        {
            entity.HasKey(e => e.ExrId).HasName("PK_ExportgruppeRolleInAusschreibung");

            entity.Property(e => e.ExrId).ValueGeneratedNever();

            entity.HasOne(d => d.ExrExp).WithMany(p => p.TExportgruppeRolleInAusschreibung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeRolleInAusschreibung_Exportgruppe");

            entity.HasOne(d => d.ExrRia).WithMany(p => p.TExportgruppeRolleInAusschreibung)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeRolleInAusschreibung_RolleInAusschreibung");
        });

        modelBuilder.Entity<TExportgruppeSachbearbeiter>(entity =>
        {
            entity.HasKey(e => e.ExsId).HasName("PK_ExportgruppeSachbearbeiter");

            entity.Property(e => e.ExsId).ValueGeneratedNever();

            entity.HasOne(d => d.ExsExp).WithMany(p => p.TExportgruppeSachbearbeiter)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeSachbearbeiter_Exportgruppe");

            entity.HasOne(d => d.ExsSac).WithMany(p => p.TExportgruppeSachbearbeiter)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeSachbearbeiter_Sachbearbeiter");
        });

        modelBuilder.Entity<TExportgruppeVerfahrensart>(entity =>
        {
            entity.HasKey(e => e.EvaId).HasName("PK_ExportgruppeVerfahrensart");

            entity.Property(e => e.EvaId).ValueGeneratedNever();

            entity.HasOne(d => d.EvaExp).WithMany(p => p.TExportgruppeVerfahrensart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeVerfahrensart_Exportgruppe");

            entity.HasOne(d => d.EvaVea).WithMany(p => p.TExportgruppeVerfahrensart)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ExportgruppeVerfahrensart_Verfahrensart");
        });

        modelBuilder.Entity<TKanton>(entity =>
        {
            entity.HasKey(e => e.KanId).HasName("PK_Kanton");

            entity.Property(e => e.KanId).ValueGeneratedNever();

            entity.HasOne(d => d.KanSpr).WithMany(p => p.TKanton)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kanton_Sprache");
        });

        modelBuilder.Entity<TLand>(entity =>
        {
            entity.HasKey(e => e.LanId).HasName("PK_Land");

            entity.Property(e => e.LanId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TLeistungsart>(entity =>
        {
            entity.HasKey(e => e.LeaId).HasName("PK_Leistungsart");

            entity.Property(e => e.LeaId).ValueGeneratedNever();

            entity.HasOne(d => d.LeaPra).WithMany(p => p.TLeistungsart)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Leistungsart_Projektart");
        });

        modelBuilder.Entity<TLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Log");
        });

        modelBuilder.Entity<TLos>(entity =>
        {
            entity.HasKey(e => e.LosId).HasName("PK_Los");

            entity.Property(e => e.LosId).ValueGeneratedNever();

            entity.HasOne(d => d.LosAus).WithMany(p => p.TLos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Los_Ausschreibung");
        });

        modelBuilder.Entity<TLosBkpNummer>(entity =>
        {
            entity.HasKey(e => e.LbnId).HasName("PK_LosBkpNummer");

            entity.Property(e => e.LbnId).ValueGeneratedNever();

            entity.HasOne(d => d.LbnBkp).WithMany(p => p.TLosBkpNummer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LosBkpNummer_BkpNummer");

            entity.HasOne(d => d.LbnLos).WithMany(p => p.TLosBkpNummer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LosBkpNummer_Los");
        });

        modelBuilder.Entity<TOfferte>(entity =>
        {
            entity.Property(e => e.OffId).ValueGeneratedNever();

            entity.HasOne(d => d.OffAnb).WithMany(p => p.TOfferte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Offerte_Anbieter");

            entity.HasOne(d => d.OffLos).WithMany(p => p.TOfferte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Offerte_Los");
        });

        modelBuilder.Entity<TOfferteOffertezustand>(entity =>
        {
            entity.HasKey(e => e.OozId).HasName("PK_OfferteOffertezustand");

            entity.Property(e => e.OozId).ValueGeneratedNever();

            entity.HasOne(d => d.OozOff).WithMany(p => p.TOfferteOffertezustand)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OfferteOffertezustand_Offerte");

            entity.HasOne(d => d.OozOfz).WithMany(p => p.TOfferteOffertezustand)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OfferteOffertezustand_Offertezustand");
        });

        modelBuilder.Entity<TOffertezustand>(entity =>
        {
            entity.HasKey(e => e.OfzId).HasName("PK_Offertezustand");

            entity.Property(e => e.OfzId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TOrganisationszugehoerigkeit>(entity =>
        {
            entity.HasKey(e => e.OrgId).HasName("PK_Organisationszugehoerigkeit");

            entity.Property(e => e.OrgId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TOrtschaft>(entity =>
        {
            entity.HasKey(e => e.OrtId).HasName("PK_Ortschaft");

            entity.Property(e => e.OrtId).ValueGeneratedNever();

            entity.HasOne(d => d.OrtKan).WithMany(p => p.TOrtschaft).HasConstraintName("FK_Ortschaft_Kanton");

            entity.HasOne(d => d.OrtLan).WithMany(p => p.TOrtschaft)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ortschaft_Land");
        });

        modelBuilder.Entity<TProjektart>(entity =>
        {
            entity.HasKey(e => e.PraId).HasName("PK_Projektart");

            entity.Property(e => e.PraId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TRelease>(entity =>
        {
            entity.HasKey(e => e.RelId).HasName("PK_Release");

            entity.Property(e => e.RelId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TReleaseContent>(entity =>
        {
            entity.HasKey(e => e.RecId).HasName("PK_ReleaseContent");

            entity.Property(e => e.RecId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TRessort>(entity =>
        {
            entity.HasKey(e => e.ResId).HasName("PK_Ressort");

            entity.Property(e => e.ResId).ValueGeneratedNever();

            entity.HasOne(d => d.ResAmt).WithMany(p => p.TRessort)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ressort_Amt");
        });

        modelBuilder.Entity<TRolleInAusschreibung>(entity =>
        {
            entity.HasKey(e => e.RiaId).HasName("PK_RolleInAusschreibung");

            entity.Property(e => e.RiaId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TSachbearbeiter>(entity =>
        {
            entity.HasKey(e => e.SacId).HasName("PK_Sachbearbeiter");

            entity.Property(e => e.SacId).ValueGeneratedNever();

            entity.HasOne(d => d.SacOrg).WithMany(p => p.TSachbearbeiter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sachbearbeiter_Organisationszugehoerigkeit");
        });

        modelBuilder.Entity<TSprache>(entity =>
        {
            entity.HasKey(e => e.SprId).HasName("PK_Sprache");

            entity.Property(e => e.SprId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TTranslation>(entity =>
        {
            entity.HasKey(e => e.TrlId).HasName("PK_Translation");

            entity.Property(e => e.TrlId).ValueGeneratedNever();

            entity.HasOne(d => d.TrlRel).WithMany(p => p.TTranslation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Translation_Release");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.UsrId).HasName("PK_User");

            entity.Property(e => e.UsrId).ValueGeneratedNever();

            entity.HasIndex(e => e.UsrExtId).IsUnique();
        });

        modelBuilder.Entity<TVerfahrensart>(entity =>
        {
            entity.HasKey(e => e.VeaId).HasName("PK_Verfahrensart");

            entity.Property(e => e.VeaId).ValueGeneratedNever();
        });

        modelBuilder.Entity<TVerfahrensschritt>(entity =>
        {
            entity.Property(e => e.VesId).ValueGeneratedNever();
        });
    }
}
