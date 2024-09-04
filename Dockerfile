FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS base

COPY . .

RUN dotnet build
RUN dotnet test

FROM base AS auth-build
RUN dotnet publish auth.webapp -o /out-auth


FROM base AS webapp-build
RUN dotnet publish webapi -o /out-webapi


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.20 AS auth
COPY --from=auth-build ./out-auth/ .

CMD [ "dotnet", "auth.webapp.dll" ]


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.20 AS webapi
COPY --from=webapp-build ./out-webapi/ .

CMD [ "dotnet", "webapi.dll" ]