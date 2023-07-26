#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# Start with the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Copy the project files to the container's working directory
COPY . ./
# Build the application
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Create a new stage for the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the compiled files from the build stage to the runtime stage
COPY --from=build /app/out ./

# Expose the port that your API will listen on (replace 80 with your actual port if needed)
EXPOSE 80
EXPOSE 443

# Start the API when the container is run
ENTRYPOINT ["dotnet", "Public_API.dll"]


# docker.io/majidmashhadi/artapi:v1

# docker build . -t majidmashhadi/artapi:v1 && docker push  majidmashhadi/artapi:v1
# pod=$(kubectl get pods | awk '/'"$partial_name"'/ {print $1}') && kubectl delete pod "$pod"
# docker push  majidmashhadi/artapi:v1
# docker run -p 8083:80 -e ASPNETCORE_URLS=http://+:80 majidmashhadi/artapi:v1


# pod=$"kubectl get pods -n your-namespace | awk '/dotnet-api/ {print $1}'"

#partial_name="dotnet-api"
#pod=$(kubectl get pods | awk '/'"$partial_name"'/ {print $1}')
#echo "$pod"