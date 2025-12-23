import requests
import json

# Node.js API endpoint adresi
url = "http://localhost:3000/api/predict"

# Test edilecek araç verileri (Modelin ve API'nin beklediği format)
test_data = {
    "kullanici_id": 1,
    "arac_id": 1,
    "marka_id": 5,        
    "seri_id": 12,        
    "model_id": 45,       
    "yil": 2018,
    "kilometre": 150000,
    "vites_tipi_id": 0,   
    "yakit_tipi_id": 1,   
    "motor_hacmi": 1582,
    "motor_gucu": 136,
    "hasar_durumu": 0,    
    "takasa_uygun": True,
    "kimden": "Galeriden"
}

def test_soa_api():
    print("--- Python ile SOA API Testi Başlatılıyor ---")
    
    try:
        # Node.js API'sine POST isteği gönder
        response = requests.post(url, json=test_data)
        
        # Yanıtı kontrol et
        if response.status_code == 200:
            result = response.json()
            if result.get("success"):
                print("Test Başarılı!")
                print("-" * 40)
                print(f"Araç Fiyat Tahmini (TL):  {result['tahmin_tl']} ₺")
                print(f"Araç Fiyat Tahmini (EUR): {result['tahmin_eur']} €")
                print(f"Araç Fiyat Tahmini (USD): {result['tahmin_usd']} $")
                print(f"Bilgi: {result['kur_bilgisi']}")
                print("-" * 40)
            else:
                print(f"API Hatası: {result.get('error')}")
        else:
            print(f"Sunucu Hatası! Durum Kodu: {response.status_code}")
            print(f"Yanıt: {response.text}")
            
    except requests.exceptions.ConnectionError:
        print("Hata: Node.js sunucusuna ulaşılamadı. Sunucunun (localhost:3000) açık olduğundan emin olun.")
    except Exception as e:
        print(f"Beklenmedik bir hata oluştu: {e}")

if __name__ == "__main__":
    test_soa_api()