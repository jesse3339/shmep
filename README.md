# SH Medical Equipment Processor

A .NET 9 docker ready background service for processing medical equipment deliveries.

While this application could be converted to a console application, like the initial draft, this application would be better suited for production and more scalable if it were to be notification => queue based from any external system that may be updating the status of the order. A webhook or endpoint to trigger the processing of orders would also serve as a better implementation compared to a watcher/polling service.

Listed below are some production changes I would implement if we were to roll out the Hosted Service version to production.

# Production Changes

## Retries on API failures
To increase reliability of this service, we would need a more graceful way to handle failed requests. A retry mechanism using a package like Polly would be suitable for this.

## Queue-based or Pub/Sub Hosted Service, Webhook or Servless Function
If sticking with a hosted background service, having a queue of incoming order updates would help improve scalability and reliability of this application. Using a poller we run into a few issues that limit us from deploying more than one instance of this processor. If using a Queue, we could subscribe to some external events with a package like MassTransit.

We could also use an Azure Function or AWS Lambda on a queue and trigger a one time run of the processor when an order update comes through. Alternatively, we could use webhooks, or add an endpoint to this application, exposing it to the web, and only trigger when orders need to be processed.

## Other Considerations
- Persistent tracking/logging of orders in a database
- Using a client generator, such as Microsoft.Kiota, to generate tailored http clients for the endpoints we are calling. This saves times and ensures the OpenApi specification matches data models we use.
- Integration testing with live endpoints.
