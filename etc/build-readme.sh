printf "# Code Metrics\n" > README-parts/sloc.md
printf "\n\`\`\`\n" >> README-parts/sloc.md
./run-metrics.sh >> README-parts/sloc.md
printf "\n\`\`\`\n\n" >> README-parts/sloc.md

cat \
    README-parts/intro.md \
    README-parts/setup.md \
    README-parts/dev.md \
    README-parts/sloc.md \
	> ../README.md