using Godot;
using System;
using UpBall.Managers;

namespace UpBall.UI;

/// <summary>
/// HUD showing current level during gameplay.
/// </summary>
public partial class HUD : CanvasLayer
{
    private Label _levelLabel;

    public override void _Ready()
    {
        _levelLabel = GetNode<Label>("LevelLabel");

        // Connect to GameManager signals
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelChanged += OnLevelChanged;
            UpdateLevel(GameManager.Instance.CurrentLevel);
        }
    }

    private void OnLevelChanged(int level)
    {
        UpdateLevel(level);
    }

    private void UpdateLevel(int level)
    {
        _levelLabel.Text = $"Level {level}";
    }
}
