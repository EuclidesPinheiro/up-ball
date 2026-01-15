using Godot;
using System;
using UpBall.Managers;

namespace UpBall.Entities;

/// <summary>
/// Sistema de spawn de obstáculos.
/// Gera buracos pretos e o buraco amarelo (objetivo).
/// </summary>
public partial class ObstacleSpawner : Node2D
{
    // Packed scenes for obstacles
    [Export] public PackedScene BlackHoleScene { get; set; }
    [Export] public PackedScene YellowHoleScene { get; set; }

    // Spawn settings
    [Export] public float SpawnY { get; set; } = -100f;
    [Export] public float MinX { get; set; } = 100f;
    [Export] public float MaxX { get; set; } = 620f;

    // Timer for spawning
    private Godot.Timer _spawnTimer;
    private int _obstaclesSpawned = 0;
    private int _targetObstacles = 5;
    private float _currentSpeed = 150f;
    private bool _goalSpawned = false;
    private bool _isActive = false;

    public override void _Ready()
    {
        // Create spawn timer
        _spawnTimer = new Godot.Timer();
        _spawnTimer.OneShot = false;
        _spawnTimer.Timeout += OnSpawnTimeout;
        AddChild(_spawnTimer);

        // Connect to GameManager signals
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LevelChanged += OnLevelChanged;
            GameManager.Instance.StateChanged += OnGameStateChanged;
        }
    }

    public void StartSpawning(int level)
    {
        _obstaclesSpawned = 0;
        _goalSpawned = false;
        _isActive = true;

        // Get difficulty settings from GameManager
        _currentSpeed = GameManager.Instance?.ObstacleSpeed ?? (100f + level * 20f);
        _targetObstacles = GameManager.Instance?.ObstaclesPerLevel ?? (3 + level);
        float interval = GameManager.Instance?.SpawnInterval ?? Mathf.Max(1.5f - level * 0.1f, 0.5f);

        _spawnTimer.WaitTime = interval;
        _spawnTimer.Start();
    }

    public void StopSpawning()
    {
        _isActive = false;
        _spawnTimer.Stop();
    }

    public void ClearObstacles()
    {
        // Remove all existing obstacles
        foreach (Node child in GetChildren())
        {
            if (child is BlackHole or YellowHole)
            {
                child.QueueFree();
            }
        }
    }

    private void OnSpawnTimeout()
    {
        if (!_isActive) return;

        if (_obstaclesSpawned < _targetObstacles)
        {
            SpawnBlackHole();
            _obstaclesSpawned++;
        }
        else if (!_goalSpawned)
        {
            SpawnYellowHole();
            _goalSpawned = true;
            _spawnTimer.Stop();
        }
    }

    private void SpawnBlackHole()
    {
        if (BlackHoleScene == null)
        {
            GD.PrintErr("BlackHoleScene not assigned!");
            return;
        }

        var hole = BlackHoleScene.Instantiate<BlackHole>();
        hole.Position = new Vector2(GetRandomX(), SpawnY);
        hole.SetSpeed(_currentSpeed);
        GetParent().AddChild(hole);
    }

    private void SpawnYellowHole()
    {
        if (YellowHoleScene == null)
        {
            GD.PrintErr("YellowHoleScene not assigned!");
            return;
        }

        var goal = YellowHoleScene.Instantiate<YellowHole>();
        goal.Position = new Vector2(GetRandomX(), SpawnY);
        goal.SetSpeed(_currentSpeed);
        GetParent().AddChild(goal);
    }

    private float GetRandomX()
    {
        return (float)GD.RandRange(MinX, MaxX);
    }

    private void OnLevelChanged(int level)
    {
        StartSpawning(level);
    }

    private void OnGameStateChanged(int state)
    {
        var gameState = (GameManager.GameState)state;
        
        if (gameState == GameManager.GameState.Playing)
        {
            // If restarting, clear and start fresh
            if (!_isActive)
            {
                ClearObstacles();
                StartSpawning(GameManager.Instance?.CurrentLevel ?? 1);
            }
        }
        else if (gameState == GameManager.GameState.GameOver || gameState == GameManager.GameState.Victory)
        {
            StopSpawning();
        }
    }
}
