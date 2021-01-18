rem Локальная сборка бекнда в docker image = local_backend_img
cd ../..
docker build -f Dockerfile_local_build -t local_backend_img .