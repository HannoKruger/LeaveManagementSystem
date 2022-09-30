FROM node:18.9.0

WORKDIR /app

COPY package*.json ./

RUN npm install
# If you are building your code for production
# RUN npm ci --only=production


COPY . .


EXPOSE 3000

CMD ["node", "server.cjs"]