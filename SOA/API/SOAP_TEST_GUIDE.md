# SOAP Servisi Test Rehberi

## WSDL Erişimi
```
http://localhost:3000/soap/vehicle?wsdl
```

## Test 1: GetVehicleInfo (Araç Bilgisi Sorgulama)

### cURL ile Test:
```bash
curl -X POST http://localhost:3000/soap/vehicle \
  -H "Content-Type: text/xml" \
  -d '<?xml version="1.0" encoding="UTF-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="http://localhost:3000/soap/vehicle">
  <soap:Body>
    <tns:GetVehicleInfoRequest>
      <tns:arac_id>14444</tns:arac_id>
    </tns:GetVehicleInfoRequest>
  </soap:Body>
</soap:Envelope>'
```

### PowerShell ile Test:
```powershell
$body = @'
<?xml version="1.0" encoding="UTF-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="http://localhost:3000/soap/vehicle">
  <soap:Body>
    <tns:GetVehicleInfoRequest>
      <tns:arac_id>14444</tns:arac_id>
    </tns:GetVehicleInfoRequest>
  </soap:Body>
</soap:Envelope>
'@

Invoke-WebRequest -Uri "http://localhost:3000/soap/vehicle" -Method POST -ContentType "text/xml" -Body $body
```

---

## Test 2: CalculatePrice (Fiyat Tahmini - gRPC üzerinden)

### cURL ile Test:
```bash
curl -X POST http://localhost:3000/soap/vehicle \
  -H "Content-Type: text/xml" \
  -d '<?xml version="1.0" encoding="UTF-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="http://localhost:3000/soap/vehicle">
  <soap:Body>
    <tns:CalculatePriceRequest>
      <tns:marka_id>5</tns:marka_id>
      <tns:seri_id>12</tns:seri_id>
      <tns:model_id>45</tns:model_id>
      <tns:yil>2020</tns:yil>
      <tns:kilometre>50000</tns:kilometre>
      <tns:vites_tipi_id>0</tns:vites_tipi_id>
      <tns:yakit_tipi_id>1</tns:yakit_tipi_id>
      <tns:motor_hacmi>1582</tns:motor_hacmi>
      <tns:motor_gucu>136</tns:motor_gucu>
      <tns:hasar_durumu>0</tns:hasar_durumu>
    </tns:CalculatePriceRequest>
  </soap:Body>
</soap:Envelope>'
```

### PowerShell ile Test:
```powershell
$body = @'
<?xml version="1.0" encoding="UTF-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="http://localhost:3000/soap/vehicle">
  <soap:Body>
    <tns:CalculatePriceRequest>
      <tns:marka_id>5</tns:marka_id>
      <tns:seri_id>12</tns:seri_id>
      <tns:model_id>45</tns:model_id>
      <tns:yil>2020</tns:yil>
      <tns:kilometre>50000</tns:kilometre>
      <tns:vites_tipi_id>0</tns:vites_tipi_id>
      <tns:yakit_tipi_id>1</tns:yakit_tipi_id>
      <tns:motor_hacmi>1582</tns:motor_hacmi>
      <tns:motor_gucu>136</tns:motor_gucu>
      <tns:hasar_durumu>0</tns:hasar_durumu>
    </tns:CalculatePriceRequest>
  </soap:Body>
</soap:Envelope>
'@

Invoke-WebRequest -Uri "http://localhost:3000/soap/vehicle" -Method POST -ContentType "text/xml" -Body $body
```

---

## Postman ile Test:

1. **Method:** POST
2. **URL:** `http://localhost:3000/soap/vehicle`
3. **Headers:**
   - Content-Type: `text/xml`
4. **Body:** Raw (XML) - soap-test.xml dosyasından kopyalayın

---

## SOAP UI ile Test:

1. SOAP UI'yi açın
2. New SOAP Project oluşturun
3. WSDL URL: `http://localhost:3000/soap/vehicle?wsdl`
4. İki method görünecek: GetVehicleInfo ve CalculatePrice
5. Test istekleri otomatik oluşturulacak
