using Godot;
using System;

namespace UpBall.Managers;

/// <summary>
/// Singleton para gerenciar estado global do jogo.
/// </summary>
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    // Game States
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    // Current state
    private GameState _currentState = GameState.Menu;
    public GameState CurrentState
    {
        get => _currentState;
        private set
        {
            _currentState = value;
            EmitSignal(SignalName.StateChanged, (int)value);
        }
    }

    // Level and Score
    public int CurrentLevel { get; private set; } = 1;
    public int HighScore { get; private set; } = 0;

    // Difficulty settings per level
    public float ObstacleSpeed => 100f + (CurrentLevel * 20f);
    public float SpawnInterval => Mathf.Max(1.5f - (CurrentLevel * 0.1f), 0.5f);
    public int ObstaclesPerLevel => 3 + CurrentLevel;

    // Signals
    [Signal] public delegate void StateChangedEventHandler(int newState);
    [Signal] public delegate void LevelChangedEventHandler(int level);
    [Signal] public delegate void GameOverEventHandler();
    [Signal] public delegate void VictoryEventHandler();

    public override void _Ready()
    {
        Instance = this;
        LoadHighScore();
    }

    public void StartGame()
    {
        CurrentLevel = 1;
        CurrentState = GameState.Playing;
        EmitSignal(SignalName.LevelChanged, CurrentLevel);
    }

    public void RestartLevel()
    {
        CurrentState = GameState.Playing;
    }

    public void NextLevel()
    {
        CurrentLevel++;
        CurrentState = GameState.Playing;
        EmitSignal(SignalName.LevelChanged, CurrentLevel);
    }

    public void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;
        EmitSignal(SignalName.GameOver);
    }

    public void TriggerVictory()
    {
        if (CurrentLevel > HighScore)
        {
            HighScore = CurrentLevel;
            SaveHighScore();
        }
        CurrentState = GameState.Victory;
        EmitSignal(SignalName.Victory);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            GetTree().Paused = true;
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            GetTree().Paused = false;
            CurrentState = GameState.Playing;
        }
    }

    public void GoToMenu()
    {
        CurrentState = GameState.Menu;
        GetTree().ChangeSceneToFile("res://Scenes/UI/MainMenu.tscn");
    }

    private void LoadHighScore()
    {
        if (FileAccess.FileExists("user://highscore.save"))
        {
            using var file = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Read);
            HighScore = (int)file.Get32();
        }
    }

    private void SaveHighScore()
    {
        using var file = FileAccess.Open("user://highscore.save", FileAccess.ModeFlags.Write);
        file.Store32((uint)HighScore);
    }
}
