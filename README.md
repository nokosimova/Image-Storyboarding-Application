# Image-storyboarding-application


Приложение для различной раскадровки изображений

*Если приложение запускается не на Windows, то добавьте в файле runtimeconfig.json в параметр "configProperties" новый параметр "System.Drawing.EnableUnixSupport": 
{ 
    "configProperties": 
    { 
        "System.Drawing.EnableUnixSupport": true 
    } 
} 
Подробнее: https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only
