#!/bin/bash

rm -f ~/testoutput.log

echo "pong" > ~/testoutputres.log 2>&1 &
cat ~/testoutput.log
