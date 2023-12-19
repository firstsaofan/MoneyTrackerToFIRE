using Marten;
using Marten.Events.Projections;
using MassTransit;
using MoneyTrackerToFIRE.Api.Projections;
using MoneyTrackerToFIRE.Contracts.Events;
using Weasel.Core;

//用于启用或禁用 Npgsql 客户端与 Postgres 服务器之间的时间戳行为。它并不会直接修改 Postgres 的时区设置。
//时间设置要提前，之前在身份检验后会导致失效
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

//注入Bus
builder.Services.AddMassTransit(x =>
{
    // elided...

    //x.AddConsumer<BConsume>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });

});

// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);
    //options.Connection(builder.Configuration.GetValue("Marten")!);
    //options.Connection(builder.Configuration["ConnectionStrings:Marten"]);
    options.Schema.For<MartenDemoTest>();

    options.Projections.Add(new MartenDemoTestProjection(), ProjectionLifecycle.Inline);
    //options.Projections.Add<MartenDemoTestProjection>(ProjectionLifecycle.Inline);

    options.Events.AddEventType(typeof(MartenDemoTest));

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
});


builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//自动根据模型生成db.sql文件
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetService<IDocumentStore>() ?? throw new Exception($"无法找到服务：{typeof(IDocumentStore)}");
    //store.Schema.WriteDatabaseCreationScriptFile("my_database.sql"); //4-5版本写法
    //官网地址:https://martendb.io/schema/exporting.html#exporting-the-schema-definition

    //System.InvalidOperationException: The ConnectionString property has not been initialized.
    //如果不注入event 下面语句会报上述错误
    //options.Events.AddEventType(typeof(MartenDemoTest));
    store.Storage.WriteCreationScriptToFile("my_database.sql");//6版本写法
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

