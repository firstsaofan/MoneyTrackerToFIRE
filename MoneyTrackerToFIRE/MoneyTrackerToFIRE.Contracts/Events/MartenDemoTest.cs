namespace MoneyTrackerToFIRE.Contracts.Events;

public class MartenDemoTest
{
    public MartenDemoTest()
    {
        STime = DateTime.Now;
    }

    public long Id { get; set; }
    //获取当前时间
    public DateTime STime { get; set; }

    void Apply()
    {
        STime = DateTime.Now;
    }

}