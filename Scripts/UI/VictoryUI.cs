using Godot;
using System;
using UpBall.Managers;

namespace UpBall.UI;

/// <summary>
/// Victory screen shown when completing a level.
/// </summary>
public partial class VictoryUI : Control
{
    private Button _nextLevelButton;
    private Button _menuButton;
    private Label _levelLabel;
    private Label _congratsLabel;

    public override void _Ready()
    {
        _nextLevelButton = GetNode<Button>("VBoxContainer/NextLevelButton");
        _menuButton = GetNode<Button>("VBoxContainer/MenuButton");
        _levelLabel = GetNode<Label>("VBoxContainer/LevelLabel");
        _congratsLabel = GetNode<Label>("VBoxContainer/CongratsLabel");

        _nextLevelButton.Pressed += OnNextLevelPressed;
        _menuButton.Pressed += OnMenuPressed;

        UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (GameManager.Instance != null)
        {
            _levelLabel.Text = $"Level {GameManager.Instance.CurrentLevel} Complete!";
        }
    }

    private void OnNextLevelPressed()
    {
        Hide();
        GetTree().ReloadCurrentScene();
        GameManager.Instance?.NextLevel();
    }

    private void OnMenuPressed()
    {
        GameManager.Instance?.GoToMenu();
    }

    public new void Show()
    {
        UpdateLabels();
        Visible = true;
    }
}
