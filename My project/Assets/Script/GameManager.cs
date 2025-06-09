using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject player;
    TerrainGenerator terrainGenerator;
    private void Awake()
    {
        terrainGenerator = TerrainGenerator.Instance;
    }
    private IEnumerator Start()
    {

        terrainGenerator.LoadChunks(player.transform);
        yield return new WaitUntil(() => TerrainGenerator.chunks.Count >= TerrainChunk.chunkWidth*TerrainGenerator.chunkDist);
        player.SetActive(true);
    }
}
