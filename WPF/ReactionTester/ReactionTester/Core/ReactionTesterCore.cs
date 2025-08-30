public enum EState : byte {
    None,
    Waiting,
    TooSoon,
    Result
}

public class ReactionTesterCore {
    private static ReactionTesterCore _instance;
    public static ReactionTesterCore Instance {
        get => _instance ??= new ReactionTesterCore();
    }
    
    public EState State { get; private set; } = EState.None;
    public long ReactionTimeMs { get; private set; }
    public string StatusMessage { get; private set; } = "Click to start";
    
    public void ResetTest() {
        State = EState.None;
        StatusMessage = "Click to start";
    }
    
    public void StartWaiting() {
        State = EState.Waiting;
        StatusMessage = "Wait for green...";
    }
    
    public void SetTooSoon() {
        State = EState.TooSoon;
        StatusMessage = "Too soon! Click to try again";
    }
    
    public void SetResult(long reactionTimeMs) {
        State = EState.Result;
        ReactionTimeMs = reactionTimeMs;
        StatusMessage = $"Reaction time: {reactionTimeMs} ms\nClick to try again";
    }
}