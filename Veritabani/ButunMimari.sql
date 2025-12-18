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
    telefon VARCHAR(15),
    rol_id INT NOT NULL,
    kayit_tarihi DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rol_id) REFERENCES Rol(rol_id)

     CONSTRAINT chk_telefon
        CHECK (telefon IS NULL OR telefon REGEXP '^[0-9]{10,15}$')
        
	 CONSTRAINT chk_email_format
        CHECK (email LIKE '%@%.%')

) ENGINE=InnoDB;


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




-- Views 


CREATE VIEW vw_ilan_detay AS
SELECT 
    i.ilan_id,
    m.marka_adi,
    s.seri_adi,
    mo.model_adi,
    a.yil,
    a.kilometre,
    i.fiyat
FROM Ilan i
JOIN Arac a ON i.arac_id = a.arac_id
JOIN Model mo ON a.model_id = mo.model_id
JOIN Seri s ON mo.seri_id = s.seri_id
JOIN Marka m ON s.marka_id = m.marka_id;

CREATE VIEW vw_kullanici_tahminleri AS
SELECT 
    k.ad_soyad,
    ft.tahmin_edilen_fiyat,
    ft.tahmin_tarihi
FROM Fiyat_Tahmin ft
JOIN Kullanicilar k ON ft.kullanici_id = k.kullanici_id;

CREATE VIEW vw_arac_teknik_bilgi AS
SELECT 
    a.arac_id,
    a.yil,
    a.motor_hacmi,
    a.motor_gucu,
    a.yakit_tipi,
    a.vites_tipi
FROM Arac a;

CREATE VIEW vw_ml_model_performans AS
SELECT 
    model_adi,
    versiyon,
    dogruluk_orani
FROM ML_Model;

CREATE VIEW vw_son_tahminler AS
SELECT 
    tahmin_id,
    tahmin_edilen_fiyat,
    tahmin_tarihi
FROM Fiyat_Tahmin
ORDER BY tahmin_tarihi DESC;


-- Functions 

DELIMITER //

CREATE FUNCTION Ortalama_Tahmin_Fiyati ()
RETURNS DECIMAL(12,2)
DETERMINISTIC
BEGIN
    DECLARE ort_fiyat DECIMAL(12,2);

    SELECT AVG(tahmin_edilen_fiyat)
    INTO ort_fiyat
    FROM Fiyat_Tahmin;

    RETURN ort_fiyat;
END //

DELIMITER ;


DELIMITER //

CREATE FUNCTION Kullanici_Tahmin_Sayisi (p_kullanici_id INT)
RETURNS INT
DETERMINISTIC
BEGIN
    DECLARE toplam INT;

    SELECT COUNT(*)
    INTO toplam
    FROM Fiyat_Tahmin
    WHERE kullanici_id = p_kullanici_id;

    RETURN toplam;
END //

DELIMITER ;


-- StoredProcedures

DELIMITER //

CREATE PROCEDURE Fiyat_Tahmin_Ekle (
    IN p_kullanici_id INT,
    IN p_arac_id INT,
    IN p_tahmin_fiyat DECIMAL(12,2)
)
BEGIN
    INSERT INTO Fiyat_Tahmin
    (kullanici_id, arac_id, tahmin_edilen_fiyat)
    VALUES
    (p_kullanici_id, p_arac_id, p_tahmin_fiyat);
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE Kullanici_Tahmin_Gecmisi (
    IN p_kullanici_id INT
)
BEGIN
    SELECT 
        ft.tahmin_id,
        ft.tahmin_edilen_fiyat,
        ft.tahmin_tarihi
    FROM Fiyat_Tahmin ft
    WHERE ft.kullanici_id = p_kullanici_id
    ORDER BY ft.tahmin_tarihi DESC;
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE Ilan_Ekle (
    IN p_arac_id INT,
    IN p_kullanici_id INT,
    IN p_fiyat DECIMAL(12,2),
    IN p_takasa_uygun BOOLEAN,
    IN p_kimden ENUM('Sahibinden','Galeriden')
)
BEGIN
    INSERT INTO Ilan
    (arac_id, kullanici_id, fiyat, ilan_tarihi, takasa_uygun, kimden)
    VALUES
    (p_arac_id, p_kullanici_id, p_fiyat, CURDATE(), p_takasa_uygun, p_kimden);
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE Kullanici_Ilanlari (
    IN p_kullanici_id INT
)
BEGIN
    SELECT 
        i.ilan_id,
        i.fiyat,
        i.ilan_tarihi,
        i.takasa_uygun,
        i.kimden
    FROM Ilan i
    WHERE i.kullanici_id = p_kullanici_id
    ORDER BY i.ilan_tarihi DESC;
END //

DELIMITER ;

-- Masking 

CREATE VIEW vw_kullanici_email_maskeli AS
SELECT 
    kullanici_id,
    ad_soyad,
    CONCAT(LEFT(email,3),'***@',SUBSTRING_INDEX(email,'@',-1)) AS email
FROM Kullanicilar;


CREATE VIEW vw_kullanici_telefon_maskeli AS
SELECT
    kullanici_id,
    ad_soyad,
    CONCAT(LEFT(telefon,3),'****',RIGHT(telefon,2)) AS telefon
FROM Kullanicilar;


