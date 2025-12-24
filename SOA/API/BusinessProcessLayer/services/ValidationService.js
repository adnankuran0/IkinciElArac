/**
 * BUSINESS PROCESS LAYER - Validation Service
 * Veri doğrulama servisi
 */

class ValidationService {
    /**
     * Araç verilerini doğrular
     */
    validateVehicleData(vehicleData) {
        const errors = [];

        // Basit kontroler - mevcut mantığı değiştirmiyor
        if (!vehicleData.marka_id || vehicleData.marka_id < 1) {
            errors.push('Geçersiz marka ID');
        }

        if (!vehicleData.yil || vehicleData.yil < 1990) {
            errors.push('Yıl 1990\'dan büyük olmalı');
        }

        if (vehicleData.kilometre < 0) {
            errors.push('Kilometre negatif olamaz');
        }

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    }

    /**
     * Araç ID doğrulama
     */
    validateVehicleId(aracId) {
        if (!aracId || aracId < 1) {
            return {
                isValid: false,
                errors: ['Geçersiz araç ID']
            };
        }
        return { isValid: true, errors: [] };
    }
}

module.exports = ValidationService;
