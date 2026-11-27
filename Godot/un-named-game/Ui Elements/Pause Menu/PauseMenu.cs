using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    public override void _Ready()
{
    ProcessMode = ProcessModeEnum.Always;
    Visible = false;
    Pausable.PauseChanged += paused => Visible = paused;
}
}
