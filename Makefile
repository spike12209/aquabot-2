#make -s (to disable echo)
build: ./src/**/*.cs
	mkdir -p ./bin
	mcs ./src/core/*.cs /target:library /out:./bin/aquaforms.dll \
		/r:System.Windows.Forms
	mcs ./src/*.cs /target:exe /out:./bin/aquaforms.exe \
		/r:System.Windows.Forms \
		/r:./bin/aquaforms.dll
	if [ -a ./src/tests/*.cs ]; then \
		mcs ./src/tests/*.cs /target:library /out:./bin/aquatests.dll \
		/r:./lib/Contest.Core.dll \
		/r:./bin/atropos.dll; \
	fi;

test:
	mono ./lib/contest.exe run -nh ./bin/aquatests.dll
