CREATE TABLE Rol (
    rol_id INT AUTO_INCREMENT PRIMARY KEY,
    rol_adi VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- KULLANICILAR
CREATE TABLE Kullanicilar (
    kullanici_id INT AUTO_INCREMENT PRIMARY KEY,
    ad_soyad VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    sifre_hash VARCHAR(255) NOT NULL,
    rol_id INT NOT NULL,
    kayit_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rol_id) REFERENCES Rol(rol_id)
) ENGINE=InnoDB;
ALTER TABLE Kullanicilar
ADD CONSTRAINT chk_telefon
CHECK (telefon REGEXP '^[0-9]{10,15}$');

-- MARKA
CREATE TABLE Marka (
    marka_id INT AUTO_INCREMENT PRIMARY KEY,
    marka_adi VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- SERI
CREATE TABLE Seri (
    seri_id INT AUTO_INCREMENT PRIMARY KEY,
    marka_id INT NOT NULL,
    seri_adi VARCHAR(100) NOT NULL,
    FOREIGN KEY (marka_id) REFERENCES Marka(marka_id)
) ENGINE=InnoDB;

-- MODEL
CREATE TABLE Model (
    model_id INT AUTO_INCREMENT PRIMARY KEY,
    seri_id INT NOT NULL,
    model_adi VARCHAR(100) NOT NULL,
    FOREIGN KEY (seri_id) REFERENCES Seri(seri_id)
) ENGINE=InnoDB;

-- ARAC (TEKNIK OZELLIKLER)
CREATE TABLE Arac (
    arac_id INT AUTO_INCREMENT PRIMARY KEY,
    model_id INT NOT NULL,
    yil INT CHECK (yil >= 1990),
    kilometre INT CHECK (kilometre >= 0),
    vites_tipi ENUM('Manuel','Otomatik'),
    yakit_tipi ENUM('Benzin','Dizel','Elektrik','Hibrit'),
    kasa_tipi VARCHAR(50),
    renk VARCHAR(50),
    motor_hacmi INT CHECK (motor_hacmi > 0),
    motor_gucu INT CHECK (motor_gucu > 0),
    cekis_tipi ENUM('Onden','Arkadan','DortCeker'),
    arac_durumu ENUM('Sifir','IkinciEl'),
    ort_yakit_tuketimi DECIMAL(4,2),
    yakit_deposu INT,
    boya_degisen VARCHAR(100),
    FOREIGN KEY (model_id) REFERENCES Model(model_id)
) ENGINE=InnoDB;

-- ILAN
CREATE TABLE Ilan (
    ilan_id INT AUTO_INCREMENT PRIMARY KEY,
    arac_id INT NOT NULL,
    kullanici_id INT NOT NULL,
    fiyat DECIMAL(12,2) CHECK (fiyat > 0),
    ilan_tarihi DATE,
    takasa_uygun BOOLEAN,
    kimden ENUM('Sahibinden','Galeriden'),
    FOREIGN KEY (arac_id) REFERENCES Arac(arac_id),
    FOREIGN KEY (kullanici_id) REFERENCES Kullanicilar(kullanici_id)
) ENGINE=InnoDB;

CREATE TABLE Kullanici_Islem_Log (
    islem_id INT AUTO_INCREMENT PRIMARY KEY,
    kullanici_id INT NOT NULL,
    islem_tipi VARCHAR(100) NOT NULL,
    islem_detayi TEXT,
    ip_adresi VARCHAR(45),
    islem_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (kullanici_id) REFERENCES Kullanicilar(kullanici_id)
) ENGINE=InnoDB;

CREATE TABLE Fiyat_Tahmin (
    tahmin_id INT AUTO_INCREMENT PRIMARY KEY,
    kullanici_id INT NOT NULL,
    arac_id INT NOT NULL,
    tahmin_edilen_fiyat DECIMAL(12,2) CHECK (tahmin_edilen_fiyat > 0),
    tahmin_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (kullanici_id) REFERENCES Kullanicilar(kullanici_id),
    FOREIGN KEY (arac_id) REFERENCES Arac(arac_id)
) ENGINE=InnoDB;

CREATE TABLE ML_Model (
    ml_model_id INT AUTO_INCREMENT PRIMARY KEY,
    model_adi VARCHAR(100) NOT NULL,
    versiyon VARCHAR(50),
    dogruluk_orani DECIMAL(5,2) CHECK (dogruluk_orani BETWEEN 0 AND 100),
    eklenme_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE Tahmin_Log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    tahmin_id INT NOT NULL,
    ml_model_id INT NOT NULL,
    calisma_suresi_ms INT CHECK (calisma_suresi_ms >= 0),
    FOREIGN KEY (tahmin_id) REFERENCES Fiyat_Tahmin(tahmin_id),
    FOREIGN KEY (ml_model_id) REFERENCES ML_Model(ml_model_id)
) ENGINE=InnoDB;

CREATE INDEX idx_ilan_fiyat ON Ilan(fiyat);
CREATE INDEX idx_arac_model ON Arac(model_id);
CREATE INDEX idx_tahmin_kullanici ON Fiyat_Tahmin(kullanici_id);
