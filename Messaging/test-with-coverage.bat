setlocal

set PATH=%~dp0\.node;%PATH%
cd main
call test-with-coverage.bat

endlocal