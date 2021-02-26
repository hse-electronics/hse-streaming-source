#!/bin/bash -e
echo "acc"
command mkdir /var/lib/grafana/plugins/hse-streaming-datasource/ || cp -rfv /var/lib/hse/hse-streaming-datasource/* /var/lib/grafana/plugins/hse-streaming-datasource/
echo "bcc"
exec /run.sh "$@"