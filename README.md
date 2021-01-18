# Template microservice for ASP.NET CORE.

Проект шаблона микросервиса на ASP.NET CORE. Реализует полностью рабочий микросервис с базовым функционалом:
- чтение файлов настроек: appsetting.json и appsetting.{ASPNETCORE_ENVIRONMENT}.json
- чтение настроек подключения к БД и RabbitMQ из переменных окружения
- работа с базой данных через EntityFramework (опционально, при задании настроек)
- обмен сообщений через RabbitMQ (опционально, при задании настроек)
- проверка статусов healthCheck для БД и RabbitMQ (опционально, при задании настроек)
- базовое логирование в файл Log/log.txt через Serilog
- подключение документирования REST API для контроллеров на WEB-странице через Swagger

Развертывание и запуск сервиса возможно как:
 - Linux docker-контейнер (см. скрипты в Scripts/Docker_container_run)
 - служба Windows (см. скрипты в Scripts/Windows_run)
 - консольное ASP.NET CORE приложение.

-------------
Для тестирования микросервиса в Visual Studio (консольное приложение) необходимо:
- настроить appsettings.json (или appsettings.Development.json):
  - при работе с БД - задать нужные поля DataBaseSettings. Иначе - стереть эту секцию.
  - для обмена сообщениями через RabbitMQ - настроить поля RabbitMqSettings. Иначе стереть эту секцию.
  - задать уровень логирования через LogEventLevel
- настроить порт запуска в launchSettings.json. При задании порта желательно проверить, что он не является зарезервированным, для этого запустите скрипт:
  - Scripts/"Список зарезервированных портов в Windows".bat
- при необходимости запустить сервис RabbitMQ, это можно сделать через скрипт:
  - Scripts/run_rabbit_mq.bat

-------------
Для сборки и запуска микросервиса как Docker-контейнера необходимо:
- задать нужные переменные окружения (ENV) контейнера в файле "Dockerfile_local_build"
- запустить сборку образа через build_backend.bat (см. Scripts\Docker_container_run)
- создать контейнер из готового образа и запустить его через run_backend.bat

-------------
Для сборки и запуска микросервиса как Windows-службы необходимо:
- запустить сборку службы через build_win_service_backend.bat (см Scripts\Windows_run)
- настроить файл Scripts\Windows_run\publish\appsettings.json
- запустить службу, для этого в режиме администратора запустите скрипт run_win_service_backend.bat

-------------
После локального запуска микросервиса доступны следующие WEB-страницы:
- тестирования REST API: http://localhost:{port}/swagger
- статус healthCheck: http://localhost:{port}/health
