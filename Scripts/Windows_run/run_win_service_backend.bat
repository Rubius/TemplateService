rem Создает службу Windows и запускает ее.
set C_D=%CD%

echo Administrator rights required
sc delete TemplateService
sc create TemplateService binPath= "%C_D%\publish\TemplateService\TemplateService.exe --win-service --urls http://localhost:5000"
sc start TemplateService 


