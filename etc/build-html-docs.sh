rm -rf ../docs
sed -e "s/GENERATE_XML           = YES/GENERATE_XML           = NO/" doxygen.config > doxygen.html.config
doxygen doxygen.html.config
rm doxygen.html.config
cp ../*.png ../docs/
cp ../*.jpg ../docs/