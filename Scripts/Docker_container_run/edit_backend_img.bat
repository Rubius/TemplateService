rem Создает временный контейнер local_backend_tmp и запускает в нем Bash.
docker rm local_backend_tmp
docker run -it -p 5000:5000 --name local_backend_tmp --entrypoint "/bin/bash" local_backend_img
