FROM mhart/alpine-node:12 AS node-base

COPY plugin/package.json /app/package.json
COPY plugin/yarn.lock /app/yarn.lock

RUN mkdir /app/build
WORKDIR /app
RUN yarn install

FROM node-base AS plugin-build

COPY plugin/ /app/
RUN yarn dev
#enable this for prod
#RUN yarn build 


FROM grafana/grafana:7.4.3 AS grafana
RUN mkdir /var/lib/grafana/plugins/hse-streaming-datasource/
COPY --from=plugin-build /app/dist/ /var/lib/hse/hse-streaming-datasource
COPY grafana/provisioning/ /etc/grafana/provisioning/
ENV GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH=/var/lib/grafana/dashboards/test-dashboard.json
#this is a little bit stupid but as we want the grafana.db to be in the volume
#and the grafana.db is in the same directory as the plugin folder
#we have to copy the new plugin on the start of the container so it wont be
#overwritten by an old version already in the volume.
COPY grafana/run-and-copy.sh /run-and-copy.sh
USER 0
ENTRYPOINT ["/run-and-copy.sh"]
#this does not work :/
#COPY --from=plugin-build /app/dist/ /usr/share/grafana/public/app/plugins/datasource/hse-streaming-datasource
#COPY --from=plugin-build /app/dist/ /var/lib/grafana-hse/hse/