/**
 * BUSINESS PROCESS LAYER - Price Prediction Workflow
 * Fiyat tahmin iş akışı - server.js'teki mantığı taşır
 */

const axios = require('axios');
const db = require('../../database');

class PricePredictionWorkflow {
    constructor(grpcClient) {
        this.grpcClient = grpcClient;
    }

    /**
     * Fiyat tahmin iş akışını çalıştırır
     * Mantık: server.js'teki CalculatePrice ve /api/predict ile aynı
     */
    async execute(vehicleData, kullaniciId = 1, aracId = 0) {
        return new Promise((resolve, reject) => {
            // 1. gRPC servisine gönder (mevcut mantık)
            const grpcData = {
                marka: vehicleData.marka_id || vehicleData.marka,
                seri: vehicleData.seri_id || vehicleData.seri,
                model: vehicleData.model_id || vehicleData.model,
                yil: vehicleData.yil,
                kilometre: vehicleData.kilometre,
                vites_tipi: vehicleData.vites_tipi_id || vehicleData.vites_tipi,
                yakit_tipi: vehicleData.yakit_tipi_id || vehicleData.yakit_tipi,
                motor_hacmi: vehicleData.motor_hacmi,
                motor_gucu: vehicleData.motor_gucu,
                hasar_durumu: vehicleData.hasar_durumu,
                kasa_tipi: 0,
                renk: 0,
                cekis: 0
            };

            this.grpcClient.PredictPrice(grpcData, async (err, response) => {
                if (err) {
                    reject(new Error("gRPC servisi hatası: " + err.message));
                    return;
                }

                try {
                    const priceTL = Number(response.tahmin_edilen_fiyat);

                    // 2. Döviz kuru al (mevcut mantık)
                    let priceEUR = 0, priceUSD = 0;
                    try {
                        const exchange = await axios.get('https://api.exchangerate-api.com/v4/latest/TRY');
                        priceEUR = priceTL * exchange.data.rates.EUR;
                        priceUSD = priceTL * exchange.data.rates.USD;
                    } catch {
                        priceEUR = priceTL * 0.027;
                        priceUSD = priceTL * 0.029;
                    }

                    // 3. Veritabanına kaydet (mevcut mantık)
                    try {
                        const [aracResult] = await db.query(`
                        INSERT INTO Arac (
                            model_id, yil, kilometre,
                            vites_tipi, yakit_tipi,
                            motor_hacmi, motor_gucu,
                            arac_durumu
                        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)
                    `, [
    vehicleData.model_id,
    vehicleData.yil,
    vehicleData.kilometre,
    vehicleData.vites_tipi_id === 0 ? 'Manuel' : 'Otomatik',
    vehicleData.yakit_tipi_id === 0 ? 'Benzin' : 'Dizel',
    vehicleData.motor_hacmi,
    vehicleData.motor_gucu,
    'IkinciEl'
]
);

                    const aracId = aracResult.insertId;

                        await db.query(`
                            INSERT INTO Fiyat_Tahmin (kullanici_id, arac_id, tahmin_edilen_fiyat)
                            VALUES (?, ?, ?)
                        `, [kullaniciId, aracId, priceTL]);
                        
                        console.log('✅ Tahmin veritabanına kaydedildi (BPL Workflow)');
                    } catch (dbErr) {
                        console.error('❌ DB kayıt hatası:', dbErr.message);
                    }

                    // 4. Sonucu döndür (mevcut mantık)
                    resolve({
                        success: true,
                        tahmin_tl: priceTL.toFixed(2),
                        tahmin_eur: priceEUR.toFixed(2),
                        tahmin_usd: priceUSD.toFixed(2),
                        mesaj: "Fiyat tahmini başarıyla hesaplandı (BPL → gRPC → ML Model → Database)"
                    });
                } catch (error) {
                    reject(error);
                }
            });
        });
    }
}

module.exports = PricePredictionWorkflow;
