using Microsoft.AspNetCore.Mvc;
using MoneyTrackerToFIRE.Contracts.Events;

namespace MoneyTrackerToFIRE.Api.Controllers;

[Route("[controller]/[action]")]
public class DemoController : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> Test()
    {
        //打印现在时间
        return Ok(new MartenDemoTest().STime);
    }
}