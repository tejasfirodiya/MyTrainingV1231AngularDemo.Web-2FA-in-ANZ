# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webMvcFolder = Join-Path $slnFolder "src/MyTrainingV1231AngularDemo.Web.Mvc"
$webPublicFolder = Join-Path $slnFolder "src/MyTrainingV1231AngularDemo.Web.Public"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore MyTrainingV1231AngularDemo.Web.sln

## PUBLISH WEB MVC PROJECT ####################################################

Set-Location $webMvcFolder
dotnet publish --output (Join-Path $outputFolder "Mvc") --configuration Release
yarn
yarn run build
Copy-Item ("Dockerfile.original") (Join-Path $outputFolder "Mvc")

## COPY YML AND PFX FIKES PUBLIC ##############################################
Set-Location $outputFolder
Copy-Item ("../mvc/*.yml") (Join-Path $outputFolder "Mvc")

## PUBLISH WEB PUBLIC PROJECT #################################################

Set-Location $webPublicFolder
yarn
yarn run build
dotnet publish --output (Join-Path $outputFolder "Public") --configuration Release
Copy-Item ("Dockerfile.original") (Join-Path $outputFolder "Public")

## COPY YML AND PFX FIKES PUBLIC ##############################################
Set-Location $outputFolder
Copy-Item ("../public/*.yml") (Join-Path $outputFolder "Public")

## CREATE DOCKER IMAGES #######################################################

# Mvc
Set-Location (Join-Path $outputFolder "Mvc")
Remove-Item ("Dockerfile")
Rename-Item -Path "Dockerfile.original" -NewName "Dockerfile"
dotnet dev-certs https -v -ep aspnetzero-devcert-mvc.pfx -p 2825e4d9-5cef-4373-bed3-d7ebf59de216

docker rmi mycompanynameabpzerotemplatewebmvc -f
docker compose -f docker-compose.yml build

## CREATE DOCKER IMAGES #######################################################

# Public
Set-Location (Join-Path $outputFolder "Public")
Remove-Item ("Dockerfile")
Rename-Item -Path "Dockerfile.original" -NewName "Dockerfile"
dotnet dev-certs https -v -ep aspnetzero-devcert-public.pfx -p b7ca126d-5085-47a0-8ac3-1b5971bd65a1

docker rmi mycompanynameabpzerotemplatewebpublic -f
docker compose -f docker-compose.yml build

## FINALIZE ###################################################################

Set-Location $outputFolder