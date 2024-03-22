namespace ExtensionManager.UI.Worker;

public static class ProgressStepExtensions
{
    public static void Report<TStep>(this IProgress<ProgressStep<TStep>> progress, float? percentage, TStep step)
        => progress.Report(new(percentage, step));
}
