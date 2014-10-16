#IfWinActive ahk_class CabinetWClass
;#IfWinActive Desktop
#space::
WinGet, Hwnd
Run "Glimpse.exe" %Hwnd%
return


Escape::
if Process,Exist,"Glimpse.exe"
	Run Notepad
return

#IfWinActive