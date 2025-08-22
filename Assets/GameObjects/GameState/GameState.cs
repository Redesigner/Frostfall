using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour
{
    public UnityEvent onGamePaused = new();
    public UnityEvent onGameUnpaused = new();

    public bool paused { get; private set; }
    
    private static GameState _instance;
    
    public static GameState instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }
            
            var container = new GameObject("GameState");
            
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            // Only called once per game
            _instance = container.AddComponent<GameState>();
            return _instance;
        }
    }
    
    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Pause()
    {
        if (paused)
        {
            return;
        }
        
        onGamePaused.Invoke();
        Time.timeScale = 0.0f;
        paused = true;
    }

    public void Unpause()
    {
        if (!paused)
        {
            return;
        }
        
        onGameUnpaused.Invoke();
        Time.timeScale = 1.0f;
        paused = false;
    }
}
