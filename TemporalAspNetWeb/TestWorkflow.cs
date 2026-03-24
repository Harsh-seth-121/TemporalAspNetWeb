using System;
using System.Threading.Tasks;
using Temporalio.Common;
using Temporalio.Workflows;

namespace TemporalAspNetWeb;

[Workflow]
public class TestWorkflow
{
    private bool _endWorkflow;
    private bool _signalReceived;
    private int _newValue;
    private int _runningSum;

    [WorkflowRun]
    public async Task<string> RunAsync()
    {
        while (true)
        {
            await Workflow.WaitConditionAsync(() => _signalReceived);

            _signalReceived = false;

            if (_endWorkflow)
                break;

            _runningSum += await Workflow
                .ExecuteActivityAsync<TestActivity, int>(a => a.Add(_runningSum, _newValue), GetActivityOptions());
        }

        return $"The final sum is {_runningSum}.";
    }

    [WorkflowSignal]
    public Task AddSignal(int num)
    {
        _newValue = num;
        _signalReceived = true;
        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task EndWorkflowSignal()
    {
        _endWorkflow = true;
        _signalReceived = true;
        return Task.CompletedTask;
    }

    private static ActivityOptions GetActivityOptions() => new()
    {
        StartToCloseTimeout = TimeSpan.FromSeconds(10),
        CancellationToken = Workflow.CancellationToken,
        RetryPolicy = new RetryPolicy
        {
            MaximumAttempts = 1,
        }
    };
}