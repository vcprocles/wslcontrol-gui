dotnet publish -c Release -r win-x64 -p:SelfContained=false -p:PublishSingleFile=true -p:EnableTrim=true -p:DebugType=None -p:PublishReadyToRun=true -p:DebugSymbols=false     
dotnet publish -c Release -r win-arm64 -p:SelfContained=false -p:PublishSingleFile=true -p:EnableTrim=true -p:DebugType=None -p:PublishReadyToRun=true -p:DebugSymbols=false     
