FROM mcr.microsoft.com/devcontainers/dotnet:10.0

ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=1000

RUN apt-get update \
    && CURRENT_UID="$(id -u ${USERNAME})" \
    && CURRENT_GID="$(id -g ${USERNAME})" \
    && if [ "${CURRENT_GID}" != "${USER_GID}" ]; then groupmod --gid "${USER_GID}" "${USERNAME}"; fi \
    && if [ "${CURRENT_UID}" != "${USER_UID}" ]; then usermod --uid "${USER_UID}" --gid "${USER_GID}" "${USERNAME}"; fi \
    && chown -R "${USER_UID}:${USER_GID}" "/home/${USERNAME}" \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get install -y --no-install-recommends protobuf-compiler \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
