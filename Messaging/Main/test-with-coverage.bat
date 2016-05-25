setlocal

set BUILD_TARGET=debug
set PATH=%~dp0\.node;%PATH%
set NUNIT=packages\NUnit.Runners.2.6.4\tools\nunit-console.exe
set OPENCOVER=packages\OpenCover.4.5.3723\OpenCover.Console.exe
set REPORT_GEN=packages\ReportGenerator.2.1.3.0\ReportGenerator.exe
set TOCOBERTURA=packages\OpenCoverToCoberturaConverter.0.2.1.0\OpenCoverToCoberturaConverter.exe
set TEST_TARGETS=Pyxis.Messaging.Tests\bin\%BUILD_TARGET%\Pyxis.Messaging.Tests.dll  Pyxis.Messaging.Azure.Tests\bin\%BUILD_TARGET%\Pyxis.Messaging.Azure.Tests.dll 
set FILTER_INCLUSION=+[Pyxis.Messaging]* +[Pyxis.Messaging.Azure]*
set FILTER_EXCLUSION=
mkdir Reports\Coverage
del /Q Reports

%OPENCOVER% -register:user "-target:%NUNIT%" "-targetargs:%TEST_TARGETS% /noshadow /xml:Reports\TestResults.xml" -output:Reports\OpenCover.xml "-filter: %FILTER_INCLUSION% %FILTER_EXCLUSION%"

rem to enable code coverage uncomment the two following lines
%TOCOBERTURA% -input:Reports\OpenCover.xml -output:Reports\cobertura.xml -sources:.
%REPORT_GEN% -reports:Reports\OpenCover.xml -targetdir:Reports\Coverage\Server

endlocal
