using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField]
    private List<BubbleBase> bubblePrefabs = new();

    [SerializeField]
    private float spawnRangeX = 10f;

    [SerializeField]
    private float spawnRangeZ = 10f;

    public float DelayBetweenWaves = 3f;

    public int SpawnAmount = 10;
    private Transform spawnPoint;
    [SerializeField]
    private float rotateSpeed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartSpawner());
        spawnPoint = transform.GetChild(0);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));
    }

    void SpawnBubbles(int amount)
    {
        for (int i = 1; i < amount; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0.5f, Random.Range(-spawnRangeZ, spawnRangeZ));
            PoolManager.Instance.GetPooledObject(bubblePrefabs.GetRandomElementFromList().gameObject, new PoolObjectSettings() { positionToSet = spawnPosition, rotationToSet = spawnPoint.rotation, timeBeforeReturningToPool = 60f });
            //Instantiate(bubblePrefabs.GetRandomElementFromList(), spawnPosition, transform.rotation);
        }
    }

    IEnumerator StartSpawner()
    {
        SpawnBubbles(SpawnAmount);
        yield return new WaitForSeconds(DelayBetweenWaves);
        StartCoroutine(StartSpawner());
    }
}
