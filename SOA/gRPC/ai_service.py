import grpc
from concurrent import futures
import predict_pb2 as predict_pb2
import predict_pb2_grpc as predict_pb2_grpc
import joblib
import numpy as np

# Modeli yükle
model = joblib.load('../MakineOgrenmesi/out/best_model.joblib')

class PricePredictionServicer(predict_pb2_grpc.PricePredictionServicer):
    def PredictPrice(self, request, context):
        # Gelen veriyi modelin beklediği formata sok (Notebook'taki yapı)
        input_data = np.array([[
            request.marka, request.seri, request.model, request.yil,
            request.kilometre, request.vites_tipi, request.yakit_tipi,
            request.motor_hacmi, request.motor_gucu, request.hasar_durumu
        ]])
        
        prediction = model.predict(input_data)[0]
        return predict_pb2.PredictionResponse(tahmin_edilen_fiyat=prediction)

def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    predict_pb2_grpc.add_PricePredictionServicer_to_server(PricePredictionServicer(), server)
    server.add_insecure_port('[::]:50051')
    print("Python gRPC Servisi 50051 portunda çalışıyor...")
    server.start()
    server.wait_for_termination()

if __name__ == '__main__':
    serve()