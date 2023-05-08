Unicode True
; Custom defines
  !define NAME "SPOSVIET-PLUGIN"
  !define APPFILE "SposVietPluginKySo.exe"
  !define VERSION "1.0.0"
  !define SLUG "${NAME} v${VERSION}"
  ;--------------------------------
;--------------------------------
!define UNINST_KEY \
  "Software\Microsoft\Windows\CurrentVersion\Uninstall\SposVietPluginKySo"
!define MULTIUSER_INSTALLMODE_DEFAULT_REGISTRY_VALUENAME "CurrentUser"
!define MULTIUSER_INSTALLMODE_COMMANDLINE
!define MULTIUSER_MUI
!define MultiUser.InstallMode
!define MULTIUSER_INSTALLMODE_INSTDIR "$(^NAME)"

  !include "MUI2.nsh"
  !include "logiclib.nsh"
  !include "FileFunc.nsh"
;--------------------------------

; General

  Name "${NAME}"
  OutFile "${NAME}Setup.exe"
  InstallDir "$PROGRAMFILES\${NAME}"
  InstallDirRegKey HKLM "Software\${NAME}" ""
  RequestExecutionLevel admin
  ;--------------------------------
; UI
  
  !define MUI_ICON "assets\favicon.ico"
  !define MUI_HEADERIMAGE
  !define MUI_WELCOMEFINISHPAGE_BITMAP "assets\welcome.bmp"
  !define MUI_HEADERIMAGE_BITMAP "assets\head.bmp"
  !define MUI_ABORTWARNING
  !define MUI_WELCOMEPAGE_TITLE "${SLUG} Setup"
  ;--------------------------------
  ;
; Pages
  
  ; Installer pages
  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "license.txt"
  ;!insertmacro MUI_PAGE_LICENSE "$(license)"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN "$INSTDIR\SposVietPluginKySo.exe"
  !insertmacro MUI_PAGE_FINISH

  ; Uninstaller pages
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
  ; Set UI language
 
  !insertmacro MUI_LANGUAGE "Vietnamese"
  ;!insertmacro MUI_LANGUAGE "English"
  ;--------------------------------
  


Function onMultiUserModeChanged
${If} $MultiUser.InstallMode == "CurrentUser"
    StrCpy $InstDir "$LocalAppdata\Programs\${MULTIUSER_INSTALLMODE_INSTDIR}"
${EndIf}
FunctionEnd
  
;--------------------------------
; Section - Install App
;https://nsis.sourceforge.io/Add_uninstall_information_to_Add/Remove_Programs#With_a_MultiUser_Installer
  Section "-hidden app"
    SectionIn RO
    SetOutPath "$INSTDIR"
    File /r "app\*.*" 
    WriteRegStr HKLM "Software\${NAME}" "" $INSTDIR 
    ;WriteUninstaller "$INSTDIR\Uninstall.exe"
	WriteRegStr HKLM "${UNINST_KEY}" "DisplayName" "SposViet Plugin"
	;WriteRegStr HKLM "${UNINST_KEY}" "DisplayIcon" "${INSTDIR}\\favicon.ico"
	WriteRegStr HKLM "${UNINST_KEY}" "Publisher" "SPOSVIET-PLUGIN"
	WriteRegStr HKLM "${UNINST_KEY}" "DisplayVersion" "${VERSION}"
	WriteRegStr HKLM "${UNINST_KEY}" "UninstallString" \
		"$\"$INSTDIR\uninstall.exe$\" /$MultiUser.InstallMode"
	WriteRegStr HKLM "${UNINST_KEY}" $MultiUser.InstallMode 1 
	WriteRegStr HKEY_LOCAL_MACHINE "${UNINST_KEY}" "QuietUninstallString" \
		"$\"$INSTDIR\uninstall.exe$\" /$MultiUser.InstallMode /S"
	WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" \
"SposVietPluginKySo" "$INSTDIR\SposVietPluginKySo.exe"
	;WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Run" \
		;"SposVietKySo" "$InstDir\SposVietPluginKySo.exe"
 ; MessageBox MB_OK "Plugin SPOSVIET-PLUGIN sẽ khởi động cùng window"
	WriteUninstaller $INSTDIR\uninstall.exe
  SectionEnd
Section

SectionEnd
  ;-------------khởi động cùng win-------------------
  ;khởi động cùng win
 ; !ifndef BUILD_UNINSTALLER
 ; Function AddToStartup
  ;  CreateShortCut "$SMSTARTUP\AppName.lnk" "$INSTDIR\AppName.exe" ""
 ; FunctionEnd

  ; Using the read me setting as an easy way to add an add to startup option
 ; !define MUI_FINISHPAGE_SHOWREADME
  ;!define MUI_FINISHPAGE_SHOWREADME_TEXT "Run at startup"
 ; !define MUI_FINISHPAGE_SHOWREADME_FUNCTION AddToStartup
;!endif
 ; ----------end khởi động cùng win---------------
; Section - Shortcut

  Section "Desktop Shortcut" DeskShort
    CreateShortCut "$DESKTOP\${NAME}.lnk" "$INSTDIR\${APPFILE}"
 CreateShortcut "$SMPROGRAMS\${NAME}.lnk" "$INSTDIR\${APPFILE}"
  SectionEnd
  ;--------------------------------
; Descriptions

  ;Language strings
  ;LangString DESC_DeskShort ${LANG_ENGLISH} "Create Shortcut on Dekstop."
  LangString DESC_DeskShort ${LANG_Vietnamese} "Tạo thư mục ngoài Deskop."
  LangString PAGE_INSTALL_TYPE_SUBTITLE ${LANG_Vietnamese} "Choose installation type."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${DeskShort} $(DESC_DeskShort)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END
  ;--------------------------------
; Remove empty parent directories

  Function un.RMDirUP
    !define RMDirUP '!insertmacro RMDirUPCall'

    !macro RMDirUPCall _PATH
          push '${_PATH}'
          Call un.RMDirUP
    !macroend

    ; $0 - current folder
    ClearErrors

    Exch $0
    ;DetailPrint "ASDF - $0\.."
    RMDir "$0\.."

    IfErrors Skip
    ${RMDirUP} "$0\.."
    Skip:

    Pop $0

  FunctionEnd
  ;--------------------------------
; Section - Uninstaller

!macro customUnInstall
  Delete "$SMSTARTUP\AppName.lnk"
!macroend
Section "Uninstall"

  ;Delete Shortcut
  Delete "$DESKTOP\${NAME}.lnk"
  # second, remove the link from the start menu
  Delete "$SMPROGRAMS\${NAME}.lnk"

  ;Delete Uninstall
  Delete "$INSTDIR\Uninstall.exe"

  ;Delete Folder
  RMDir /r "$INSTDIR"
  ${RMDirUP} "$INSTDIR"

  DeleteRegKey /ifempty HKLM "Software\${NAME}"
  DeleteRegKey HKLM "${UNINST_KEY}"
SectionEnd