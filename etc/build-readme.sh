printf "\n# Source-Lines of Code\n" > README-parts/sloc.md

printf "\n## Juniper\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c --avg-wage 110000 ../src/Juniper/Assets/Juniper/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

printf "\n## Juniper.Units\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c --avg-wage 110000 ../src/Juniper.Units/src/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

printf "\n## Juniper.NeuroSky\n" >> README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c --avg-wage 110000 ../src/Juniper.NeuroSky/src >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

cat \
    README-parts/intro.md \
    README-parts/sloc.md \
    README-parts/setup.md \
    README-parts/dev.md \
> ../README.md