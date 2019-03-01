rm -rf Documentation/xml/*
sed -e "s/GENERATE_HTML          = YES/GENERATE_HTML          = NO/" doxygen.config > doxygen.xml.config
doxygen doxygen.xml.config
rm doxygen.xml.config

printf "# Documentation Coverage\n\n\`\`\`\n" > Documentation/markdown/coverage.md
./doxy-coverage.py Documentation/xml/ >> Documentation/markdown/coverage.md
printf "\`\`\`\n" >> Documentation/markdown/coverage.md

rm -rf Documentation/xml/