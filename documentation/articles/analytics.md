# Adriva Analytics
Adriva analytics library is designed to support multiple analytics clients with multiple analytics servers. Out-of-the-box implementation supports Microsoft AppInsights both on client and server side, which means you can use Microsoft AppInsights under the hood and still persist analytics data in your own custom repository.

## Libraries

|Name|Description|
|-|-|
|Adriva.Extensions.Analytics.AppInsights|Provides extension methods and wrapper classes to host Microsoft AppInsights in your generic or asp&period;core application that is capable of working with ```Ariva.Extensions.Analytics.Server.AppInsights``` library.|
|Adriva.Extensions.Analytics.Server|Provides a generic analytics server implementation that is configurable to parse and persist virtually *any* incoming analytics data.|
|Adriva.Extensions.Analytics.Server.AppInsights|Provides handler and parser classes that can be used with ```Adriva.Extensions.Analytics.Server``` library, to convert all incoming Microsoft AppInsights analytics data to a common format.|