rm -rf xml/
cat doxygen.config \
	| sed -e "s/GENERATE_XML           = NO/GENERATE_XML           = YES/" \
	> doxygen.xml.config
doxygen doxygen.xml.config
rm doxygen.xml.config

printf "# Documentation Coverage\n\n\`\`\`\n" > markdown/coverage.md
./doxy-coverage.py xml/ >> markdown/coverage.md
printf "\`\`\`\n" >> markdown/coverage.md

rm -rf xml/