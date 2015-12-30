; setup.nsi
;
!include WordFunc.nsh
!insertmacro VersionCompare
 
!include LogicLib.nsh
!include nsDialogs.nsh

!include MUI.nsh
;--------------------------------

!define WNDCLASS "wndclass_desked_gsk"

!ifndef VERSION
	!error "VERSION is not undefined"
!endif

; The name of the installer
Name "Atlassian Connector For Visual Studio"

LicenseData LICENSE

; The file to write
OutFile "plvs\bin\Release\plvs-setup.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Atlassian Connector For Visual Studio"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Atlassian\Plvs" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

Var Dialog
Var Label
Var CheckboxVS2008
Var CheckboxVS2008_State
Var CheckboxVS2010
Var CheckboxVS2010_State
Var CheckboxVS2011
Var CheckboxVS2011_State
Var CheckboxVS2013
Var CheckboxVS2013_State

Var FoundVS2008
Var FoundVS2010
Var FoundVS2011
Var FoundVS2013
; == 4 if we have all versions
Var FoundAll

Function .onInit
	Call CheckVS
	FindWindow $0 "${WNDCLASS}"
	${If} $0 != 0
	    MessageBox MB_ICONSTOP|MB_OK "Visual Studio is running. Close it and try again." /SD IDOK
		Abort
	${EndIf}	

	InitPluginsDir
	
FunctionEnd

Function un.onInit
	FindWindow $0 "${WNDCLASS}"
	${If} $0 != 0
	    MessageBox MB_ICONSTOP|MB_OK "Visual Studio is running. Close it and try again." /SD IDOK
		Abort
	${EndIf}	
FunctionEnd

Function CheckVS
    ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	${IfNot} ${Errors}
		IntOp $FoundVS2008 0 + 1
	${EndIf}	

    ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
	${IfNot} ${Errors}
		IntOp $FoundVS2010 0 + 1
	${EndIf}	

    ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
	${IfNot} ${Errors}
		IntOp $FoundVS2011 0 + 1
	${EndIf}	

    ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
	${IfNot} ${Errors}
		IntOp $FoundVS2013 0 + 1
	${EndIf}	
	
	IntOp $FoundAll $FoundVS2013 + $FoundVS2011
	IntOp $FoundAll $FoundAll + $FoundVS2010
	IntOp $FoundAll $FoundAll + $FoundVS2008

	${If} $FoundVS2008 <> 1
		${AndIf} $FoundVS2010 <> 1
			${AndIf} $FoundVS2011 <> 1
				${AndIf} $FoundVS2013 <> 1
					Call AbortOnNoVS
	${EndIf}		
FunctionEnd

Function AbortOnNoVS
	MessageBox MB_ICONSTOP|MB_OK "Visual Studio installation is not found. Setup will now close." /SD IDOK
	Abort
FunctionEnd

Function un.RemoveVs2010Files

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
 
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Ankh.ExtensionPoints.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Aga.Controls.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\edit.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ajax-loader.gif"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\nothing.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.pkgdef"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\extension.vsixmanifest"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ide_plugin_32.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\LICENSE"

	RMDir "$0\Extensions\Atlassian\Atlassian Connector"
 
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage"

	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\10.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
FunctionEnd

Function un.Unregister2008
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegValue HKLM "Software\Microsoft\VisualStudio\9.0\AutoLoadPackages\{ADFC4E64-0397-11D1-9F4E-00A0C911004F}" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" 
	DeleteRegValue HKLM "Software\Microsoft\VisualStudio\9.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}"
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
FunctionEnd

Function un.Unregister2011

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
 
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Ankh.ExtensionPoints.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Aga.Controls.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\edit.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ajax-loader.gif"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\nothing.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.pkgdef"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\extension.vsixmanifest"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ide_plugin_32.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\LICENSE"

	RMDir "$0\Extensions\Atlassian\Atlassian Connector"
 
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage"

	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
	
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0_Config\InstalledProducts\PlvsPackage"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
	DeleteRegValue HKCU "Software\Microsoft\VisualStudio\11.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\11.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}"

FunctionEnd

Function un.Unregister2013

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
 
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Ankh.ExtensionPoints.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\Aga.Controls.dll"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\edit.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ajax-loader.gif"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\nothing.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\plvs2010.pkgdef"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\extension.vsixmanifest"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\ide_plugin_32.png"
	Delete "$0\Extensions\Atlassian\Atlassian Connector\1.0\LICENSE"

	RMDir "$0\Extensions\Atlassian\Atlassian Connector"
 
	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage"

	DeleteRegKey HKLM "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
	
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0_Config\InstalledProducts\PlvsPackage"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"
	DeleteRegValue HKCU "Software\Microsoft\VisualStudio\12.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}"
	DeleteRegKey HKCU "Software\Microsoft\VisualStudio\12.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}"

FunctionEnd

Function LaunchVS
	IfSilent noLaunchVs

	${If} $FoundAll = 4
		${If} $CheckboxVS2013_State == ${BST_CHECKED} 
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $CheckboxVS2011_State == ${BST_CHECKED} 
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $CheckboxVS2010_State == ${BST_CHECKED} 
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $CheckboxVS2008_State == ${BST_CHECKED} 
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
			Exec "$0\devenv.exe"
		${EndIf}
	${Else}
		${If} $FoundVS2013 = 1
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $FoundVS2011 = 1
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $FoundVS2010 = 1
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
			Exec "$0\devenv.exe"
		${ElseIf} $FoundVS2008 = 1
			ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
			Exec "$0\devenv.exe"
		${EndIf}
	${Endif}	

	noLaunchVs:
FunctionEnd

; Pages

!define MUI_ICON plvs\Resources\icons\ide_plugin_32.ico
!define MUI_UNICON plvs\Resources\icons\ide_plugin_32.ico
!define MUI_WELCOMEFINISHPAGE_BITMAP "plvs\Resources\icons\atlassian-installer.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "plvs\Resources\icons\atlassian-installer.bmp"
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE LICENSE
Page custom nsComponentsPage nsComponentsPageLeave
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
 
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_CHECKED
!define MUI_FINISHPAGE_RUN_TEXT "Start Visual Studio"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchVS"
!define MUI_FINISHPAGE_NOAUTOCLOSE
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Function nsComponentsPage

	nsDialogs::Create 1018
	Pop $Dialog

	${If} $Dialog == error
		Abort
	${EndIf}
	
	${If} $FoundAll > 1
		
		${NSD_CreateLabel} 0 0 100% 24u "Setup has detected that multiple versions of Microsoft Visual Studio are installed. Please select versions that you want to integrate with"
		Pop $Label

		${If} $FoundVS2008 = 1
			${NSD_CreateCheckbox} 35u 32u 100% 10u "Visual Studio 200&8"
			Pop $CheckboxVS2008
			${NSD_Check} $CheckboxVS2008
			${If} $CheckboxVS2008_State == ${BST_UNCHECKED}
				${NSD_Uncheck} $CheckboxVS2008
			${EndIf}
		${EndIf}
	
		${If} $FoundVS2010 = 1
			${NSD_CreateCheckbox} 35u 42u 100% 10u "Visual Studio 201&0"
			Pop $CheckboxVS2010
			${NSD_Check} $CheckboxVS2010
			${If} $CheckboxVS2010_State == ${BST_UNCHECKED}
				${NSD_Uncheck} $CheckboxVS2010
			${EndIf}
		${EndIf}

		${If} $FoundVS2011 = 1
			${NSD_CreateCheckbox} 35u 52u 100% 10u "Visual Studio 201&2"
			Pop $CheckboxVS2011
			${NSD_Check} $CheckboxVS2011
			${If} $CheckboxVS2011_State == ${BST_UNCHECKED}
				${NSD_Uncheck} $CheckboxVS2011
			${EndIf}
		${EndIf}

		${If} $FoundVS2013 = 1
			${NSD_CreateCheckbox} 35u 62u 100% 10u "Visual Studio 201&3"
			Pop $CheckboxVS2013
			${NSD_Check} $CheckboxVS2013
			${If} $CheckboxVS2013_State == ${BST_UNCHECKED}
				${NSD_Uncheck} $CheckboxVS2013
			${EndIf}
		${EndIf}

		nsDialogs::Show
		
	${Else}

		Abort
		
	${EndIf}	

FunctionEnd

Function nsComponentsPageLeave

	${NSD_GetState} $CheckboxVS2008 $CheckboxVS2008_State
	${NSD_GetState} $CheckboxVS2010 $CheckboxVS2010_State
	${NSD_GetState} $CheckboxVS2011 $CheckboxVS2011_State
	${NSD_GetState} $CheckboxVS2013 $CheckboxVS2013_State

	${If} $CheckboxVS2013_State == ${BST_UNCHECKED}
		${AndIf} $CheckboxVS2011_State == ${BST_UNCHECKED}
			${AndIf} $CheckboxVS2010_State == ${BST_UNCHECKED}
				${AndIf} $CheckboxVS2008_State == ${BST_UNCHECKED}
					MessageBox MB_OK|MB_ICONSTOP "At least one version of Visual Studio has to be selected" /SD IDOK
					Abort
	${EndIf}		

FunctionEnd

Section "Connector Files"

	SectionIn RO
  
	; Set output path to the installation directory.
	SetOutPath $INSTDIR
  
	; Put files there
	File "plvs\bin\Release\plvs2008.dll"
	File "plvs\bin\Release\plvs2010.dll"
	File "plvs\bin\Release\Ankh.ExtensionPoints.dll"
	File "plvs\bin\Release\Aga.Controls.dll"
	File "plvs\third party\newtonsoft\Newtonsoft.Json.dll"
	File "plvs\bin\Release\edit.png"
	File "plvs\bin\Release\ajax-loader.gif"
	File "plvs\bin\Release\nothing.png"
	File "plvs\bin\Release\plvs2010.pkgdef"
	File "plvs\bin\Release\extension.vsixmanifest"
	File "plvs\Resources\icons\ide_plugin_32.png"
	File "plvs\LICENSE"
  
	; Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\Atlassian\Plvs "Install_Dir" "$INSTDIR"
  
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "DisplayName" "Atlassian Connector For Visual Studio"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "DisplayVersion" "${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "Publisher" "Atlassian"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs" "NoRepair" 1
	WriteUninstaller "uninstall.exe"
  
	IfSilent silentReg

	${If} $FoundVS2008 = 1
		${If} $FoundAll > 1 
		${AndIf} $CheckboxVS2008_State == ${BST_CHECKED} 
		${OrIf} $FoundAll = 1
			Call Integrate2008
		${EndIf}
	${EndIf}
	
	${If} $FoundVS2010 = 1
		${If} $FoundAll > 1
		${AndIf} $CheckboxVS2010_State == ${BST_CHECKED} 
		${OrIf} $FoundAll = 1
			Call Integrate2010
		${EndIf}
	${EndIf}	

	${If} $FoundVS2011 = 1
		${If} $FoundAll > 1
		${AndIf} $CheckboxVS2011_State == ${BST_CHECKED} 
		${OrIf} $FoundAll = 1
			Call Integrate2011
		${EndIf}
	${EndIf}
	
	${If} $FoundVS2013 = 1
		${If} $FoundAll > 1
		${AndIf} $CheckboxVS2013_State == ${BST_CHECKED} 
		${OrIf} $FoundAll = 1
			Call Integrate2013
		${EndIf}
	${EndIf}	
	
	Goto endFiles

	silentReg:
		${If} $FoundVS2008 = 1
			Call Integrate2008
		${EndIf}
		
		${If} $FoundVS2010 = 1
			Call Integrate2010
		${EndIf}	

		${If} $FoundVS2011 = 1
			Call Integrate2011
		${EndIf}	
		
		${If} $FoundVS2013 = 1
			Call Integrate2013
		${EndIf}	
		
	endFiles:
SectionEnd

Section "Start Menu Shortcuts"

	CreateDirectory "$SMPROGRAMS\Atlassian Connector For Visual Studio"
	CreateShortCut "$SMPROGRAMS\Atlassian Connector For Visual Studio\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
SectionEnd

Function Integrate2008

	ClearErrors

	${If} $FoundVS2008 = 1
		ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "" "#110"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "ProductDetails" "#112"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "PID" "1.0"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "LogoID" "#600"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\InstalledProducts\PlvsPackage" "UseInterface" 1
  
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "" "Atlassian.plvs.PlvsPackage, plvs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "InprocServer32" "$SYSDIR\mscoree.dll"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "Class" "Atlassian.plvs.PlvsPackage"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "CodeBase" "$INSTDIR\plvs2008.dll"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ID" 1
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "MinEdition" "Standard"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ProductVersion" "1.0"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "ProductName" "Atlassian Connector for Visual Studio"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Packages\{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" "CompanyName" "Atlassian"

		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\AutoLoadPackages\{ADFC4E64-0397-11D1-9F4E-00A0C911004F}" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" 0

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Menus" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}" ", 1000, 1"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Name" "Atlassian.plvs.AtlassianToolWindow"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Orientation" "Bottom"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "Style" "Tabbed"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}" "DontForceCreate" 1

		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{06c81945-10ef-4d72-8daf-32d29f7e9573}\Visibility" "{f1536ef8-92ec-443c-9ed7-fdadf150da82}" 0

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Name" "Atlassian.plvs.ui.jira.IssueDetailsToolWindow"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Orientation" "Bottom"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "Style" "Tabbed"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}" "DontForceCreate" 1

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}" "Name" "Atlassian.plvs.ui.bamboo.BuildDetailsToolWindow"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}" "Orientation" "Bottom"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}" "Style" "Tabbed"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}" "DontForceCreate" 1

		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{34218db5-88b7-4773-b356-c07e94987cd2}\Visibility" "{f1536ef8-92ec-443c-9ed7-fdadf150da82}" 0
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\9.0\ToolWindows\{F9624C15-E757-4582-BF55-F2DB8146681C}\Visibility" "{f1536ef8-92ec-443c-9ed7-fdadf150da82}" 0

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}" "" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Services\{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}" "Name" "JIRA Link Service"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "" "JIRA Links (Margin)"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "DisplayName" "JIRA Links (Margin)"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{658DDF58-FC14-4db9-8110-B52A6845B6CF}" "Service" "{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "" "JIRA Links (Text)"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "DisplayName" "JIRA Links (Text)"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\Text Editor\External Markers\{D7F03136-206D-4674-ADC7-DA0E9EE38869}" "Service" "{34D3D2C5-60CD-4d79-8BD8-7759EBB3C27A}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "" "Atlassian JIRA Connector"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "Service" "{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "" "#113"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\9.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "Package" "{36FA5F7F-2B5D-4CEC-8C06-10C483683A16}"

		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}
  
FunctionEnd

Function Integrate2010

	${If} $FoundVS2010 = 1
		ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
  
		SetOutPath "$0\Extensions\Atlassian\Atlassian Connector\1.0"
 
		File "plvs\bin\Release\plvs2010.dll"
		File "plvs\bin\Release\Ankh.ExtensionPoints.dll"
		File "plvs\bin\Release\Aga.Controls.dll"
		File "plvs\third party\newtonsoft\Newtonsoft.Json.dll"
		File "plvs\bin\Release\edit.png"
		File "plvs\bin\Release\ajax-loader.gif"
		File "plvs\bin\Release\nothing.png"
		File "plvs\bin\Release\plvs2010.pkgdef"
		File "plvs\bin\Release\extension.vsixmanifest"
		File "plvs\Resources\icons\ide_plugin_32.png"
		File "plvs\LICENSE"

		SetOutPath $INSTDIR  
  
		;; seems to be needed regardless of the vsixmanifest. Without this, logo does not show up in the VS About box
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "" "#110"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "ProductDetails" "#112"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "PID" "1.0"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "LogoID" "#600"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\10.0\InstalledProducts\PlvsPackage" "UseInterface" 1

		;; still required for AnkhSVN integration?
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "" "Atlassian JIRA Connector"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "Service" "{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "" "#113"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\10.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "Package" "{36FA5F7F-2B5D-4CEC-8C06-10C483683A16}"
	
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}

FunctionEnd

Function Integrate2011

	${If} $FoundVS2011 = 1
		ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
  
		SetOutPath "$0\Extensions\Atlassian\Atlassian Connector\1.0"
 
		File "plvs\bin\Release\plvs2010.dll"
		File "plvs\bin\Release\Ankh.ExtensionPoints.dll"
		File "plvs\bin\Release\Aga.Controls.dll"
		File "plvs\third party\newtonsoft\Newtonsoft.Json.dll"
		File "plvs\bin\Release\edit.png"
		File "plvs\bin\Release\ajax-loader.gif"
		File "plvs\bin\Release\nothing.png"
		File "plvs\bin\Release\plvs2010.pkgdef"
		File "plvs\bin\Release\extension.vsixmanifest"
		File "plvs\Resources\icons\ide_plugin_32.png"
		File "plvs\LICENSE"

		SetOutPath $INSTDIR  
  
		;; seems to be needed regardless of the vsixmanifest. Without this, logo does not show up in the VS About box
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "" "#110"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "ProductDetails" "#112"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "PID" "1.0"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "LogoID" "#600"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\11.0\InstalledProducts\PlvsPackage" "UseInterface" 1

		;; still required for AnkhSVN integration?
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "" "Atlassian JIRA Connector"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "Service" "{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "" "#113"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\11.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "Package" "{36FA5F7F-2B5D-4CEC-8C06-10C483683A16}"
	
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}

FunctionEnd

Function Integrate2013

	${If} $FoundVS2013 = 1
		ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
  
		SetOutPath "$0\Extensions\Atlassian\Atlassian Connector\1.0"
 
		File "plvs\bin\Release\plvs2010.dll"
		File "plvs\bin\Release\Ankh.ExtensionPoints.dll"
		File "plvs\bin\Release\Aga.Controls.dll"
		File "plvs\third party\newtonsoft\Newtonsoft.Json.dll"
		File "plvs\bin\Release\edit.png"
		File "plvs\bin\Release\ajax-loader.gif"
		File "plvs\bin\Release\nothing.png"
		File "plvs\bin\Release\plvs2010.pkgdef"
		File "plvs\bin\Release\extension.vsixmanifest"
		File "plvs\Resources\icons\ide_plugin_32.png"
		File "plvs\LICENSE"

		SetOutPath $INSTDIR  
  
		;; seems to be needed regardless of the vsixmanifest. Without this, logo does not show up in the VS About box
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "" "#110"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "Package" "{36fa5f7f-2b5d-4cec-8c06-10c483683a16}"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "ProductDetails" "#112"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "PID" "1.0"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "LogoID" "#600"
		WriteRegDWORD HKLM "Software\Microsoft\VisualStudio\12.0\InstalledProducts\PlvsPackage" "UseInterface" 1

		;; still required for AnkhSVN integration?
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "" "Atlassian JIRA Connector"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}" "Service" "{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}"

		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "" "#113"
		WriteRegStr HKLM "Software\Microsoft\VisualStudio\12.0\IssueRepositoryConnectors\{F6D2F9E0-0B03-42F2-A4BF-A3E4E0019685}\Name" "Package" "{36FA5F7F-2B5D-4CEC-8C06-10C483683A16}"
	
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}

FunctionEnd

; Uninstaller
;
; FIXME: 
; right now the uninstaller blindly tries to uninstall VS2008, VS2010, VS2012 and VS2013 integrations,
; regardless of what integration options were selected during the installation
;
Section "Uninstall"
  
	; Remove registry keys
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Plvs"
	DeleteRegKey HKLM SOFTWARE\Plvs
  
	; Remove files and uninstaller
	Delete $INSTDIR\plvs2008.dll
	Delete $INSTDIR\plvs2010.dll
	Delete $INSTDIR\Ankh.ExtensionPoints.dll
	Delete $INSTDIR\Aga.Controls.dll
	Delete $INSTDIR\Newtonsoft.Json.dll
	Delete $INSTDIR\edit.png
	Delete $INSTDIR\ajax-loader.gif
	Delete $INSTDIR\nothing.png
	Delete $INSTDIR\plvs2010.pkgdef
	Delete $INSTDIR\extension.vsixmanifest
	Delete $INSTDIR\LICENSE
  
	Delete $INSTDIR\uninstall.exe

	; Remove shortcuts, if any
	Delete "$SMPROGRAMS\Atlassian Connector For Visual Studio\*.*"

	; Remove directories used
	RMDir "$SMPROGRAMS\Atlassian Connector For Visual Studio"
	RMDir "$INSTDIR"

	ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\9.0" "InstallDir"
	${IfNot} ${Errors}
		Call un.Unregister2008
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}
	
	ClearErrors

	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\10.0" "InstallDir"
	${IfNot} ${Errors}
		Call un.RemoveVs2010Files
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}	
	
	ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\11.0" "InstallDir"
	${IfNot} ${Errors}
		Call un.Unregister2011
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}	
	
	ClearErrors
	
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\VisualStudio\12.0" "InstallDir"
	${IfNot} ${Errors}
		Call un.Unregister2013
		ExecWait '"$0\devenv.exe" /setup'
	${EndIf}	
	
	ClearErrors

SectionEnd
