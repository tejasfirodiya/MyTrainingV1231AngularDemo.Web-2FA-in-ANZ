# COMMON PATHS
echo "#################### COMMON PATHS ####################"

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webHostFolder = Join-Path $slnFolder "src/MyTrainingV1231AngularDemo.Web.Host"
$webPublicFolder = Join-Path $slnFolder "src/MyTrainingV1231AngularDemo.Web.Public"
$ngFolder = Join-Path $buildFolder "../../angular"

## CLEAR ######################################################################
echo "#################### CLEAR ####################"

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################
echo "#################### RESTORE NUGET PACKAGES ####################"

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB HOST PROJECT ###################################################
echo "#################### PUBLISH WEB HOST PROJECT ####################"

Set-Location $webHostFolder
dotnet publish --output (Join-Path $outputFolder "Host") --configuration Release
Copy-Item ("Dockerfile.original") (Join-Path $outputFolder "Host")

## COPY YML AND PFX FILES HOST ##############################################
echo "#################### COPY YML AND PFX FILES HOST ####################"

Set-Location $outputFolder
Copy-Item ("../host/*.yml") (Join-Path $outputFolder "Host")

## PUBLISH WEB PUBLIC PROJECT ###################################################
echo "#################### PUBLISH WEB PUBLIC PROJECT ####################"

Set-Location $webPublicFolder
yarn
yarn run build
dotnet publish --output (Join-Path $outputFolder "Public") --configuration Release
Copy-Item ("Dockerfile.original") (Join-Path $outputFolder "Public")

## COPY YML AND PFX FILES PUBLIC ##############################################
echo "#################### COPY YML AND PFX FILES PUBLIC ####################"
Set-Location $outputFolder
Copy-Item ("../public/*.yml") (Join-Path $outputFolder "Public")

## PUBLISH ANGULAR UI PROJECT #################################################
echo "#################### PUBLISH ANGULAR UI PROJECT ####################"
Set-Location $ngFolder
yarn
ng build --configuration production
Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/dist") -Recurse
Copy-Item (Join-Path $ngFolder "Dockerfile") (Join-Path $outputFolder "ng")

## COPY YML AND PFX FILES ANGULAR ##############################################
echo "#################### COPY YML AND PFX FILES ANGULAR ####################"
Set-Location $outputFolder
Copy-Item ("../ng/*.*") (Join-Path $outputFolder "ng")

## CREATE DOCKER IMAGES #######################################################
echo "#################### CREATE DOCKER IMAGES ####################"

# Mvc
echo "#################### CREATE DOCKER IMAGES (MVC) ####################"
Set-Location (Join-Path $outputFolder "Host")
Remove-Item ("Dockerfile")
Rename-Item -Path "Dockerfile.original" -NewName "Dockerfile"
dotnet dev-certs https -v -ep aspnetzero-devcert-host.pfx -p 2825e4d9-5cef-4373-bed3-d7ebf59de216

docker rmi mycompanynameabpzerotemplatewebhost -f
docker compose -f docker-compose.yml build

# Public
echo "#################### CREATE DOCKER IMAGES (Public) ####################"
Set-Location (Join-Path $outputFolder "Public")
Remove-Item ("Dockerfile")
Rename-Item -Path "Dockerfile.original" -NewName "Dockerfile"
dotnet dev-certs https -v -ep aspnetzero-devcert-public.pfx -p b7ca126d-5085-47a0-8ac3-1b5971bd65a1

docker rmi mycompanynameabpzerotemplatewebpublic -f
docker compose -f docker-compose.yml build

# Angular
echo "#################### CREATE DOCKER IMAGES (Angular) ####################"
Set-Location (Join-Path $outputFolder "ng")

docker rmi mycompanynameabpzerotemplatewebangular -f
docker compose -f docker-compose.yml build

## FINALIZE ###################################################################
echo "#################### FINALIZE ####################"
Set-Location $outputFolder