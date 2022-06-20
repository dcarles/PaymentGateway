FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# copy csproj and restore as distinct layers

COPY ./src/PaymentGateway.BankProcessor.Simulator/PaymentGateway.BankProcessor.Simulator.csproj ./PaymentGateway.BankProcessor.Simulator/

RUN dotnet restore PaymentGateway.BankProcessor.Simulator/PaymentGateway.BankProcessor.Simulator.csproj

# copy everything else and build
COPY ./src/ ./
WORKDIR /PaymentGateway.BankProcessor.Simulator/
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
WORKDIR /app
COPY --from=build /PaymentGateway.BankProcessor.Simulator/out ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "PaymentGateway.BankProcessor.Simulator.dll"]