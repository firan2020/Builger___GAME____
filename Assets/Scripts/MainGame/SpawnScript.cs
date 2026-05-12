using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject playerPrefab;

    private void Awake()
    {
        GameObject randomSpawnPoint = GetRandomPoint();

        Instantiate(playerPrefab, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation);
    }

    GameObject GetRandomPoint()
    {
        if (spawnPoints.Length == 0) return null;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
}
