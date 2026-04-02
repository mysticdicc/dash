#!/usr/bin/env sh
set -e

CERT_PATH="${TLS_CERT_PATH:-/app/data/certs/tls.pfx}"
CERT_PASSWORD="${TLS_CERT_PASSWORD:-}"
CERT_PASSWORD_FILE="${TLS_CERT_PASSWORD_FILE:-/app/data/certs/tls.pfx.pass}"

if [ -z "$CERT_PASSWORD" ] && [ -f "$CERT_PASSWORD_FILE" ]; then
  CERT_PASSWORD="$(cat "$CERT_PASSWORD_FILE")"
fi

if [ -f "$CERT_PATH" ]; then
  export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:8080;https://+:8443}"
  export ASPNETCORE_Kestrel__Certificates__Default__Path="$CERT_PATH"
  export ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASSWORD"

  if [ -n "$CERT_PASSWORD" ]; then
    echo "TLS cert found at '$CERT_PATH' -> HTTPS enabled on :8443 (password from env/file)"
  else
    echo "TLS cert found at '$CERT_PATH' -> HTTPS enabled on :8443 (empty password)"
  fi
else
  export ASPNETCORE_URLS="${ASPNETCORE_URLS:-http://+:8080}"
  echo "No TLS cert at '$CERT_PATH' -> HTTP only on :8080"
fi

exec dotnet web.dll