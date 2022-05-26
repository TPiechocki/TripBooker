FROM node:16.14.0-alpine as builder
WORKDIR /app

COPY package.json .
RUN apk add --update python3 make g++ && rm -rf /var/cache/apk/*
RUN npm update
RUN npm install

ARG WEB_API_URL
RUN echo WEB_API_URL=${WEB_API_URL} > .env.production

COPY . .
RUN npm run build

FROM nginx
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80

WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*

COPY --from=builder /app/public .
ENTRYPOINT ["nginx", "-g", "daemon off;"]