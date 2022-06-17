@echo OFF
pushd Source
pushd CopyRouteMod

dotnet build --nologo --verbosity quiet

popd
popd