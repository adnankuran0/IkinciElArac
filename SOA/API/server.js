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
        marka: Number(req.body.marka_id),
        seri: Number(req.body.seri_id),
        model: Number(req.body.model_id),
        yil: Number(req.body.yil),
        kilometre: Number(req.body.kilometre),
        vites_tipi: Number(req.body.vites_tipi_id),
        yakit_tipi: Number(req.body.yakit_tipi_id),
        motor_hacmi: Number(req.body.motor_hacmi),
        motor_gucu: Number(req.body.motor_gucu),
        hasar_durumu: Number(req.body.hasar_durumu),

        // test.py'den gelen ama daha önce kullanılmayan alanlar
        takasa_uygun: req.body.takasa_uygun ? 1 : 0,
        kimden: req.body.kimden === "Galeriden" ? 1 : 0
    };

    client.PredictPrice(vehicleData, async (err, response) => {
        if (err) {
            console.error("gRPC Hatası:", err);
            return res.status(500).json({ error: "AI Servisine ulaşılamadı" });
        }

        const priceTL = Number(response.tahmin_edilen_fiyat);

        let priceEUR = 0, priceUSD = 0;
        try {
            const exchange = await axios.get('https://api.exchangerate-api.com/v4/latest/TRY');
            priceEUR = priceTL * exchange.data.rates.EUR;
            priceUSD = priceTL * exchange.data.rates.USD;
        } catch {
            priceEUR = priceTL * 0.027;
            priceUSD = priceTL * 0.029;
        }

        res.json({
            success: true,
            tahmin_tl: priceTL.toFixed(2),
            tahmin_eur: priceEUR.toFixed(2),
            tahmin_usd: priceUSD.toFixed(2),
            kur_bilgisi: "Anlık veriler Hazır API üzerinden çekildi"
        });
    });
});


app.listen(3000, () => console.log("SOA Gateway (Node.js) 3000 portunda hazır."));