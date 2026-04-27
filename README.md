# ukdata

A site containing various radio data tools developed by Tom M0LTE

## Docker

Build and push a multi-arch image for the Pi deployment:

```powershell
docker buildx build --platform linux/amd64,linux/arm64 -t m0lte/radiodata-ui:latest -t m0lte/radiodata-ui:<tag> --push .
```

The live host currently updates by pulling from Docker Hub on `ubuntu@services-pi-1`:

```sh
cd /home/ubuntu/radiodata-ui
sudo docker-compose pull
sudo docker-compose up -d
```
