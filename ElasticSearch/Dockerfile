FROM microsoft/windowsservercore  AS downloader
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]


ENV ES_VERSION="6.2.2" \
    ES_HOME="C:\elasticsearch" \
    ES_DATA="C:\data" \
    ES_LOGS="C:\elasticsearch\logs" \
    JAVA_HOME="C:\jdk1.8.0_162"

RUN $env:PATH = $env:JAVA_HOME + '\bin;' + $env:PATH; \
    [Environment]::SetEnvironmentVariable('PATH', $env:PATH, [EnvironmentVariableTarget]::Machine); \
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; \
    Invoke-WebRequest "https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-$($env:ES_VERSION).zip.sha512" -OutFile 'elasticsearch.zip.sha512' -UseBasicParsing; \
    Invoke-WebRequest "https://artifacts.elastic.co/downloads/elasticsearch/elasticsearch-$($env:ES_VERSION).zip" -OutFile 'elasticsearch.zip' -UseBasicParsing; \
    $env:ES_SHA512 = Get-Content -Raw elasticsearch.zip.sha512; \
    $env:ES_HASH512 = (Get-FileHash elasticsearch.zip -Algorithm SHA512).Hash; \
    Write-Output $env:ES_SHA512 \
    Write-Output $env:ES_HASH512 \
    if ($env:ES_HASH512 -ne $env:ES_SHA512) {exit 1}; \
    Expand-Archive elasticsearch.zip -DestinationPath C:\ ; \
    Move-Item c:/elasticsearch-$($env:ES_VERSION) $env:ES_HOME; \
    mkdir c:\data; \
    Remove-Item elasticsearch.zip; \
    Remove-Item elasticsearch.zip.sha512; 

COPY jdk1.8.0_162 ./jdk1.8.0_162
WORKDIR $ES_HOME
COPY config ./config

VOLUME $ES_DATA

EXPOSE 9200 9300
SHELL ["cmd", "/S", "/C"]
CMD ".\bin\elasticsearch.bat"

HEALTHCHECK --interval=5s \
CMD powershell -command \
    try { \
     $content = (iwr -useb http://localhost:9200/_cat/health).Content; \
     $health = $content.Split(' ')[3]; \
     if ($health -eq 'green' -or $health -eq 'yellow') { return 0 } \
     else { return 1 }; \
    } catch { return 1 }