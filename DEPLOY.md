# Deployment Guide

# I On build host

## 1 Build backend docker image

### 1.1 build
```bash
sudo docker compose build labserver
```
## 2 Prepare database schema file

### 2.1 Drop data and restart PostgreSQL DB container
```bash
sudo docker compose stop db
sudo docker compose down db
sudo docker compose up -d db
```

Проверить доступ к базе данных с хоста:
```bash
export PGPASSWORD=123456
psql -h localhost -p 5432 -U postgres -d labs -c '\conninfo'
```
### 2.2 Create schema via dotnet ef toolset

#### 2.2.1 Delete migrations
```bash
rm -r ./Server/Migrations
```

#### 2.2.2 Create new migration

```bash
cd ./Server
dotnet new tool-manifest  # если файла .config/dotnet-tools.json ещё нет
dotnet tool install dotnet-ef
dotnet ef migrations add init
```
#### 2.2.3 Apply migrations to database

```
cd ./Server
dotnet ef database update
```

Если не работает команда выше (смотрит только в `appsettings.json` файл, а не в `appsettings.docker.json`), тогда использовать:
```
dotnet ef database update --connection "Host=localhost;Port=5432;Database=labs;Username=postgres;Password=123456"
или
dotnet ef database update --connection "Host=127.0.0.1;Port=5432;Database=labs;Username=postgres;Password=123456"
```

Проверка миграции
```bash
sudo docker compose exec db psql -U postgres -d labs -c "\dt"
```


### 2.3 Export database schema (так то не нужно)
```
sudo docker exec -i labserver-db-1 /bin/bash -c "PGPASSWORD=<DB_CONTAINER_PWD_ON_BUILD_HOST> pg_dump --username postgres labs" > labsdbschema.sql
```

```bash
# Смотрим docker compose ps
# labserveroriginal-main-db-1

docker exec -i labserveroriginal-main-db-1 /bin/bash -c "PGPASSWORD=123456 pg_dump --username postgres labs" > labsdbschema.sql
```

## 3 Build frontend bundles (для админ панели)

### 3.1 Clear old bundles
```bash
rm -r ./nginx/sites/admin/browser/
rm ./nginx/sites/admin/3rdpartylicenses.txt
rm -r ./nginx/sites/dashboard/browser/
rm ./nginx/sites/dashboard/3rdpartylicenses.txt
```

### 3.2 Build admin portal
```bash
cd ./web/admin
npm install
npm install -g @angular/cli
ng build --output-path ../../nginx/sites/admin/
```

### 3.3 Build student dashboard portal
```bash
cd ./web/dashboard
npm install
ng build --output-path ../../nginx/sites/dashboard/
```

## 4 Create deployment package (не нужно)
```bash
tar -czf labserver_deployment.tar.gz labserver.tar labsdbschema.sql compose.yml appsettings.docker.json.template nginx/
```

# Debug deploy

## copy appsettings.docker.json.template to Server folder (достаточно подменить файлы, которые скинул)

```bash
cp appsettings.docker.json.template ./Server/appsettings.docker.json
```

Configure required parameters in appsettings.docker.json (**note:** DB configuration for PostgreSQL should be the same as in compose.yml)

## Prepare domain names

Domain names: labserver.local, git.labserver.local, admin.labserver.local, my.labserver.local should point to the IP of labserver dev stand.

E.g. it can be configured via hosts file on Linux and Windows

```bash
# Linux
/etc/hosts
# На Windows отредоктировать файл
C:\Windows\System32\drivers\etc\hosts
```

Добавить строки:
```
10.10.10.10 labserver.local
10.10.10.10 git.labserver.local
10.10.10.10 admin.labserver.local
10.10.10.10 my.labserver.local
```
Где вместо 10.10.10.10 ip адреса из:
```bash
sudo docker compose ps -q | xargs -n 1 sudo docker inspect -f '{{.Name}} - {{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}'
```

`
## Configure GitLab

1. Startup gitlab container

```bash
sudo docker compose up gitlab
```

2. Wait for gitlab initialization (check via http://git.labserver.local)

3. Get root password from the container

```bash
sudo docker exec -it gitlab grep "Password:" /etc/gitlab/initial_root_password
```

4. Login to GitLab

5. Change root password (пример пароля)
```password
C~GvEZDC1rB6u~ji
```
5. Generate access token for root user (пример токена)
```
glpat-Rj-jgPtMvy5kiN6NMcok
```

6. Enter GitLab access token into `appsettings.docker.json` configuration (gitlab section)

---
## Для пущей уверенности работы следующего пункта

```bash
sudo docker compose restart labserver webserver
```
## Start up LabServer

1. Start all containers:
```bash
sudo docker compose up -d
```
2. Connect to LabServer admin panel - http://admin.labserver.local
3. Register first user via registration form (/register)
4. Login via registered user
5. Add all roles via users menu
6. Relogin


---