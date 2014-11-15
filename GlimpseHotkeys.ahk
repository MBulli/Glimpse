
GroupAdd targets, ahk_class CabinetWClass ; Explorer
GroupAdd targets, ahk_class Progman       ; Desktop

#IfWinActive ahk_group targets
#space::
WinGet, Hwnd
Run "Glimpse.exe" %Hwnd%
return

Escape::
pid := PID("Glimpse.exe")
if %pid% <> 0
	WinClose ahk_pid %pid%
return

#IfWinActive

PID(Name) {
	Process,Exist,%Name%
	return Errorlevel
}
