printf "\n# Code Metrics\n" > README-parts/sloc.md

printf "\n## Juniper\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c -i cs,shader --avg-wage 110000  ../src/Juniper.Common/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

printf "\n## Juniper.Unity\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c -i cs,shader --avg-wage 110000  ../src/Juniper/Assets/Juniper/Scripts >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

printf "\n## Juniper.Units\n" >> README-parts/sloc.md
printf "\n[project site](https://capnmidnight.github.io/Juniper.Units)\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c -i cs --avg-wage 110000 ../src/Juniper.Units/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

printf "\n## Juniper.NeuroSky\n" >> README-parts/sloc.md
printf "\n[project site](https://capnmidnight.github.io/Juniper.NeuroSky)\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c -i cs --avg-wage 110000 ../src/Juniper.NeuroSky/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

cat \
    README-parts/intro.md \
    README-parts/setup.md \
    README-parts/dev.md \
    README-parts/sloc.md \
	> ../README.md