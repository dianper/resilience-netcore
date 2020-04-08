# Resilience .NETCore

A simple POC using [Polly](https://github.com/App-vNext/Polly) to deal with resilience.

* Timeout
* Retry
* Circuit Breaker
* Fallback

## Running without API
With Powershell run `dotnet run` into `ResilienceSample` folder.

```sh
Now listening on: http://localhost:5001
```

Request `http://localhost:5001/api/values` on browser and look at the logs on Powershell. It will try to connect on `http://localhost:5880/api/products/:productId`

* It will try 3 request with an interval of 2 seconds each before open the circuit and will return fallback result.

Logs:

```
11:58:52 - Error: [ProductClient.GetAsync]: No connection could be made because the target machine actively refused it
11:58:52 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 1
11:58:54 - Error: [ProductClient.GetAsync]: No connection could be made because the target machine actively refused it
11:58:54 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 2
11:58:56 - Error: [ProductClient.GetAsync]: No connection could be made because the target machine actively refused it
11:58:56 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 3
11:58:56 - Failed! Circuit open, waiting: 00:00:30
11:58:56 - Fallback method used due to: The circuit is now open and is not allowing calls.
```

* Until 30 seconds, if you request again the logs will be the same and the service will not call the WebApi `http://localhost:5880/api/products/:productId`
* After 30 seconds, if you request again, the circuit state will be `half open` and will try to request again

Logs:

```
12:36:02 - Circuit is half open.
12:36:04 - Error: [ProductClient.GetAsync]: No connection could be made because the target machine actively refused it
12:36:04 - Failed! Circuit open, waiting: 00:00:30
12:36:04 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 1
12:36:06 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 1
12:36:08 - Request failed. Waiting 00:00:02 before next retry. Retry attempt 1
12:36:10 - Fallback method used due to: The circuit is now open and is not allowing calls.
```

* When WebApi is back, the circuit is reseted and the service works properly and return expected result

Logs:

```
12:40:10 - Circuit is reset.
```

## Running with API

With Powershell run `dotnet run` into `WebApi` folder.

```sh
Now listening on: http://localhost:5880
Application started. Press Ctrl+C to shut down.
```

Using a new instance of Powershell, run `dotnet run` into `ResilienceSample` folder.

```sh
Now listening on: http://localhost:5001
```

Request `http://localhost:5001/api/values` on browser and it will return the expect result from `http://localhost:5880/api/products/:productId`

```json
{
    "id": 123456,
    "name": "Product"
}
```