using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player;
    void Start()
    {
        Vector3Int startPos = GetIntPosition(transform.position);
        ChunkPos chunkPos = new ChunkPos(startPos.x, startPos.z);
        BlockType[,,] block = TerrainGenerator.chunks[chunkPos].blocks;
        AStarBlockPathfinder aStarBlockPathfinder = new AStarBlockPathfinder();
        /*        Vector3Int a = GetIntBlockPosition(transform.position);
                Debug.Log(a);*/
        List<Vector3Int> vector3Ints = aStarBlockPathfinder.FindPath(GetIntBlockPosition(transform.position), GetIntBlockPosition(player.position), block);
        StartCoroutine(move(vector3Ints));
    }
    IEnumerator move(List<Vector3Int> path)
    {
        int currentIndex = 0;
        float speed = 1;
        if (path == null || path.Count == 0) yield break;
      
        while(currentIndex != path.Count - 1)
        {

            Vector3 targetPos = LocalToWorld(path[currentIndex], GetIntPosition(transform.position));
            Debug.Log(Vector3.Distance(transform.position, targetPos));
            Debug.Log(Vector3.Distance(transform.position, targetPos) < 0.05f);
            Vector3 moveDir = (targetPos - transform.position).normalized;
            float step = speed * Time.deltaTime;

            // 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            // 다음 지점으로 넘어가기
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                currentIndex++;

                if (currentIndex >= path.Count)
                {
                    path.Clear(); // 경로 종료
                    Debug.Log("도착!");
                }
            }

            yield return null;
        }
        
    }
    private Vector3Int GetIntPosition(Vector3 pos)
    {
        Vector3Int Pos = new Vector3Int((Mathf.FloorToInt(pos.x / 16) * 16), Mathf.FloorToInt(pos.y) - 1, Mathf.FloorToInt(pos.z / 16) * 16);
        return Pos;
    }

    public Vector3Int GetIntBlockPosition(Vector3 pos)
    {
        int chunkStartX = Mathf.FloorToInt(pos.x / 16f) * 16;
        int chunkStartZ = Mathf.FloorToInt(pos.z / 16f) * 16;

        int localX = Mathf.FloorToInt(pos.x) - chunkStartX + 1;
        int localY = Mathf.FloorToInt(pos.y);
        int localZ = Mathf.FloorToInt(pos.z) - chunkStartZ + 1;

        return new Vector3Int(localX, localY, localZ);
    }
    public Vector3 LocalToWorld(Vector3Int localPos, Vector3 chunkWorldPos)
    {
        return new Vector3(
            chunkWorldPos.x + localPos.x - 1,
            localPos.y,
            chunkWorldPos.z + localPos.z - 1
        );
    }
}
