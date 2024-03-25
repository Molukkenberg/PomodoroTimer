using System.Diagnostics;

namespace PomodoroTimer.Components;

/// <summary>
/// Codebehind Class for PomodoroTimerComponent
/// </summary>
public partial class PomodoroTimerComponent
{
    public double PomodoroMinutes { get; private set; } = 25;
    public double RadialProgress { get; private set; }
    public TimeSpan RemainingTime { get; private set; }

    private double _stepSize;
    private double _remainingSeconds;
    
    private Stopwatch _stopwatch = new();
    private Timer? _timer;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private void StartTimer()
    {
        if (_stopwatch.IsRunning)
        {
            return;
        }

        _stepSize = 12 / (PomodoroMinutes * 60);

        _timer = new(async state =>
        {
            await DoWork();

        }, null, 0, 1000);

        _stopwatch.Start();
    }

    private async Task DoWork()
    {
        if(!_stopwatch.IsRunning)
        {
            return;
        }

        if (_stopwatch.Elapsed.TotalSeconds >= (PomodoroMinutes * 60))
        {
            StopTimer(true);
        }

        _remainingSeconds = (PomodoroMinutes * 60) - _stopwatch.Elapsed.TotalSeconds;
        RemainingTime = new TimeSpan(0,0,0, Convert.ToInt32(_remainingSeconds));
        RadialProgress = _remainingSeconds * _stepSize;
        await InvokeAsync(StateHasChanged);
    }

    private void StopTimer(bool timeRanOut)
    {
        _stopwatch.Stop();
        _timer?.Dispose();
        _timer = null;

        if (timeRanOut)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        StopTimer(false);

        _stopwatch.Reset();
        RadialProgress = 0;
        RemainingTime = new TimeSpan(0, Convert.ToInt32(PomodoroMinutes), 0);
    }
}