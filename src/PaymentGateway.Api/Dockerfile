FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# copy csproj and restore as distinct layers

COPY ./src/PaymentGateway.Api/PaymentGateway.Api.csproj ./PaymentGateway.Api/
COPY ./src/PaymentGateway.PaymentsCore/PaymentGateway.PaymentsCore.csproj ./PaymentGateway.PaymentsCore/
COPY ./src/PaymentGateway.BankProcessor/PaymentGateway.BankProcessor.csproj ./PaymentGateway.BankProcessor/
COPY ./src/PaymentGateway.Data/PaymentGateway.Data.csproj ./PaymentGateway.Data/

RUN dotnet restore PaymentGateway.Api/PaymentGateway.Api.csproj

# copy everything else and build
COPY ./src/ ./
WORKDIR /PaymentGateway.Api/
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

 ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
WORKDIR /app
COPY --from=build /PaymentGateway.Api/out ./

EXPOSE 55000
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]