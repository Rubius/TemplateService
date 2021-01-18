rem Запускат сборку сервиса как консольное приложение
set C_D=%CD%

cd ../..
call dotnet publish TemplateService.sln -c Debug -o %C_D%/publish/TemplateService
dotnet "%C_D%\publish\TemplateService\TemplateService.dll" --urls http://localhost:5000



