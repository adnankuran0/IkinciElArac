import pandas as pd
import numpy as np
from pathlib import Path
import joblib

import matplotlib.pyplot as plt

from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import r2_score, mean_absolute_error
from sklearn.linear_model import LinearRegression
from sklearn.ensemble import RandomForestRegressor, GradientBoostingRegressor


# =========================
# CONFIG
# =========================
DATA_PATH = "toplu.csv"
OUT_DIR = Path("out")
OUT_DIR.mkdir(exist_ok=True)

plt.rcParams["figure.figsize"] = (7, 4)


# =========================
# HELPERS
# =========================
def save_step(df: pd.DataFrame, name: str):
    df.to_csv(OUT_DIR / f"{name}.csv", index=False)


def clean_price(val):
    if pd.isna(val):
        return np.nan
    val = str(val).replace("TL", "").replace(".", "").strip()
    if val == "":
        return np.nan
    try:
        return float(val)
    except:
        return np.nan


def clean_km(val):
    if pd.isna(val):
        return np.nan
    val = str(val).replace("km", "").replace(".", "").strip()
    try:
        return float(val)
    except:
        return np.nan


def clean_engine(val):
    if pd.isna(val):
        return np.nan
    val = (
        str(val)
        .lower()
        .replace("cc", "")
        .replace("hp", "")
        .replace("cm3", "")
        .replace(".", "")
        .strip()
    )
    if "-" in val:
        try:
            a, b = val.split("-")
            return (float(a) + float(b)) / 2
        except:
            return np.nan
    try:
        return float(val)
    except:
        return np.nan


def hasar_durumu_analiz(text):
    text = str(text).lower()
    if "orjinal" in text or "hatasız" in text:
        return 0
    elif "belirtilmemiş" in text:
        return 2
    else:
        return 1


# =========================
# MAIN PIPELINE
# =========================
def main():
    print("📥 Veri okunuyor...")
    df = pd.read_csv(DATA_PATH)
    save_step(df, "01_raw_data")

    # -------- PRICE --------
    df["Fiyat"] = df["Fiyat"].apply(clean_price)
    df = df.dropna(subset=["Fiyat"])

    low = df["Fiyat"].quantile(0.01)
    high = df["Fiyat"].quantile(0.99)
    df = df[(df["Fiyat"] > low) & (df["Fiyat"] < high)]
    save_step(df, "04_price_cleaned")

    # -------- KM --------
    df["Kilometre"] = df["Kilometre"].apply(clean_km)
    save_step(df, "05_kilometre_cleaned")

    # -------- ENGINE --------
    df["Motor Hacmi"] = df["Motor Hacmi"].apply(clean_engine)
    df["Motor Gücü"] = df["Motor Gücü"].apply(clean_engine)
    save_step(df, "06_engine_cleaned")

    # -------- FEATURE ENG --------
    df["Hasar_Durumu"] = df["Boya-değişen"].apply(hasar_durumu_analiz)
    df["Yas"] = 2025 - df["Yıl"]

    num_cols = ["Yas", "Kilometre", "Motor Hacmi", "Motor Gücü"]
    for col in num_cols:
        df[col] = df[col].fillna(df[col].median())

    save_step(df, "08_missing_filled")

    # -------- CATEGORICAL --------
    cat_cols = [
        "Marka", "Seri", "Model", "Vites Tipi",
        "Yakıt Tipi", "Kasa Tipi", "Renk", "Çekiş"
    ]

    label_encoders = {}
    for col in cat_cols:
        df[col] = df[col].fillna("Bilinmiyor")
        le = LabelEncoder()
        df[col] = le.fit_transform(df[col].astype(str))
        label_encoders[col] = le

    save_step(df, "09_encoded")

    # -------- TRAIN --------
    X = df.drop(
        [
            "Fiyat", "İlan Tarihi", "Yakıt Deposu",
            "Ort. Yakıt Tüketimi", "Boya-değişen",
            "Takasa Uygun", "Kimden", "Araç Durumu", "Yıl"
        ],
        axis=1,
        errors="ignore"
    )
    y = df["Fiyat"]

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42
    )

    models = {
        "LinearRegression": LinearRegression(),
        "RandomForest": RandomForestRegressor(n_estimators=300, random_state=42),
        "GradientBoosting": GradientBoostingRegressor(
            n_estimators=1000, learning_rate=0.05, max_depth=6, random_state=42
        ),
    }

    results = []

    for name, model in models.items():
        print(f"🚀 Eğitim: {name}")
        model.fit(X_train, y_train)
        preds = model.predict(X_test)

        results.append({
            "Model": name,
            "R2": r2_score(y_test, preds),
            "MAE": mean_absolute_error(y_test, preds)
        })

    results_df = pd.DataFrame(results).sort_values("R2", ascending=False)
    print(results_df)

    best_model_name = results_df.iloc[0]["Model"]
    best_model = models[best_model_name]

    MODEL_PATH = OUT_DIR / "best_model.joblib"
    joblib.dump(best_model, MODEL_PATH)

    print("✅ En iyi model:", best_model_name)
    print("💾 Kaydedildi:", MODEL_PATH)


if __name__ == "__main__":
    main()
