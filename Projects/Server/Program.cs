using dotenv.net;

DotEnv.Load();

var applicationBuilder = new ApplicationBuilder(args);
applicationBuilder.AddDbContextPool();

var app = applicationBuilder.GetWebApplication();
app.Run();
