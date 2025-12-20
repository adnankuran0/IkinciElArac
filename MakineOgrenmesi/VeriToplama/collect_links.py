import undetected_chromedriver as uc
from selenium.webdriver.common.by import By
import time

driver = uc.Chrome()

links = set()  

for page in range(50):
    url = f"https://www.arabam.com/ikinci-el/otomobil?page={page}"
    driver.get(url)
    time.sleep(2)

    rows = driver.find_elements(By.CSS_SELECTOR, "tr.listing-list-item")

    for row in rows:
        try:
            a = row.find_element(By.CSS_SELECTOR, "a[href*='/ilan/']")
            links.add(a.get_attribute("href"))
        except:
            pass

driver.quit()

print("Toplam benzersiz link:", len(links))

with open("ilanlar.txt", "w", encoding="utf-8") as f:
    for l in links:
        f.write(l + "\n")

print("Kaydedildi: ilanlar.txt")