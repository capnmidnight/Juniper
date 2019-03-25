printf "\n# Code Metrics\n" > README-parts/sloc.md

printf "\n\`\`\`\n" >> README-parts/sloc.md
scc -c -i cs --avg-wage 110000 ../src/ >> README-parts/sloc.md
printf "\`\`\`\n" >> README-parts/sloc.md

cat \
    README-parts/intro.md \
    README-parts/sloc.md \
> ../README.md