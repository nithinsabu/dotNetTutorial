Download the images as:
docker pull ghcr.io/nithinsabu/react-app:latest
docker pull ghcr.io/nithinsabu/dotnet-api:latest
docker pull ghcr.io/nithinsabu/fastapi-api:latest
docker pull mongo

Create a docker network:
docker network create drf-app

Run the images:
docker run -p 5173:80 --name frontend ghcr.io/nithinsabu/react-app  (frontend at http://localhost:5173 in local machine)
docker run --network drf-app --name fastapi ghcr.io/nithinsabu/fastapi-api (runs at http://fastapi:80 in network)
docker run --network drf-app --name db mongo (runs at mongodb://db:27017 in network)
docker run --name backend --network drf-app -p 5253:8080 ghcr.io/nithinsabu/dotnet-api dotnet WebAPI.dll -e ConnectionStrings:MongoDB=mongodb://db:27017 ConnectionStrings:FastAPI=http://fastapi:80 (Runs at http://localhost:5253 in local machine and http://backend:8080 in network)