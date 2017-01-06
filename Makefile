#make -s (to disable echo)
build: ./src/**/*.cs
	clear
	mkdir -p ./bin

	cp ./lib/parca.dll ./bin/parca.dll
	
	mcs ./src/core/*.cs /target:library /out:./bin/aquabot.dll \
		/r:System.Drawing \
		/r:./lib/parca.dll \
		/r:./lib/Contest.Core.dll \
		/r:System.Windows.Forms
	
	mcs ./src/app/*.cs /target:exe /out:./bin/aqua.exe \
		/r:System.Windows.Forms \
		/r:./lib/parca.dll \
		/r:./bin/aquabot.dll

	if [ -a ./src/tests/*.cs ]; then \
		mcs ./src/tests/*.cs /target:library /out:./bin/aquatests.dll \
		/r:./lib/Contest.Core.dll \
		/r:./lib/parca.dll \
		/r:./bin/aquabot.dll \
		/r:System.Windows.Forms; \
	fi;

test:
	mono ./lib/contest.exe run -nh ./bin/aquatests.dll
