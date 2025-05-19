#!/bin/bash

# Build the Front.API project
echo "Building UIOMatic.Front.API..."
dotnet build ../UIOMatic.Front.API/UIOMatic.Front.API.csproj -c Release

# Build the SPA project
echo "Building UIOMatic.SPA..."
cd ../UIOMatic.SPA
npm install
npm run build
cd ../build

# Create NuGet packages
echo "Creating NuGet packages..."
nuget pack UIOMatic.Front.API.nuspec -OutputDirectory ./packages
nuget pack UIOMatic.SPA.nuspec -OutputDirectory ./packages

echo "Packages created in ./packages directory" 