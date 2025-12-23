using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace TermProject.Models;

public partial class ProjeContext : DbContext
{
    public ProjeContext()
    {
    }

    public ProjeContext(DbContextOptions<ProjeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Arac> Aracs { get; set; }

    public virtual DbSet<FiyatTahmin> FiyatTahmins { get; set; }

    public virtual DbSet<Ilan> Ilans { get; set; }

    public virtual DbSet<KullaniciIslemLog> KullaniciIslemLogs { get; set; }

    public virtual DbSet<Kullanicilar> Kullanicilars { get; set; }

    public virtual DbSet<Marka> Markas { get; set; }

    public virtual DbSet<MlModel> MlModels { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Seri> Seris { get; set; }

    public virtual DbSet<TahminLog> TahminLogs { get; set; }

    public virtual DbSet<VwAracTeknikBilgi> VwAracTeknikBilgis { get; set; }

    public virtual DbSet<VwIlanDetay> VwIlanDetays { get; set; }

    public virtual DbSet<VwKullaniciEmailMaskeli> VwKullaniciEmailMaskelis { get; set; }

    public virtual DbSet<VwKullaniciTahminleri> VwKullaniciTahminleris { get; set; }

    public virtual DbSet<VwKullaniciTelefonMaskeli> VwKullaniciTelefonMaskelis { get; set; }

    public virtual DbSet<VwMlModelPerforman> VwMlModelPerformans { get; set; }

    public virtual DbSet<VwSonTahminler> VwSonTahminlers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3308;database=arackiralama;user=yazilimci;password=123456;sslmode=None;allowpublickeyretrieval=True", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Arac>(entity =>
        {
            entity.HasKey(e => e.AracId).HasName("PRIMARY");

            entity.ToTable("arac");

            entity.HasIndex(e => e.ModelId, "idx_arac_model");

            entity.Property(e => e.AracId).HasColumnName("arac_id");
            entity.Property(e => e.AracDurumu)
                .HasColumnType("enum('Sifir','IkinciEl')")
                .HasColumnName("arac_durumu");
            entity.Property(e => e.BoyaDegisen)
                .HasMaxLength(100)
                .HasColumnName("boya_degisen");
            entity.Property(e => e.CekisTipi)
                .HasColumnType("enum('Onden','Arkadan','DortCeker')")
                .HasColumnName("cekis_tipi");
            entity.Property(e => e.KasaTipi)
                .HasMaxLength(50)
                .HasColumnName("kasa_tipi");
            entity.Property(e => e.Kilometre).HasColumnName("kilometre");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.MotorGucu).HasColumnName("motor_gucu");
            entity.Property(e => e.MotorHacmi).HasColumnName("motor_hacmi");
            entity.Property(e => e.OrtYakitTuketimi)
                .HasPrecision(4, 2)
                .HasColumnName("ort_yakit_tuketimi");
            entity.Property(e => e.Renk)
                .HasMaxLength(50)
                .HasColumnName("renk");
            entity.Property(e => e.VitesTipi)
                .HasColumnType("enum('Manuel','Otomatik')")
                .HasColumnName("vites_tipi");
            entity.Property(e => e.YakitDeposu).HasColumnName("yakit_deposu");
            entity.Property(e => e.YakitTipi)
                .HasColumnType("enum('Benzin','Dizel','Elektrik','Hibrit')")
                .HasColumnName("yakit_tipi");
            entity.Property(e => e.Yil).HasColumnName("yil");

            entity.HasOne(d => d.Model).WithMany(p => p.Aracs)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("arac_ibfk_1");
        });

        modelBuilder.Entity<FiyatTahmin>(entity =>
        {
            entity.HasKey(e => e.TahminId).HasName("PRIMARY");

            entity.ToTable("fiyat_tahmin");

            entity.HasIndex(e => e.AracId, "arac_id");

            entity.HasIndex(e => e.KullaniciId, "idx_tahmin_kullanici");

            entity.Property(e => e.TahminId).HasColumnName("tahmin_id");
            entity.Property(e => e.AracId).HasColumnName("arac_id");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
            entity.Property(e => e.TahminEdilenFiyat)
                .HasPrecision(12, 2)
                .HasColumnName("tahmin_edilen_fiyat");
            entity.Property(e => e.TahminTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("tahmin_tarihi");

            entity.HasOne(d => d.Arac).WithMany(p => p.FiyatTahmins)
                .HasForeignKey(d => d.AracId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fiyat_tahmin_ibfk_2");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.FiyatTahmins)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fiyat_tahmin_ibfk_1");
        });

        modelBuilder.Entity<Ilan>(entity =>
        {
            entity.HasKey(e => e.IlanId).HasName("PRIMARY");

            entity.ToTable("ilan");

            entity.HasIndex(e => e.AracId, "arac_id");

            entity.HasIndex(e => e.Fiyat, "idx_ilan_fiyat");

            entity.HasIndex(e => e.KullaniciId, "kullanici_id");

            entity.Property(e => e.IlanId).HasColumnName("ilan_id");
            entity.Property(e => e.AracId).HasColumnName("arac_id");
            entity.Property(e => e.Fiyat)
                .HasPrecision(12, 2)
                .HasColumnName("fiyat");
            entity.Property(e => e.IlanTarihi).HasColumnName("ilan_tarihi");
            entity.Property(e => e.Kimden)
                .HasColumnType("enum('Sahibinden','Galeriden')")
                .HasColumnName("kimden");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
            entity.Property(e => e.TakasaUygun).HasColumnName("takasa_uygun");

            entity.HasOne(d => d.Arac).WithMany(p => p.Ilans)
                .HasForeignKey(d => d.AracId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ilan_ibfk_1");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.Ilans)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ilan_ibfk_2");
        });

        modelBuilder.Entity<KullaniciIslemLog>(entity =>
        {
            entity.HasKey(e => e.IslemId).HasName("PRIMARY");

            entity.ToTable("kullanici_islem_log");

            entity.HasIndex(e => e.KullaniciId, "kullanici_id");

            entity.Property(e => e.IslemId).HasColumnName("islem_id");
            entity.Property(e => e.IpAdresi)
                .HasMaxLength(45)
                .HasColumnName("ip_adresi");
            entity.Property(e => e.IslemDetayi)
                .HasColumnType("text")
                .HasColumnName("islem_detayi");
            entity.Property(e => e.IslemTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("islem_tarihi");
            entity.Property(e => e.IslemTipi)
                .HasMaxLength(100)
                .HasColumnName("islem_tipi");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");

            entity.HasOne(d => d.Kullanici).WithMany(p => p.KullaniciIslemLogs)
                .HasForeignKey(d => d.KullaniciId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kullanici_islem_log_ibfk_1");
        });

        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciId).HasName("PRIMARY");

            entity.ToTable("kullanicilar");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.RolId, "rol_id");

            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
            entity.Property(e => e.Ad)
                .HasMaxLength(50)
                .HasColumnName("ad");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.KayitTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("kayit_tarihi");
            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.SifreHash)
                .HasMaxLength(255)
                .HasColumnName("sifre_hash");
            entity.Property(e => e.Soyad)
                .HasMaxLength(50)
                .HasColumnName("soyad");
            entity.Property(e => e.Telefon)
                .HasMaxLength(15)
                .HasColumnName("telefon");

            entity.HasOne(d => d.Rol).WithMany(p => p.Kullanicilars)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("kullanicilar_ibfk_1");
        });

        modelBuilder.Entity<Marka>(entity =>
        {
            entity.HasKey(e => e.MarkaId).HasName("PRIMARY");

            entity.ToTable("marka");

            entity.HasIndex(e => e.MarkaAdi, "marka_adi").IsUnique();

            entity.Property(e => e.MarkaId).HasColumnName("marka_id");
            entity.Property(e => e.MarkaAdi)
                .HasMaxLength(100)
                .HasColumnName("marka_adi");
        });

        modelBuilder.Entity<MlModel>(entity =>
        {
            entity.HasKey(e => e.MlModelId).HasName("PRIMARY");

            entity.ToTable("ml_model");

            entity.Property(e => e.MlModelId).HasColumnName("ml_model_id");
            entity.Property(e => e.DogrulukOrani)
                .HasPrecision(5, 2)
                .HasColumnName("dogruluk_orani");
            entity.Property(e => e.EklenmeTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("eklenme_tarihi");
            entity.Property(e => e.ModelAdi)
                .HasMaxLength(100)
                .HasColumnName("model_adi");
            entity.Property(e => e.Versiyon)
                .HasMaxLength(50)
                .HasColumnName("versiyon");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PRIMARY");

            entity.ToTable("model");

            entity.HasIndex(e => e.SeriId, "seri_id");

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.ModelAdi)
                .HasMaxLength(100)
                .HasColumnName("model_adi");
            entity.Property(e => e.SeriId).HasColumnName("seri_id");

            entity.HasOne(d => d.Seri).WithMany(p => p.Models)
                .HasForeignKey(d => d.SeriId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("model_ibfk_1");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.HasIndex(e => e.RolAdi, "rol_adi").IsUnique();

            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.RolAdi)
                .HasMaxLength(50)
                .HasColumnName("rol_adi");
        });

        modelBuilder.Entity<Seri>(entity =>
        {
            entity.HasKey(e => e.SeriId).HasName("PRIMARY");

            entity.ToTable("seri");

            entity.HasIndex(e => e.MarkaId, "marka_id");

            entity.Property(e => e.SeriId).HasColumnName("seri_id");
            entity.Property(e => e.MarkaId).HasColumnName("marka_id");
            entity.Property(e => e.SeriAdi)
                .HasMaxLength(100)
                .HasColumnName("seri_adi");

            entity.HasOne(d => d.Marka).WithMany(p => p.Seris)
                .HasForeignKey(d => d.MarkaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seri_ibfk_1");
        });

        modelBuilder.Entity<TahminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PRIMARY");

            entity.ToTable("tahmin_log");

            entity.HasIndex(e => e.MlModelId, "ml_model_id");

            entity.HasIndex(e => e.TahminId, "tahmin_id");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.CalismaSuresiMs).HasColumnName("calisma_suresi_ms");
            entity.Property(e => e.MlModelId).HasColumnName("ml_model_id");
            entity.Property(e => e.TahminId).HasColumnName("tahmin_id");

            entity.HasOne(d => d.MlModel).WithMany(p => p.TahminLogs)
                .HasForeignKey(d => d.MlModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tahmin_log_ibfk_2");

            entity.HasOne(d => d.Tahmin).WithMany(p => p.TahminLogs)
                .HasForeignKey(d => d.TahminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tahmin_log_ibfk_1");
        });

        modelBuilder.Entity<VwAracTeknikBilgi>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_arac_teknik_bilgi");

            entity.Property(e => e.AracId).HasColumnName("arac_id");
            entity.Property(e => e.MotorGucu).HasColumnName("motor_gucu");
            entity.Property(e => e.MotorHacmi).HasColumnName("motor_hacmi");
            entity.Property(e => e.VitesTipi)
                .HasColumnType("enum('Manuel','Otomatik')")
                .HasColumnName("vites_tipi");
            entity.Property(e => e.YakitTipi)
                .HasColumnType("enum('Benzin','Dizel','Elektrik','Hibrit')")
                .HasColumnName("yakit_tipi");
            entity.Property(e => e.Yil).HasColumnName("yil");
        });

        modelBuilder.Entity<VwIlanDetay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ilan_detay");

            entity.Property(e => e.Fiyat)
                .HasPrecision(12, 2)
                .HasColumnName("fiyat");
            entity.Property(e => e.IlanId).HasColumnName("ilan_id");
            entity.Property(e => e.Kilometre).HasColumnName("kilometre");
            entity.Property(e => e.MarkaAdi)
                .HasMaxLength(100)
                .HasColumnName("marka_adi");
            entity.Property(e => e.ModelAdi)
                .HasMaxLength(100)
                .HasColumnName("model_adi");
            entity.Property(e => e.SeriAdi)
                .HasMaxLength(100)
                .HasColumnName("seri_adi");
            entity.Property(e => e.Yil).HasColumnName("yil");
        });

        modelBuilder.Entity<VwKullaniciEmailMaskeli>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_kullanici_email_maskeli");

            entity.Property(e => e.AdSoyad)
                .HasMaxLength(101)
                .HasColumnName("ad_soyad");
            entity.Property(e => e.Email)
                .HasMaxLength(262)
                .HasColumnName("email");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
        });

        modelBuilder.Entity<VwKullaniciTahminleri>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_kullanici_tahminleri");

            entity.Property(e => e.AdSoyad)
                .HasMaxLength(101)
                .HasColumnName("ad_soyad");
            entity.Property(e => e.TahminEdilenFiyat)
                .HasPrecision(12, 2)
                .HasColumnName("tahmin_edilen_fiyat");
            entity.Property(e => e.TahminTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("tahmin_tarihi");
        });

        modelBuilder.Entity<VwKullaniciTelefonMaskeli>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_kullanici_telefon_maskeli");

            entity.Property(e => e.AdSoyad)
                .HasMaxLength(101)
                .HasColumnName("ad_soyad");
            entity.Property(e => e.KullaniciId).HasColumnName("kullanici_id");
            entity.Property(e => e.Telefon)
                .HasMaxLength(9)
                .HasColumnName("telefon");
        });

        modelBuilder.Entity<VwMlModelPerforman>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ml_model_performans");

            entity.Property(e => e.DogrulukOrani)
                .HasPrecision(5, 2)
                .HasColumnName("dogruluk_orani");
            entity.Property(e => e.ModelAdi)
                .HasMaxLength(100)
                .HasColumnName("model_adi");
            entity.Property(e => e.Versiyon)
                .HasMaxLength(50)
                .HasColumnName("versiyon");
        });

        modelBuilder.Entity<VwSonTahminler>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_son_tahminler");

            entity.Property(e => e.TahminEdilenFiyat)
                .HasPrecision(12, 2)
                .HasColumnName("tahmin_edilen_fiyat");
            entity.Property(e => e.TahminId).HasColumnName("tahmin_id");
            entity.Property(e => e.TahminTarihi)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("tahmin_tarihi");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
