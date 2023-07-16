//metinyakar.net
var runcmds = new RunCommands();
runcmds.AllAfter();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(async trxContext => {
        var rl = new RequestLogger(trxContext);
        await rl.SaveRequest();
    });
var app = builder.Build();
app.MapReverseProxy(x => {
    x.UseForwardedHeaders();
    x.UseLoadBalancing();
    x.UseResponseCaching();
});
app.UseRequestCatcher();

//"webhook" endpoint that triggers run commands
app.MapPost("/__webhook/{appname}", ([FromRoute] string appname) => {
    var rc = new RunCommands(appname);
    rc.Before();
    rc.After();
    return appname;
});

app.Run();