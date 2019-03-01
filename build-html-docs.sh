sed -e "s/GENERATE_XML           = YES/GENERATE_XML           = NO/" doxygen.config > doxygen.html.config
doxygen doxygen.html.config
rm doxygen.html.config
cp *.png Documentation/html/
cp *.jpg Documentation/html/