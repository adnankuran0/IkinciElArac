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


