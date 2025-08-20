using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;

    [SerializeField]
    [Min(0)]
    private int maxObjects = 3;

    private TimerHandle _spawnTimer;

    private int _spawnedObjectCount;

    [SerializeField]
    [Min(0.0f)]
    private float spawnTime = 1.0f;

    private void Start()
    {
        SpawnObject();
    }

    private void SpawnObject()
    {
        _spawnedObjectCount++;
        GameObject newlySpawnedPrefab = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        var healthComponent = newlySpawnedPrefab.GetComponent<HealthComponent>();
        if (healthComponent)
        {
            healthComponent.onDeath.AddListener(ObjectDestroyed);
        }
        else
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var destroyEventComponent = newlySpawnedPrefab.GetComponent<DestroyEvent>();
            if (destroyEventComponent)
            {
                destroyEventComponent.destroyed.AddListener(ObjectDestroyed);
            }
        }

        if (_spawnedObjectCount == maxObjects)
        {
            return;
        }

        _spawnTimer = TimerManager.instance.CreateTimer(this, spawnTime, SpawnObject);
    }

    private void ObjectDestroyed()
    {
        if (_spawnedObjectCount == maxObjects)
        {
            _spawnTimer = TimerManager.instance.CreateTimer(this, spawnTime, SpawnObject);
        }
        _spawnedObjectCount--;
    }
}
