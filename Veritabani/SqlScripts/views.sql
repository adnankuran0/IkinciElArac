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