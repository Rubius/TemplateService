rem Создает копию контейнера local_backend и сохраняет как образ с именем local_backend_edit_img. Этот образ запускается как контейнер с именем local_backend_edit.

docker rm local_backend_edit
docker rmi local_backend_edit_img

docker commit local_backend local_backend_edit_img

docker run -it -p 5000:5000 --name local_backend_edit --entrypoint "/bin/bash" local_backend_edit_img
