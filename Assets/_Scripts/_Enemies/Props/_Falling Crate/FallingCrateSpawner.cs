using System.Collections;
using UnityEngine;

public class FallingCrateSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform cratePrefab;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [SerializeField] private float spawnY = 10f; // height where crates appear

    [Header("Timing Settings")]
    [SerializeField] private float fallingDuration = 5f;
    [SerializeField] private float spawnInterval = 0.5f;

    private Coroutine _spawnRoutine;

    public void StartSpawning()
    {
        if (_spawnRoutine != null) return;

        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        float elapsed = 0f;

        while (elapsed < fallingDuration)
        {
            SpawnCrate();

            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        _spawnRoutine = null;
    }

    private void SpawnCrate()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        ObjectPooler.Instance.Spawn(cratePrefab, spawnPos, Quaternion.identity);
    }
}