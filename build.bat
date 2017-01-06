cls
mkdir .\bin

copy .\lib\parca.dll .\bin\parca.dll

csc ./src/core/*.cs /target:library /out:./bin/aquabot.dll ^
	/r:System.Drawing.dll ^
	/r:./lib/parca.dll ^
	/r:./lib/Contest.Core.dll ^
	/r:System.Windows.Forms.dll

csc ./src/app/*.cs /target:exe /out:./bin/aqua.exe ^
	/r:System.Windows.Forms.dll ^
	/r:./lib/parca.dll ^
	/r:./bin/aquabot.dll

csc ./src/tests/*.cs /target:library /out:./bin/aquatests.dll ^
	/r:./lib/Contest.Core.dll ^
	/r:./lib/parca.dll ^
	/r:./bin/aquabot.dll ^
	/r:System.Windows.Forms.dll

