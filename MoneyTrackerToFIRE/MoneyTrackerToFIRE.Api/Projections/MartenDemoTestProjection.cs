using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using MoneyTrackerToFIRE.Contracts.Events;

namespace MoneyTrackerToFIRE.Api.Projections;

public class MartenDemoTestProjection: EventProjection
{
    public MartenDemoTest Create(IEvent<MartenDemoTest> input)
    {
        return new MartenDemoTest();
    }
    
    //获取当前时间
    public DateTime STime;
    
    
}