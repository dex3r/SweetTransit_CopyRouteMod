@echo OFF
call build.bat

pushd bin
pushd CLIUtils
pushd netcoreapp3.1

CLIUtils.exe %1

popd
popd
popd