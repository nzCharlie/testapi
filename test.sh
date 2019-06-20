#! /bin/bash

curl -k -X POST -d '@test.json' https://localhost:5001/api/total -H "Content-Type: application/json"