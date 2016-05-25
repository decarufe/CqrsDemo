setlocal

set PATH=%~dp0\.node;%PATH%
.\.nuget\NuGet.exe restore .\Pyxis.Persistance.sln -source "https://www.nuget.org/api/v2;http://nuget.pyxis-tech.com:90/nuget"

endlocal