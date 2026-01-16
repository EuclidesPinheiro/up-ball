using Godot;
using System;
using UpBall.Managers;

namespace UpBall.UI;

/// <summary>
/// Main Menu screen.
/// </summary>
public partial class MainMenu : Control
{
    private TextureButton _playButton;
    private Label _highScoreLabel;

    public override void _Ready()
    {
        _playButton = GetNode<TextureButton>("VBoxContainer/PlayButton");
        _highScoreLabel = GetNode<Label>("VBoxContainer/HighScoreLabel");

        _playButton.Pressed += OnPlayPressed;

        UpdateHighScore();
    }

    private void UpdateHighScore()
    {
        if (GameManager.Instance != null)
        {
            _highScoreLabel.Text = $"Best Level: {GameManager.Instance.HighScore}";
        }
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://Upballfield.tscn");
        GameManager.Instance?.StartGame();
    }
}
