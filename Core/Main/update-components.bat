setlocal

set PATH=%~dp0\.node;%PATH%
.\.nuget\NuGet.exe restore .\Pyxis.Core.sln

endlocal