# code-showcase

A collection of diverse code snippets and projects demonstrating my software development skills across various domains.
This collection is unfortunately not exhaustive, but it does provide a glimpse into my coding style and problem-solving abilities.

I have room at my work to spent some time on learning new subjects and technologies.
Some of the projects are created during this time.

## Azure-Pipelines

I was tasked with creating a pipeline for our pre-release that was using Azure DevOps.
We have integration UI tests which we wanted to automate as well. This pipeline was created to automate the process.
I have included a few steps used by the pipeline to show how it was set up.

## Bash

### tmux-ssh

This script was created to simplify the process of connecting to remote servers using `tmux`.
For more information, refer to the [README](./Bash/tmux-ssh/README.md).

## Batch

The product I am working on makes use of Windows services. And sometimes there is the need to quickly start more instances of the same service.
This script was created to simplify the process of starting multiple instances of the same service.

## Dotnet

### Auditing

This code is created to save audit logs to an audit store. Currently only the logfile is implemented as there is currently no need for an additional store.
The logfile is from log4net and is saved in a specific format. More about that can be found in [Logging](#Logging).

### LiveLog

This code is created to view logs in real-time. The frontend part can be found in the [Javascript](#LiveLogView) section.
The logs are pushed through Nats, which can be viewed at [Logging](#Logging), this enables to collect logs from multiple services and machines.
The logs are then retrieved using Nats and saved in a LiteDB database, which only exists in memory and when needed.
Lastly the logs can be viewed in a web interface, with support to filter on specific services, log-levels and search for specific text.

### Logging

The use of log4net is a legacy choice. In the codebase we replaced logging with the `Microsoft.Extensions.Logging` library.
Now log4net is still used to log to files and to push logs to Nats.

There is code to configure log4net and it's appenders, which was done in configuration files before.

### Middleware

#### RawSampling

This middleware is used to sample raw data. The data is saved in a file.
The purpose of this middleware is in the time a customer tries to integrate with our product and the data is not as expected.

### Nats

The use of Nats is originated from the need to communicate between services.
One of the use cases for that is to push and collect logs from multiple services and machines.
Another one is to signal when the configurations are changed, which are saved in the database.
This gives the ability to reload the configurations without restarting the services.

The code here handles the connection to Nats and the publishing and subscribing to subjects.
Next, there is also code to handle the Nats configuration changes. The configurations for Nats are saved in a config file.
This is designed like this, because the ability of a new machine to join the network and start receiving messages without the need to restart the services.
When a new machine is added to the network, the configuration is changed and the services are notified of the change.

### OptionsSeeder

This utility is used to seed the options in the database. This was needed as part to remove our hardcoded secrets from the codebase.
More about this can be found in the [PowerShell](#SecretsManager) section.

### SchemaValidation

This code is used to validate the schema of the SOAP messages we receive and sent.
The code here is old, which is visible, still it's in use and working as expected. Our incoming and outgoing SOAP APIs are validated with this code.

### SoapServices

This code is used to set up the SOAP services we provide. The code is less old then the [SchemaValidation](#SchemaValidation) code, but still old.
Before we used WCF to set up the SOAP services, but now we use the `SoapCore` library to set up the SOAP services.

### Templates

Here is a template that I created to speed up the process of setting up a new Business Rules project.
This is one of the projects I created during my learning time.

### TestsSupport

The code with names starting with `Assertable-` is used to support the tests.
The code here is used to assert to our options in a fluent way.

#### Entities

This code is used to support the tests. The code here is used to set up entities in a fluent way.
For example, given the following entity:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

You can then construct your `User` entity like this:

```csharp
Entity.User.With(user => user.Id = 1)
           .With(user => user.Name = "John Doe");
```

This library supports more features then just setting properties.
One of them is attaching an entity to another entity. This is useful when you have a one-to-many relationship.

#### Extensions

This code is used to support the tests. The `ReflectionExtensions` is used to support where reflection is needed.
The `SupportsResetExtensions` is used for a special feature which I implemented.
This feature is used to reset the state of entities in the database. Where could be defined that a specific entity could be reset or not.
As well as the way how to reset the entity could be set.

#### Soap

These code is used to support tests for the SOAP services.
This way you can construct a SOAP message that is not fully correct and test if the service is able to handle it.

#### TestClasses

Some various test classes are created to support the tests.

### UnitTests

These are the unit tests I created for the codebase. Here I show parts of the TestsSupport I created, as well as 
the style of the tests I created.

### Utility

These are utility applications created in my learning time.
The purpose of these applications is to learn new technologies and to see how they can be used in our product.

### Workflow

This application is created in my learning time. The purpose of the application is help me with my workflow.
Each week I have to send a weekly with the work I have done and the work I will do.
I also try to keep track of the feedback I receive.
This is a fun project to learn more about creating command line applications.

## Javascript

A few Javascript and Vue scripts I created for various purposes.
One of the pages I created is used to perform first-line support on our product.
Most of these actions are retrieving data from the database and displaying it.
The logic is made to handle lots of data and to be as fast as possible.
I am unable to showcase this code as it is proprietary, but I can show these scripts.

The `PerformantTageField` is a vue component that is able to handle lots (100.000) of ids with ease. These ids are used to retrieve data from the database.
The `TagBasedSelector` is a vue component that is used to select tags from a list of tags. The tags are used to filter data from the database.
The `formatMixin` is a mixin that is used to format data in a specific way.
The `csvExportMixin` is a mixin that is used to export data to a csv file. Retrieved data from the database can be exported to a csv file.

### LiveLogView

This is the frontend part of a service I created to view logs in real-time.
The backend part can be found in the [Dotnet](#LiveLog) section.

## PowerShell

### SecretsManager

This module came to life when I wanted to remove all hardcoded secrets from our codebase.
For more information, refer to the [README](./PowerShell/SecretsManager/README.md).

## Python

### InfluxDBQueryMetrics

This script was a customer request. They wanted to be able to query our InfluxDB instance for specific metrics.
For more information, refer to the [README](./Python/InfluxDBQueryMetrics/README.md).