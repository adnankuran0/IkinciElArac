/**
 * BUSINESS PROCESS LAYER - Vehicle Info Workflow
 * Araç bilgisi sorgulama iş akışı - server.js'teki GetVehicleInfo mantığını taşır
 */

const db = require('../../database');

class VehicleInfoWorkflow {
    /**
     * Araç bilgisi getir
     * Mantık: server.js'teki GetVehicleInfo ile tamamen aynı
     */
    async execute(aracId) {
        try {
            // Veritabanından araç bilgisini çek (mevcut mantık)
            const [rows] = await db.query(`
                SELECT 
                    i.ilan_id,
                    i.arac_id,
                    m.marka_adi,
                    s.seri_adi,
                    mo.model_adi,
                    CONCAT(m.marka_adi, ' ', s.seri_adi, ' ', mo.model_adi) as full_model,
                    a.yil,
                    a.kilometre,
                    i.fiyat,
                    a.vites_tipi,
                    a.yakit_tipi,
                    a.motor_hacmi,
                    a.motor_gucu
                FROM Ilan i
                JOIN Arac a ON i.arac_id = a.arac_id
                JOIN Model mo ON a.model_id = mo.model_id
                JOIN Seri s ON mo.seri_id = s.seri_id
                JOIN Marka m ON s.marka_id = m.marka_id
                WHERE i.arac_id = ?
                LIMIT 1
            `, [aracId]);

            if (rows.length === 0) {
                return {
                    success: false,
                    arac_id: aracId,
                    marka: "",
                    model: "",
                    yil: 0,
                    fiyat: 0,
                    mesaj: "Araç bulunamadı (BPL Workflow - Database)"
                };
            }

            const vehicle = rows[0];
            
            return {
                success: true,
                arac_id: aracId,
                marka: vehicle.marka_adi,
                model: vehicle.full_model,
                yil: vehicle.yil,
                fiyat: parseFloat(vehicle.fiyat),
                mesaj: "Araç bilgisi başarıyla getirildi (BPL Workflow - Database Query)"
            };
        } catch (error) {
            console.error('VehicleInfoWorkflow DB Error:', error);
            throw new Error("Veritabanı hatası: " + error.message);
        }
    }

    /**
     * Araç listesi getir
     * Mantık: server.js'teki GET /api/vehicles ile aynı
     */
    async getVehicleList(limit = 100) {
        try {
            const [rows] = await db.query(`
                SELECT 
                    i.ilan_id,
                    i.arac_id,
                    m.marka_adi,
                    s.seri_adi,
                    mo.model_adi,
                    a.yil,
                    a.kilometre,
                    i.fiyat,
                    a.vites_tipi,
                    a.yakit_tipi
                FROM Ilan i
                JOIN Arac a ON i.arac_id = a.arac_id
                JOIN Model mo ON a.model_id = mo.model_id
                JOIN Seri s ON mo.seri_id = s.seri_id
                JOIN Marka m ON s.marka_id = m.marka_id
                ORDER BY i.ilan_tarihi DESC
                LIMIT ?
            `, [limit]);

            return {
                success: true,
                count: rows.length,
                data: rows
            };
        } catch (error) {
            console.error('VehicleInfoWorkflow List Error:', error);
            throw new Error('Veritabanı hatası: ' + error.message);
        }
    }
}

module.exports = VehicleInfoWorkflow;
