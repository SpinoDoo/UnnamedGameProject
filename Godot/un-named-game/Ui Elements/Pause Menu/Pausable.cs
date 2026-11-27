using Godot;
using System;

public partial class Pausable : Node
{
    // Shared by every Pausable, so they all stay in sync
    public static bool IsPaused { get; private set; } = false;
    public static event Action<bool> PauseChanged;

    private static ulong _lastToggleFrame = ulong.MaxValue;

    private Node _target;
    private ProcessModeEnum _originalMode;

    public override void _Ready()
    {
        // Must keep running while the parent is frozen
        ProcessMode = ProcessModeEnum.Always;

        _target = GetParent();
        _originalMode = _target.ProcessMode;

        PauseChanged += Apply;
        if (IsPaused) Apply(true); // joined while the game was already paused
    }

    public override void _ExitTree()
    {
        PauseChanged -= Apply;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("PauseButton")) return;

        // Every Pausable receives the same key press, but only the first one toggles
        ulong frame = Engine.GetProcessFrames();
        if (frame == _lastToggleFrame) return;
        _lastToggleFrame = frame;

        IsPaused = !IsPaused;
        PauseChanged?.Invoke(IsPaused);
    }

    private void Apply(bool paused)
    {
        _target.ProcessMode = paused ? ProcessModeEnum.Disabled : _originalMode;
    }
}