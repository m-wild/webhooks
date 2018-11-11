$ErrorActionPreference = "stop"

## Example: ./scripts/loadtest -n 100 --rps 10


$api_endpoint = "http://localhost:500/api/orders"

loadtest -p $PSScriptRoot/loadtest-order.json $args $api_endpoint