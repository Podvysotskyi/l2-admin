FROM node:24-bookworm-slim

WORKDIR /workspace
COPY . .

EXPOSE 3002
CMD ["sh", "-c", "npm ci && npm run dev"]
