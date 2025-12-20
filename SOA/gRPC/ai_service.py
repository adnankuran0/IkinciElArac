import grpc
from concurrent import futures
import predict_pb2
import predict_pb2_grpc
import joblib
import numpy as np

model = joblib.load('../MakineOgrenmesi/out/best_model.joblib')

class PricePredictionServicer(predict_pb2_grpc.PricePredictionServicer):
    def PredictPrice(self, request, context):
        # Yıl bilgisinden Yas türetme (Model eğitimindeki gibi)
        yas = 2025 - request.yil
        
        # EĞİTİMDEKİ TAM SIRA: 
        # Marka, Seri, Model, Kilometre, Vites, Yakit, Kasa, Renk, Hacim, Guc, Cekis, Hasar, Yas
        input_data = np.array([[
            request.marka,        # 1
            request.seri,         # 2
            request.model,        # 3
            request.kilometre,    # 4
            request.vites_tipi,   # 5
            request.yakit_tipi,   # 6
            request.kasa_tipi,    # 7
            request.renk,         # 8
            request.motor_hacmi,  # 9
            request.motor_gucu,   # 10
            request.cekis,        # 11
            request.hasar_durumu, # 12
            yas                   # 13
        ]])
        
        prediction = model.predict(input_data)[0]
        return predict_pb2.PredictionResponse(tahmin_edilen_fiyat=float(prediction))

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    predict_pb2_grpc.add_PricePredictionServicer_to_server(PricePredictionServicer(), server)
    server.add_insecure_port('[::]:50051')
    print("Python gRPC Servisi 50051 portunda çalışıyor...")
    server.start()
    server.wait_for_termination()

if __name__ == '__main__':
    serve()