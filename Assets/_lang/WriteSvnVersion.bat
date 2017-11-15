@cd /d %~dp0
::@cd../..
@set svn_bin="C:\Program Files (x86)\Subversion\bin"
@%svn_bin%\svnversion.exe
::@pause