# Stage 1: Build
FROM node:18.14.2 AS build
WORKDIR /app

# Copy package.json and install dependencies
COPY package.json package-lock.json ./
RUN npm ci

# Copy the rest of the frontend source code
COPY . .

# Build the frontend
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Remove default Nginx static files
RUN rm -rf ./*

# Copy built frontend from build stage
COPY --from=build /app/dist ./

# Expose port for frontend
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
