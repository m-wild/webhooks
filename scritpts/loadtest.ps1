$ErrorActionPreference = "stop"

## Example: ./scripts/loadtest -n 100 --rps 10


$api_endpoint = "http://localhost:5000/api/orders"

loadtest -p $PSScriptRoot/loadtest-order.json -T "application/json" $args $api_endpoint