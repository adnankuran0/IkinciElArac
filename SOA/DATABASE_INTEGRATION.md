# SOA Yapısı - Veritabanı Entegrasyonu

## 🎯 Ne Yaptık?

SOAP servisini **mock data** yerine **gerçek MySQL veritabanı** ile çalışacak şekilde güncelledik.

---

## 📂 Eklenen/Güncellenen Dosyalar

### 1. **database.js** (YENİ)
- MySQL bağlantı havuzu (connection pool) oluşturuldu
- ASP.NET'teki appsettings.json ile aynı bağlantı bilgileri kullanıldı
- Host: `localhost:3308`
- Database: `arackiralama`
- User: `yazilimci`

### 2. **package.json** (GÜNCELLENDİ)
- `mysql2` kütüphanesi eklendi
- Promise desteği için mysql2/promise kullanıldı

### 3. **server.js** (GÜNCELLENDİ)

#### SOAP Metodları:

**GetVehicleInfo (Araç Bilgisi Sorgulama)**
- ❌ Önce: Mock data döndürüyordu
- ✅ Şimdi: Veritabanından JOIN sorgusuyla gerçek veri çekiyor
- Query: `Ilan → Arac → Model → Seri → Marka`
- View benzeri karmaşık JOIN kullanıyor

**CalculatePrice (Fiyat Tahmini)**
- ✅ gRPC üzerinden ML modeline gönderiliyor
- ✅ ExchangeRate API'den döviz kurları alınıyor
- ✅ **YENİ:** Tahmin sonucu `Fiyat_Tahmin` tablosuna kaydediliyor

#### REST API:
- **POST /api/predict** - Tahmin yap ve veritabanına kaydet
- **GET /api/vehicles** - Veritabanından araç listesi çek (YENİ endpoint)

---

## 🔄 Veri Akışı

### SOAP GetVehicleInfo:
```
SOAP İstek
    ↓
Node.js Server
    ↓
MySQL Query (JOIN 4 tablo)
    ↓
SOAP Response (Gerçek veri)
```

### SOAP CalculatePrice:
```
SOAP İstek
    ↓
Node.js Server
    ↓
gRPC → Python AI Service
    ↓
ML Model (Scikit-learn)
    ↓
ExchangeRate API (Döviz kuru)
    ↓
MySQL INSERT (Tahmin kayıt)
    ↓
SOAP Response
```

### REST API:
```
POST /api/predict
    ↓
gRPC → ML Model
    ↓
ExchangeRate API
    ↓
MySQL INSERT
    ↓
JSON Response
```

---

## 📊 Veritabanı Tabloları

### Kullanılan Tablolar:
1. **Ilan** - İlan bilgileri
2. **Arac** - Araç teknik özellikleri
3. **Model** - Model isimleri
4. **Seri** - Seri isimleri
5. **Marka** - Marka isimleri
6. **Fiyat_Tahmin** - Tahmin sonuçları (INSERT)

### Örnek JOIN Query:
```sql
SELECT 
    i.arac_id,
    m.marka_adi,
    s.seri_adi,
    mo.model_adi,
    a.yil,
    i.fiyat
FROM Ilan i
JOIN Arac a ON i.arac_id = a.arac_id
JOIN Model mo ON a.model_id = mo.model_id
JOIN Seri s ON mo.seri_id = s.seri_id
JOIN Marka m ON s.marka_id = m.marka_id
WHERE i.arac_id = ?
```

---

## 🚀 Servisi Başlatma

### 1. MySQL Bağımlılıklarını Yükle:
```powershell
cd C:\projeler\IkinciElArac-main\SOA\API
npm install
```

### 2. Veritabanının Hazır Olduğundan Emin Ol:
- MySQL'in 3308 portunda çalıştığından emin olun
- `arackiralama` veritabanı oluşturulmuş olmalı
- Tablolar ve veriler yüklenmiş olmalı

### 3. Servisi Başlat:
```powershell
npm start
```

**Çıktı:**
```
✅ MySQL veritabanına başarıyla bağlanıldı
SOA Gateway (Node.js) 3000 portunda hazır.
REST API: http://localhost:3000/api/predict
SOAP Servisi: http://localhost:3000/soap/vehicle
WSDL: http://localhost:3000/soap/vehicle?wsdl
```

---

## 🧪 Test Örnekleri

### SOAP Test (Gerçek veri):
```powershell
$body = @'
<?xml version="1.0"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="http://localhost:3000/soap/vehicle">
  <soap:Body>
    <tns:GetVehicleInfoRequest>
      <tns:arac_id>1</tns:arac_id>
    </tns:GetVehicleInfoRequest>
  </soap:Body>
</soap:Envelope>
'@

Invoke-WebRequest -Uri "http://localhost:3000/soap/vehicle" -Method POST -ContentType "text/xml" -Body $body
```

### REST Test (Araç listesi):
```powershell
Invoke-WebRequest http://localhost:3000/api/vehicles | ConvertFrom-Json
```

---

## ✅ Sonuç

Artık tüm servisler **gerçek veritabanı** ile çalışıyor:

1. ✅ SOAP GetVehicleInfo → MySQL'den veri çekiyor
2. ✅ SOAP CalculatePrice → ML tahmin + MySQL'e kayıt
3. ✅ REST API → ML tahmin + MySQL'e kayıt
4. ✅ Yeni endpoint: GET /api/vehicles → Veritabanından liste
