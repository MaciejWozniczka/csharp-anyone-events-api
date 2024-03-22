FROM mcr.microsoft.com/dotnet/sdk:6.0
LABEL author="maciej.wozniczka@outlook.com"

RUN apt-get update && \
    apt-get install -yq --no-install-recommends \ 
    libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 \ 
    libexpat1 libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 \ 
    libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 \ 
    libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 \ 
    libnss3 libgbm1
    
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
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