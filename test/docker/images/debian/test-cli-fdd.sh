#!/bin/bash

rm -f ~/testoutput.log

echo "pong" > ~/testoutput.log 2>&1 &
cat ~/testoutput.log
