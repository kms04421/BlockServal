using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Monster : MonoBehaviour
{
    public Transform player;
    void Start()
    {
        Vector3Int startPos = GetIntPosition(transform.position);
        ChunkPos chunkPos = new ChunkPos(startPos.x, startPos.z);
        BlockType[,,] block = TerrainGenerator.chunks[chunkPos].blocks;
        AStarBlockPathfinder aStarBlockPathfinder = new AStarBlockPathfinder();
        List<Vector3Int> vector3Ints = aStarBlockPathfinder.FindPath(startPos, GetIntPosition(player.position), block);
    }

    private Vector3Int GetIntPosition(Vector3 pos)
    {
        Vector3Int Pos = new Vector3Int(Mathf.FloorToInt(pos.x / 16) * 16, Mathf.FloorToInt(pos.y) - 1, Mathf.FloorToInt(pos.z / 16) * 16);
        return Pos;
    }
}
