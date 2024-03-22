FROM mcr.microsoft.com/dotnet/sdk:6.0
LABEL author="maciej.wozniczka@outlook.com"

RUN apt-get update
RUN apt-get install -yq --no-install-recommends
    
RUN apt-get curl 
RUN apt-get install -y libpng-dev 
RUN apt-get libjpeg-dev 
RUN apt-get libxi6 
RUN apt-get build-essential 
RUN apt-get libgl1-mesa-glx

RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

COPY . /app
WORKDIR /app
RUN dotnet build -c Release
RUN dotnet publish MobileApp.Host -c Release

# WORKDIR /app/frontend
# RUN npm install
# RUN npm run build

WORKDIR /app
RUN mkdir -p MobileApp.Host/bin/Release/net6.0/publish/ClientApp
RUN mkdir -p MobileApp.Host/bin/Release/net6.0/publish/ClientApp/dist
# RUN cp -a frontend/dist/. MobileApp.Host/bin/Release/net6.0/publish/ClientApp/dist

WORKDIR /app/MobileApp.Host/bin/Release/net6.0/publish
CMD ./MobileApp.Host