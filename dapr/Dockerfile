#See https://endjin.com/blog/2022/11/adventures-in-dapr-ep04

FROM tianon/true

#COPY components/email.yml.compose /components/email.yml
#COPY components/statestore.yml.compose /components/statestore.yml
#COPY components/secrets-file.yml.compose /components/secrets-file.yml

#COPY components/entrycam.yml /components
#COPY components/exitcam.yml /components
#COPY components/pubsub.yml /components
#COPY components/secrets-envvars.yml /components
#COPY components/secrets-keyvault.yml /components
#COPY components/secrets.json /components

COPY config/config.yml.compose /config/config.yml
#COPY config/ratelimit-config.yml.compose /config/ratelimit-config.yml

VOLUME /components
VOLUME /config