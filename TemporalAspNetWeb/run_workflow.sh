#!/bin/sh

i=0
limit=100

while [ "$i" -lt "$limit" ]; do
  echo "Iteration number: $((i + 1))"
  temporal workflow start --task-queue test-queue --type TestWorkflow >> ./run_output.txt
  i=$(($i + 1))
done