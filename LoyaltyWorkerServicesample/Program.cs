using LoyaltyWorkerServicesample;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHangfire(configuration =>
        {
            configuration.UseSqlServerStorage("Data Source = LTPCHE032422901\\SQLEXPRESS; Initial Catalog=Loyaltysample; Integrated Security = True;TrustServerCertificate=True");
        });
    })
    .Build();

await host.RunAsync();
