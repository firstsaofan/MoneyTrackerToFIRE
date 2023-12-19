using Marten;
using Marten.Events.Projections;
using MassTransit;
using MoneyTrackerToFIRE.Api.Projections;
using MoneyTrackerToFIRE.Contracts.Events;
using Weasel.Core;

//�������û���� Npgsql �ͻ����� Postgres ������֮���ʱ�����Ϊ����������ֱ���޸� Postgres ��ʱ�����á�
//ʱ������Ҫ��ǰ��֮ǰ����ݼ����ᵼ��ʧЧ
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

//ע��Bus
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

//�Զ�����ģ������db.sql�ļ�
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetService<IDocumentStore>() ?? throw new Exception($"�޷��ҵ�����{typeof(IDocumentStore)}");
    //store.Schema.WriteDatabaseCreationScriptFile("my_database.sql"); //4-5�汾д��
    //������ַ:https://martendb.io/schema/exporting.html#exporting-the-schema-definition

    //System.InvalidOperationException: The ConnectionString property has not been initialized.
    //�����ע��event �������ᱨ��������
    //options.Events.AddEventType(typeof(MartenDemoTest));
    store.Storage.WriteCreationScriptToFile("my_database.sql");//6�汾д��
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

