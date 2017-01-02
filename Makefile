#make -s (to disable echo)
build: ./src/**/*.cs
	clear
	mkdir -p ./bin

	cp ./lib/atropos.dll ./bin/atropos.dll
	
	mcs ./src/core/*.cs /target:library /out:./bin/aquaforms.dll \
		/r:System.Drawing \
		/r:./lib/atropos.dll \
		/r:./lib/Contest.Core.dll \
		/r:System.Windows.Forms
	
	mcs ./src/*.cs /target:exe /out:./bin/aqua.exe \
		/r:System.Windows.Forms \
		/r:./lib/atropos.dll \
		/r:./bin/aquaforms.dll

	if [ -a ./src/tests/*.cs ]; then \
		mcs ./src/tests/*.cs /target:library /out:./bin/aquatests.dll \
		/r:./lib/Contest.Core.dll \
		/r:./lib/atropos.dll \
		/r:./bin/aquaforms.dll \
		/r:System.Windows.Forms; \
	fi;

test:
	mono ./lib/contest.exe run -nh ./bin/aquatests.dll
