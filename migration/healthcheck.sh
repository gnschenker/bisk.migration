#! /bin/bash

FILE=/app/ready
echo "Checking if file $FILE exists..."
if [ ! -f "$FILE" ]; then
    echo "File $FILE does not exist!"
    exit -1
fi

