@rem call init-project.bat "<platform>" "<dest>"
	@setlocal enableextensions
	@set juniper=%~dp0
	@set platform=%~1
	@set dest=%~2

	@echo %juniper%
	@echo Platform: %platform%
	@echo Destination: %dest%

	@echo "Initializing Juniper Assets at: %dest%"

	mkdir "%dest%"
	mkdir "%dest%\Assets"
	mkdir "%dest%\Assets\Editor"
	mkdir "%dest%\Assets\Juniper"
	mkdir "%dest%\Assets\Plugins"
	mkdir "%dest%\Assets\Plugins\Editor"
	mkdir "%dest%\Assets\StreamingAssets"

	echo.%platform%> "%dest%\Assets\StreamingAssets\juniper.txt"

	@call :link /J "%dest%\AssetStore" "%juniper%..\lib\Unity Asset Store Packages"
	@call :link /J "%dest%\Assets\ShaderControl" "%juniper%Unity Asset Store Packages\ShaderControl"
	@call :link /j "%dest%\Assets\TextMesh Pro" "%juniper%Unity Asset Store Packages\TextMesh Pro"
	@call :link /J "%dest%\Assets\SpeechSDK" "%juniper%Unity Asset Store Packages\SpeechSDK"
	@if not "%platform:Oculus=%" == "%platform%" call :link /J "%dest%\Assets\Oculus" "%juniper%Unity Asset Store Packages\Oculus"

	@call :link /H "%dest%\Assets\csc.rsp" "%juniper%csc.rsp"
	@call :link /H "%dest%\Assets\link.xml" "%juniper%link.xml"
	@call :link /J "%dest%\Assets\Editor\Juniper" "%juniper%Unity Editor Scripts"
	@call :link /J "%dest%\Assets\Plugins\Juniper" "%juniper%Unity Plugins"
	@call :link /J "%dest%\Assets\Plugins\Editor\Juniper" "%juniper%Unity Editor Plugins"
	@call :link /J "%dest%\Assets\Juniper\Assets" "%juniper%Unity Assets"
	@call :link /J "%dest%\Assets\Juniper\Scripts" "%juniper%Unity Scripts"
@exit /b

@rem ==== SUBROUTINES ====

@rem call :link (/H|/J) "<dest>" "<src>"
:link
	@setlocal enableextensions
	@set switch=%1
	@set dest=%~2
	@set src=%~3

	mklink %switch% "%dest%" "%src%"
	@if exist "%src%.meta" (mklink /h "%dest%.meta" "%src%.meta")
@exit /b