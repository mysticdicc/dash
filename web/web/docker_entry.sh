#!/usr/bin/env sh
set -e

CERT_PATH="${TLS_CERT_PATH:-/app/data/certs/tls.pfx}"
CERT_PASSWORD="${TLS_CERT_PASSWORD:-}"

if [ -f "$CERT_PATH" ]; then
  # Enable both HTTP and HTTPS
  export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:8080;https://+:8443}"
  export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
  export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"

  echo "TLS cert found at '$CERT_PATH' -> HTTPS enabled on :8443"
else
  # HTTP only
  export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:8080}"
  echo "No TLS cert at '$CERT_PATH' -> HTTP only on :8080"
fi

exec dotnet web.dll