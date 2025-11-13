# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1️⃣ Copy project file into correct folder
COPY BOTAI/BOTAI.csproj BOTAI/

# 2️⃣ Restore
RUN dotnet restore "BOTAI/BOTAI.csproj"

# 3️⃣ Copy ALL source files keeping structure
COPY BOTAI ./BOTAI

# 4️⃣ Publish output
RUN dotnet publish "BOTAI/BOTAI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Copy SQL scripts correctly
RUN mkdir -p /app/publish/Data/Scripts
RUN cp -r BOTAI/Data/Scripts/* /app/publish/Data/Scripts/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8000
ENV ASPNETCORE_URLS=http://+:8000
ENTRYPOINT ["dotnet", "BOTAI.dll"]
