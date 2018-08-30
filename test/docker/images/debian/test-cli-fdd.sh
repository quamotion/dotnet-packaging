#!/bin/bash

# this script assumes that we already run as root
# we're installing with apt so that we get all the dependencies installed, too

rm -f ~/testoutput.log

echo installing /tests/packages/$1 > ~/testoutput.log 2>&1

apt-get update -y > /dev/nul
pushd /tests/packages/
dpkg -i $1 > /dev/nul
apt-get install -y -f > /dev/nul
popd

ls -a /usr/share/clifdd >> ~/testoutput.log 2>&1
ls -a /etc/clifdd >> ~/testoutput.log 2>&1
ls -a ~/.clifdd >> ~/testoutput.log 2>&1

cat ~/testoutput.log