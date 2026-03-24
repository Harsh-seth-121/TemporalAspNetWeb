using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temporalio.Client;
using Temporalio.Extensions.Hosting;
using Microsoft.Extensions.Http.Logging;

namespace TemporalAspNetWeb;

public class Program
{
    public static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            //.C(ctx => ctx.AddSimpleConsole().SetMinimumLevel(LogLevel.Information));
        ConfigureTemporalServices(builder);

        var app = builder.Build();
        return app.RunAsync();
    }

    
    private static void ConfigureTemporalServices(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration.GetSection("TemporalSettings").Get<TemporalSettings>();

        // Register Client
        builder.Services.AddTemporalClient(opts =>
        {

            opts.TargetHost = settings?.ClientTargetHost;
            if (settings?.ClientNamespace != null) 
                opts.Namespace = settings.ClientNamespace;

            if (string.IsNullOrWhiteSpace(settings?.ApiKey))
                return;

            //opts.Tls = new TlsOptions();
            opts.ApiKey = settings.ApiKey;
        }).Configure((ITemporalClient c) =>
        {
            c.Connection.ConnectAsync();
        });

        // Register Workflows
        MjrAddHostedTemporalWorker(builder, "test-queue", settings)
            .AddWorkflow<TestWorkflow>()
            .AddSingletonActivities<TestActivity>();
    }

    private static ITemporalWorkerServiceOptionsBuilder MjrAddHostedTemporalWorker(IHostApplicationBuilder builder, string taskQueue, TemporalSettings settings) =>
        builder.Services.AddHostedTemporalWorker(taskQueue).ConfigureOptions(options =>
        {
            options.ClientOptions = GetClientOptions(settings);
            options.DebugMode = builder.Environment.IsDevelopment();
            options.MaxHeartbeatThrottleInterval = TimeSpan.FromSeconds(0);
        });


    private static TemporalClientConnectOptions GetClientOptions(TemporalSettings settings)
    {
        var temporalClientConnectOptions = new TemporalClientConnectOptions
        {
            Namespace = settings.ClientNamespace ?? "default",
            TargetHost = settings?.ClientTargetHost
        };

        if (string.IsNullOrWhiteSpace(settings?.ApiKey))
            return temporalClientConnectOptions;

        temporalClientConnectOptions.Tls = new TlsOptions();
        temporalClientConnectOptions.ApiKey = settings.ApiKey;

        return temporalClientConnectOptions;
    }
}