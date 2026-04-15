# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# העתקת קובץ הפרויקט וביצוע Restore
COPY *.csproj ./
RUN dotnet restore

# העתקת שאר הקבצים ובניית הפרויקט
COPY . ./
RUN dotnet publish -c Release -o out

# שלב ההרצה
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# הגדרת הפורט ש-Render מצפה לו
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "TodoApi.dll"]