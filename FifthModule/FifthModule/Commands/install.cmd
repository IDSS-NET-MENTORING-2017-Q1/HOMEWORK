for %%A in (..\ScannerManager.exe) DO set P=%%~fA
sc create StreamingScannerService binPath="%P%"
