#!/bin/bash
set -eu -o pipefail

dotnet restore /code/Test.SimpleKit.Infrastructure.Bus.RabbitMq.csproj
dotnet test /code/Test.SimpleKit.Infrastructure.Bus.RabbitMq.csproj