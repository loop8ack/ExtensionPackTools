namespace ExtensionManager.UI.Worker;

public record struct ProgressStep<TStep>(float? Percentage, TStep Step);
