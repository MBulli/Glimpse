
; TODO autohotkey group for classes
#IfWinActive ahk_class CabinetWClass ; Explorer
#IfWinActive ahk_class Progman       ; Desktop
#space::
WinGet, Hwnd
Run "Glimpse.exe" %Hwnd%
return


Escape::
if Process,Exist,"Glimpse.exe"
	Run Notepad
return

#IfWinActive