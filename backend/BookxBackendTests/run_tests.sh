#!/bin/zsh

while read key_value; do
  export $key_value
done < default.env

dotnet test
