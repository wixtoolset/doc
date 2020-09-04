@setlocal
@pushd %~dp0

@set _C=Release
@set _INT=%~dp0build\obj\%_C%
@set _RCO=/S /R:1 /W:1 /NP /XO  /NS /NC /NFL /NDL /NJH /NJS
@set _REND=%~dp0build\%_C%\manual
@if "%1" NEQ "" set _REND=%1


nuget restore || exit /b

::// build tools
dotnet test -c Release || exit /b

::// copy manual source to intermediate tree
robocopy src\manual %_INT%\manual %_RCO%

::// build reference markdown from xsds into intermediate tree
build\%_C%\netcoreapp3.1\XsdToMarkdown.exe -out %_INT%\manual\documents\Reference src\xsd4\*.xsd || exit /b

::// build manual
tools\tinySite\tinySite render -out %_REND% %_INT%\manual || exit /b
@echo See it here:
@echo tools\tinySite\tinySite watch -out %_REND% %_INT%\manual

@popd
@endlocal
