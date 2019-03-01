./generate-docs.sh
rm -rf Documentation/html Documentation/xml
git checkout -- Documentation/html Documentation/xml
tail Documentation/coverage.txt
