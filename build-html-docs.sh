sed -e "s/GENERATE_XML           = YES/GENERATE_XML           = NO/" doxygen.config > doxygen.html.config
doxygen doxygen.html.config
rm doxygen.html.config
cp *.png Documentation/html/
cp *.jpg Documentation/html/

rm -rf Documentation/xml

cd Documentation/html
if [ -x "$(command -v zip)" ]; then
	zip -r9 ../../Assets/Juniper/Documentation.zip *
elif [ -x "$(command -v 7z)" ]; then
	7z a ../../Assets/Juniper/Documentation.zip *
fi