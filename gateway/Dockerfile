# syntax=docker/dockerfile:1
FROM nginx:1.31.7-alpine

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
