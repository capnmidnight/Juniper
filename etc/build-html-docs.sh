rm -rf ../docs/*

cat doxygen.config \
	| sed -e "s/GENERATE_HTML          = NO/GENERATE_HTML          = YES/" \
	> doxygen.html.config
doxygen doxygen.html.config
rm doxygen.html.config

cp *.css ../docs/

cp ../*.png ../docs/
cp ../*.jpg ../docs/