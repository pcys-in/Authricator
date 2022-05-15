// See https://aka.ms/new-console-template for more information

using DemoConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Patronum.Authricator;
using Patronum.Authricator.Authorization;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));


services.InitializeAuthricator(x => x.AddStaticConfigurationProvider());

services.BuildServiceProvider().GetRequiredService<AuthorizationHandler>().Demo();

Console.WriteLine("Hello, World!");


