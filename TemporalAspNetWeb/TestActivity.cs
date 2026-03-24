using Temporalio.Activities;

namespace TemporalAspNetWeb;

public class TestActivity
{
    [Activity]
    public int Add(int a, int b)
    {
        return a + b;
    }
}