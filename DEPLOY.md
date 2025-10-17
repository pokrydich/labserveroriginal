# Deployment Guide

# I On build host

## 1 Build backend docker image

### 1.1 build
```
sudo docker compose build labserver
```
### 1.2 export
```
sudo docker save labserver -o labserver.tar
sudo chown <username>:<usergroup> labserver.tar
```

## 2 Prepare database schema file

### 2.1 Drop data and restart PostgreSQL DB container
```
sudo docker compose stop db
sudo docker compose down db
sudo docker compose up -d db
```

### 2.2 Create schema via dotnet ef toolset

#### 2.2.1 Delete migrations
```
rm -r ./Server/Migrations
```

#### 2.2.2 Create new migration
```
cd ./Server
dotnet ef migrations add init
```

#### 2.2.3 Apply migrations to database
```
cd ./Server
dotnet ef database update
```

### 2.3 Export database schema
```
sudo docker exec -i labserver-db-1 /bin/bash -c "PGPASSWORD=<DB_CONTAINER_PWD_ON_BUILD_HOST> pg_dump --username postgres labs" > labsdbschema.sql
```

## 3 Build frontend bundles

### 3.1 Clear old bundles
```
rm -r ./nginx/sites/admin/browser/
rm ./nginx/sites/admin/3rdpartylicenses.txt
rm -r ./nginx/sites/dashboard/browser/
rm ./nginx/sites/dashboard/3rdpartylicenses.txt
```

### 3.2 Build admin portal
```
cd ./web/admin
ng build --output-path ../../nginx/sites/admin/
```

### 3.3 Build student dashboard portal
```
cd ./web/dashboard
ng build --output-path ../../nginx/sites/dashboard/
```

## 4 Create deployment package
```
tar -czf labserver_deployment.tar.gz labserver.tar labsdbschema.sql compose.yml appsettings.docker.json.template nginx/
```

# Debug deploy

## copy appsettings.docker.json.template to Server folder

```bash
cp appsettings.docker.json.template ./Server/appsettings.docker.json
```

Configure required parameters in appsettings.docker.json (**note:** DB configuration for PostgreSQL should be the same as in compose.yml)

## Prepare domain names

Domain names: labserver.local, git.labserver.local, admin.labserver.local, my.labserver.local should point to the IP of labserver dev stand.

E.g. it can be configured via hosts file on Linux and Windows

```
10.10.10.10 labserver.local
10.10.10.10 git.labserver.local
...
```

## Configure GitLab

1. Startup gitlab container

```
docker compose up gitlab
```

2. Wait for gitlab initialization (check via http://git.labserver.local)

3. Get root password from the container

```bash
docker exec -it gitlab grep "Password:" /etc/gitlab/initial_root_password
```

4. Login to GitLab

5. Change root password

6. Generate access token for root user

7. Enter GitLab access token into appsettings.docker.json configuration (gitlab section)

## Start up LabServer

1. Start all containers:

```bash
docker compose up -d
```

2. Connect to LabServer admin panel - http://admin.labserver.local

3. Register first user via registration form (/register)

4. Login via registered user

5. Add all roles via users menu

6. Relogin

# II On deploy host

Tested on Ubuntu Server 24.04

## 0.1 Install docker

[Guide](https://docs.docker.com/engine/install/ubuntu/)

## 0.2 Upload deployment package (created on build host)

Upload and unpack deployment package in the target installation directory.

*NOTE*: Further steps require that deployment host user is added to docker group.

## 1 Import labserver docker image
```
docker load -i labserver.tar
```

## 2 Import database schema

### 2.0 Change postgres user password
Change password in: compose.yml (db container environment options)

### 2.1 Start database docker container
```
docker compose up -d db
```

### 2.2 Import schema
```
docker exec -i labserver-db-1 /bin/bash -c "PASSWORD=<DEPLOYMENT_HOST_DB_CONTAINER_PWD> psql --username postgres labs" < ./labsdbschema.sql
```

## 3 Initial LabServer configuration

### 3.1 Move appsettings.docker.json.template to required path
```
mkdir Server
cp appsettings.docker.json.template ./Server/appsettings.docker.json
```

### 3.2 Prepare appsettings.docker.json.template file

Change all required fields except for GitLab API token (it will be generated later).
```
vi ./Server/appsettings.docker.json
```

## 4 Issue letsencrypt cert (via certbot)

*NOTE*: DNS records should be configured to point to deployment host.

[Guide](https://phoenixnap.com/kb/letsencrypt-docker)

### 4.0 Select letsencrypt Nginx configuration
```
cp ./nginx/conf/app.conf.letsencrypt ./nginx/conf/app.conf
```

### 4.1 Start Nginx container (for letsencrypt checks)
```
docker compose up -d webserver
```

### 4.2 Check via certbot dry-run
```
docker compose run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ --dry-run -d [domain-name] -d admin.[domain-name] -d git.[domain-name] -d my.[domain-name]
```
E.g.:
```
docker compose run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ --dry-run -d ooplabs.ru -d admin.ooplabs.ru -d git.ooplabs.ru -d my.ooplabs.ru
```

### 4.3 Issue certs after successful dry run
```
docker compose run --rm certbot certonly --webroot --webroot-path /var/www/certbot/ -d [domain-name] -d admin.[domain-name] -d git.[domain-name] -d my.[domain-name]
```

## 5 Install GitLab container

### 5.1 Prepare directories for GitLab docker
This step is required because gitlab container will be launched on the next step
to configure certs via letsencrypt.

[Guide](https://docs.gitlab.com/ee/install/docker/installation.html)
*NOTE*: Change paths in gitlab contianer config in *compose.yml*.

```
sudo mkdir /srv/gitlab; sudo mkdir /srv/gitlab/config; sudo mkdir /srv/gitlab/logs; sudo mkdir /srv/gitlab/data
```

### 5.2 Start gitlab container
```
docker compose up -d gitlab
```

### 5.3 Wait for root user generation
Wait...


## 6 Start Nginx with production config

### 6.1 Switch to full nginx config
```
cp ./nginx/conf/app.conf.full ./nginx/conf/app.conf
```

### 6.2 Restart Nginx with required containers
```
docker compose stop webserver
docker compose up -d labserver gitlab webserver
```


## 7 GitLab configuration

### 7.1 Fetch gitlab root password
```
docker exec -it gitlab grep 'Password:' /etc/gitlab/initial_root_password
```
### 7.2 Login to root admin panel
Login via fetched password on http://git.[domain_name].

### 7.3 Create admin user for LabServer
#### 7.3.1 Create user
Create user with email **labsadmin@ooplabs.ru**

#### 7.3.2 Create auth token for LabServer user
Save auth token to ./Server/appsettings.json

### 7.4 Deactivate sign-in


## 8 Restart containers

```
docker compose stop
docker compose up -d
```