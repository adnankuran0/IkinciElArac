// server.js
const express = require('express');
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const axios = require('axios');
const app = express();

app.use(express.json());

// gRPC İstemci Yapılandırması
const packageDefinition = protoLoader.loadSync('gRPC/predict.proto', {});
const predictProto = grpc.loadPackageDefinition(packageDefinition).PricePrediction;
const client = new predictProto('localhost:50051', grpc.credentials.createInsecure());

// --- REST API ENDPOINT ---
app.post('/api/predict', async (req, res) => {
    // 1. Frontend'den gelen verileri al
    const vehicleData = {
        marka: req.body.marka_id,
        seri: req.body.seri_id,
        model: req.body.model_id,
        yil: req.body.yil,
        kilometre: req.body.kilometre,
        vites_tipi: req.body.vites_tipi_id, // 0: Manuel, 1: Otomatik vb.
        yakit_tipi: req.body.yakit_tipi_id,
        motor_hacmi: req.body.motor_hacmi,
        motor_gucu: req.body.motor_gucu,
        hasar_durumu: req.body.hasar_durumu // 0, 1 veya 2
    };

    // 2. Python Servisine gRPC ile bağlan
    client.PredictPrice(vehicleData, async (err, response) => {
        if (err) {
            console.error("gRPC Hatası:", err);
            return res.status(500).json({ error: "AI Servisine ulaşılamadı" });
        }

        const priceTL = response.tahmin_edilen_fiyat;

        // 3. Hazır API Kullanımı (Döviz Kuru)
        let priceEUR = 0;
        try {
            const exchange = await axios.get('https://api.exchangerate-api.com/v4/latest/TRY');
            priceEUR = priceTL * exchange.data.rates.EUR;
        } catch (e) {
            priceEUR = priceTL * 0.027; // Hata durumunda sabit kur
        }

        // 4. Sonucu Frontend'e dön
        res.json({
            success: true,
            tahmin_tl: priceTL,
            tahmin_eur: priceEUR,
            kur_bilgisi: "Anlık veriler Hazır API üzerinden çekildi"
        });
    });
});

app.listen(3000, () => console.log("SOA Gateaway (Node.js) 3000 portunda hazır."));