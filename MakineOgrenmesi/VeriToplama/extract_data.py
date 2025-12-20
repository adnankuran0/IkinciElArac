import undetected_chromedriver as uc
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import csv

with open("ilanlar.txt", "r", encoding="utf-8") as f:
    links = [line.strip() for line in f.readlines() if line.strip()]

driver = uc.Chrome()
wait = WebDriverWait(driver, 10)  

csv_file = open("ilan_detay_test.csv", "w", newline="", encoding="utf-8")
writer = csv.writer(csv_file)

header = [
    "Fiyat", "İlan Tarihi", "Marka", "Seri", "Model", "Yıl",
    "Kilometre", "Vites Tipi", "Yakıt Tipi", "Kasa Tipi", "Renk",
    "Motor Hacmi", "Motor Gücü", "Çekiş", "Araç Durumu",
    "Ort. Yakıt Tüketimi", "Yakıt Deposu", "Boya-değişen",
    "Takasa Uygun", "Kimden"
]
writer.writerow(header)

for link in links:
    print("Scraping:", link)
    driver.get(link)

    try:
        wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, ".desktop-information-price")))
    except:
        print("Sayfa yüklenmedi, atlandı:", link)
        continue

    data = {}

    try:
        data["Fiyat"] = driver.find_element(By.CSS_SELECTOR, ".desktop-information-price").text.strip()
    except:
        data["Fiyat"] = ""

    items = driver.find_elements(By.CSS_SELECTOR, ".product-properties-details .property-item")

    for item in items:
        try:
            key = item.find_element(By.CSS_SELECTOR, ".property-key").text.strip()
            val = item.find_element(By.CSS_SELECTOR, ".property-value").text.strip()
            data[key] = val
        except:
            pass

    row = [
        data.get("Fiyat", ""),
        data.get("İlan Tarihi", ""),
        data.get("Marka", ""),
        data.get("Seri", ""),
        data.get("Model", ""),
        data.get("Yıl", ""),
        data.get("Kilometre", ""),
        data.get("Vites Tipi", ""),
        data.get("Yakıt Tipi", ""),
        data.get("Kasa Tipi", ""),
        data.get("Renk", ""),
        data.get("Motor Hacmi", ""),
        data.get("Motor Gücü", ""),
        data.get("Çekiş", ""),
        data.get("Araç Durumu", ""),
        data.get("Ort. Yakıt Tüketimi", ""),
        data.get("Yakıt Deposu", ""),
        data.get("Boya-değişen", ""),
        data.get("Takasa Uygun", ""),
        data.get("Kimden", "")
    ]

    writer.writerow(row)

driver.quit()
csv_file.close()

print("ilan_detay_test.csv oluşturuldu.")