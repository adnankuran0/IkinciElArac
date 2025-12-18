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