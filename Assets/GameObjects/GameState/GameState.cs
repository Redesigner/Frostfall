using UnityEngine;

public class GameState : MonoBehaviour
{
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
        Time.timeScale = 0.0f;
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
    }
}
