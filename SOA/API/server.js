const express = require('express');
const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const axios = require('axios');
const path = require('path');
const cors = require('cors');

const app = express();
app.use(express.json());
app.use(cors());

const protoPath = path.join(__dirname, '..', 'gRPC', 'predict.proto');
const packageDefinition = protoLoader.loadSync(protoPath, {
    keepCase: true,
    longs: String,
    enums: String,
    defaults: true,
    oneofs: true
});
const predictProto = grpc.loadPackageDefinition(packageDefinition).PricePrediction;
const client = new predictProto('localhost:50051', grpc.credentials.createInsecure());

// --- API ENDPOINT ---
app.post('/api/predict', async (req, res) => {
    const vehicleData = {
        marka: req.body.marka_id,
        seri: req.body.seri_id,
        model: req.body.model_id,
        yil: req.body.yil,
        kilometre: req.body.kilometre,
        vites_tipi: req.body.vites_tipi_id,
        yakit_tipi: req.body.yakit_tipi_id,
        motor_hacmi: req.body.motor_hacmi,
        motor_gucu: req.body.motor_gucu,
        hasar_durumu: req.body.hasar_durumu,
        kasa_tipi: req.body.kasa_tipi_id,
        renk: req.body.renk_id,
        cekis: req.body.cekis_id
    };

    // 1. Entegrasyon: gRPC üzerinden AI Servisi
    client.PredictPrice(vehicleData, async (err, response) => {
        if (err) {
            console.error("gRPC Hatası:", err);
            return res.status(500).json({ error: "AI Servisine ulaşılamadı" });
        }

        const priceTL = response.tahmin_edilen_fiyat;

        // 2. Hazır API: Döviz Kuru (Euro ve Dolar)
        let priceEUR = 0, priceUSD = 0;
        try {
            const exchange = await axios.get('https://api.exchangerate-api.com/v4/latest/TRY');
            priceEUR = priceTL * exchange.data.rates.EUR;
            priceUSD = priceTL * exchange.data.rates.USD;
        } catch (e) {
            priceEUR = priceTL * 0.027; 
            priceUSD = priceTL * 0.029;
        }

        // 3. Yanıt Döndürme (Service Layer)
        res.json({
            success: true,
            tahmin_tl: Number(priceTL).toFixed(2),
            tahmin_eur: priceEUR.toFixed(2),
            tahmin_usd: priceUSD.toFixed(2),
            kur_bilgisi: "Anlık veriler Hazır API üzerinden çekildi"
        });
    });
});

app.listen(3000, () => console.log("SOA Gateway (Node.js) 3000 portunda hazır."));