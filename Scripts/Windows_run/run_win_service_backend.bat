set C_D=%CD%

cd ../..
call dotnet publish TemplateService.sln -c Debug -o %C_D%/publish/TemplateService

sc delete TemplateService
sc create TemplateService binPath= "%C_D%\publish\TemplateService\TemplateService.exe --win-service --urls http://localhost:5000"
sc start TemplateService 


