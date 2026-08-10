# syntax=docker/dockerfile:1.7
FROM node:24-bookworm-slim

WORKDIR /workspace
COPY . .
RUN npm ci

EXPOSE 3002
CMD ["npm", "run", "dev"]
