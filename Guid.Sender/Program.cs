class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName; //Gets the path of the appsettings.json
            var builder = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json"); //Adds the appsettings.json file to the Configuration Builder

            services.AddTransient(); //Registers our Implementation of IWorker
            services.AddTransient(); //Registers our Implementation of ISender
            services.AddTransient(); //Registers our Implementation of IRunner

            //Gets the matching property keys and values from the appsettings.json then binds to the ApplicationSettings class
            var config = builder.Build();
            var applicationSettings = config.GetSection("ApplicationSettings").Get();

            //register our application settings as singleton.
            services.AddSingleton(applicationSettings);
            //gets the service from the registered implementations of IWorker from the service collection and Run.
            services.BuildServiceProvider().GetService().Run();
        });
}