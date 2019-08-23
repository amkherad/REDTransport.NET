# REDTransport.NET

| Linux Build (Travis-CI) | Code Coverage |
|-------------------------|---------------|
| [![Build Status](https://travis-ci.com/amkherad/ShellScript.svg?branch=master)](https://travis-ci.com/amkherad/ShellScript) | [![Coverage Status](https://coveralls.io/repos/github/amkherad/ShellScript/badge.svg?branch=master)](https://coveralls.io/github/amkherad/ShellScript?branch=master) |

[Work In Progress]  
Full featured library to handle data transportation in any architecture (client/server, p2p, ...)   
Some of REDTransport's features are:

* Request aggregation (Pack multiple request/responses into one request/response), with advanced chaining options.
* Connection yielding*, we call it Yield-Connection.
* You can enable GraphQL/OData services so your API supports these protocols without writing a single line of code.
* Crud controller, you can enable auto crud management for you entities.
* Local cache support.
* Advanced query features.
* Extensible. you can do anything in RED's pipeline.

\* Request yielding means that the endpoints can close the connection and notify the result with pushing technologies so the users (API caller) can never know that request has been closed.

## Installing

All of the packages are available on NuGet.org.

### Base Package
```
dotnet add REDTransport.NET
```

### ASP.net Core Server
```
dotnet add REDTransport.NET.Server.AspNet //core
dotnet add REDTransport.NET.Server.AspNet.Crud //auto crud managers
dotnet add REDTransport.NET.Server.AspNet.OData //OData subsystem
dotnet add REDTransport.NET.Server.AspNet.GraphQL //GraphQL subsystem
```

### SignalR
```
dotnet add REDTransport.NET.SignalR //core
dotnet add REDTransport.NET.SignalR.Server //server
dotnet add REDTransport.NET.SignalR.Client //client
```

### Client (dotnet core)
```
dotnet add REDTransport.NET.RESTClient
```


## Getting Started

The first rule of the library is to be transparent, so you must code everything!  
You can wrap your API behind REDTransport so you can have all of it's features


## Contributors
* [Ali Mousavi Kherad](https://github.com/amkherad)

Built with love from Iran  
Part of gamered.ir's toolchain.