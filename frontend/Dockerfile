FROM node:16.14.0-alpine as builder
WORKDIR /app

COPY package.json .
RUN npm update
RUN npm install

ARG WEB_API_URL
RUN echo WEB_API_URL=${WEB_API_URL} > .env.production

COPY . .
RUN npm run build

FROM nginx
EXPOSE 80
COPY --from=builder /app/public /usr/share/nginx/html