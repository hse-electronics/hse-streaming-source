#!/bin/bash -e
echo "copying hse streaming datasource..."
command mkdir -p /var/lib/grafana/plugins/hse-streaming-datasource/ ; \cp -rfv /var/lib/hse/hse-streaming-datasource/* /var/lib/grafana/plugins/hse-streaming-datasource/
echo "run grafana..."
exec /run.sh "$@"
