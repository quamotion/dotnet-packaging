if [ ! -z `docker images "$IMAGE_NAME" -q` ]; then
    docker rmi -f $IMAGE_NAME:latest
fi