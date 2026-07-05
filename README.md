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

## Deployment

The `radiodata-ui` app is hosted on a Proxmox LXC (`ham-apps`) as a Docker container.

- **Build:** `docker buildx build --platform linux/amd64 -t m0lte/radiodata-ui .`
- **Run:** `docker run -d --restart unless-stopped -p 8532:8080 \`
  `-e ETCC_CACHE_PATH=/data/etcc-systems.json -v /opt/radiodata-ui-data:/data m0lte/radiodata-ui`
- **Data:** UK repeater data via `ukrepeaterlib`; no database.
- **Public:** `data.m0lte.uk`, via a Cloudflare tunnel → `localhost:8532`.

### Upstream resilience

Repeater data comes from the ETCC API (`api-beta.rsgb.online`). `EtccRepository` caches the
last-good response so the site keeps working through upstream outages:

- serves the in-memory snapshot while fresh; refreshes are single-flighted;
- falls back to stale data when the upstream is unreachable, and returns a clean **503**
  (with `Retry-After`) only when there is no cached data at all — never a 100s hang;
- a background service refreshes every 10 minutes, so the site recovers automatically when
  the upstream returns;
- if `ETCC_CACHE_PATH` points at a writable file (mount a volume, as above), the last-good
  snapshot is persisted and reloaded on restart. The container runs as UID `1654`, so the
  host directory must be writable by it (`chown 1654:1654 /opt/radiodata-ui-data`).
  Persistence is best-effort: if the path is unwritable the app still runs from memory.
