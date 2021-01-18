rem Создает контейнер local_backend и запускает его.
docker rm local_backend
docker run -it -p 5000:5000 --name local_backend local_backend_img
