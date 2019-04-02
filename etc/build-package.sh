rm ../docs/Juniper.unitypackage
cd ../src/Juniper
"$UNITY_PATH\Unity" -quit -batchmode -nographics \
	  -projectPath . \
	  -exportPackage Assets/Juniper \
	  ../../docs/Juniper.unitypackage
