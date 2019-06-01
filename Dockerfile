FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
WORKDIR /app
ARG AWS_ACCESS_KEY_ID
ARG AWS_SECRET_ACCESS_KEY
ENV AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID
ENV AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY
ENV SSM_PARAMETER_PATH="/testing"
ENV DEFAULT_AWS_REGION="us-east-1"

COPY . .

RUN dotnet restore && dotnet test ./SsmIntegrationTest/SsmIntegrationTest.csproj