using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [SerializeField]
    private List<BubbleBase> bubblePrefabs = new();

    [SerializeField]
    private float dispersion = 10f;

    [SerializeField]
    private float delayBetweenWaves = 3f;

    [SerializeField]
    private int spawnAmount = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartSpawner());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBubbles(int amount)
    {
        for (int i = 1; i < amount; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-dispersion, dispersion), 0, Random.Range(-dispersion,dispersion));
            Instantiate(bubblePrefabs.GetRandomElementFromList(), spawnPosition, Quaternion.identity);

        }
    }

    IEnumerator StartSpawner()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        SpawnBubbles(spawnAmount);
        StartCoroutine(StartSpawner());
    }
}
