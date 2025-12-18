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