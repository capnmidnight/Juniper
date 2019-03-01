printf "\n# Source-Lines of Code\n\n##Juniper.Units\n\n\`\`\`\n" > Documentation/README-parts/sloc.md
scc --avg-wage 110000 Juniper.Units/src/ >> Documentation/README-parts/sloc.md
printf "\`\`\`\n" >> Documentation/README-parts/sloc.md

printf "\n##Juniper\n\n\`\`\`\n" > Documentation/README-parts/sloc.md
scc --avg-wage 110000 Assets/Juniper/ >> Documentation/README-parts/sloc.md
printf "\`\`\`\n" >> Documentation/README-parts/sloc.md

cat \
    Documentation/README-parts/intro.md \
    Documentation/README-parts/sloc.md \
    Documentation/README-parts/setup.md \
    Documentation/README-parts/dev.md \
> README.md

markdown-pdf README.md