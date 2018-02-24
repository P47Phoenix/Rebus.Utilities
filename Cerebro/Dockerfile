FROM microsoft/windowsservercore AS downloader
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

#USER root
ENV VERSION="0.7.2" \
    C_HOME="C:\cerebro" \
    JAVA_HOME="C:\jdk1.8.0_162"

RUN [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; \
    Invoke-WebRequest "https://github.com/lmenezes/cerebro/releases/download/v$($env:VERSION)/cerebro-$($env:VERSION).zip" -OutFile cerebro.zip  -UseBasicParsing; \
    Expand-Archive cerebro.zip -DestinationPath C:\ ; \
    Move-Item "c:\cerebro-$($env:VERSION)" "c:\cerebro"; \
    Remove-Item cerebro.zip;

COPY jdk1.8.0_162 ./jdk1.8.0_162

WORKDIR $C_HOME
EXPOSE 9000
CMD ".\bin\cerebro.bat"