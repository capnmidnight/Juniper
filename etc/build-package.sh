cd ../src
rm ../docs/Juniper.unitypackage
Unity -quit -batchmode -nographics \
	  -projectPath . \
	  -exportPackage Assets/Juniper \
	  Juniper.unitypackage
mv Juniper.unitypackage ../docs/
