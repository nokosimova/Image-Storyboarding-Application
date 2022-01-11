# Image-storyboarding-application


Приложение для различной раскадровки изображений

1) Если приложение запускается не на Windows, то  после сборки добавьте в файле runtimeconfig.json в параметр "configProperties" новое значение "System.Drawing.EnableUnixSupport": 

{ 
    "configProperties": 
    { 
        ...,
        "System.Drawing.EnableUnixSupport": true 
    } 
} 
Подробнее: https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only

2) Для тестировки можно использовать изображения в папке /images в проекте. 
