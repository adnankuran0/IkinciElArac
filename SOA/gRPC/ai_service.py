import grpc
from concurrent import futures
import predict_pb2
import predict_pb2_grpc
import joblib
import numpy as np

model = joblib.load('../../MakineOgrenmesi/out/best_model.joblib')

class PricePredictionServicer(predict_pb2_grpc.PricePredictionServicer):
    def PredictPrice(self, request, context):
        yas = 2025 - request.yil
        
        input_data = np.array([[
            request.marka,        
            request.seri,         
            request.model,        
            request.kilometre,    
            request.vites_tipi,   
            request.yakit_tipi,  
            request.kasa_tipi, 
            request.renk,        
            request.motor_hacmi, 
            request.motor_gucu, 
            request.cekis,      
            request.hasar_durumu, 
            yas                   
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