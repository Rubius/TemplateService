rem Собирает код службы
set C_D=%CD%

cd ../..
call dotnet publish TemplateService.sln -c Debug -o %C_D%/publish/TemplateService

