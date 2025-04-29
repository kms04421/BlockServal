using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player;
    void Start()
    {
        StartCoroutine(StartPathfindingLoop());
    }
    public IEnumerator StartPathfindingLoop()
    {

        Vector3Int startPos = WorldPositionHelper.GetChunkPosition(transform.position);
        Vector3Int goalPos = WorldPositionHelper.GetChunkPosition(player.position);
        ChunkPos startChunkPos = new ChunkPos(startPos.x, startPos.z);
        ChunkPos goalChunkPos = new ChunkPos(goalPos.x, goalPos.z);
        List<ChunkPos> chunkPath = AStarBlockPathfinder.FindChunkPath(startChunkPos, goalChunkPos);
        Vector3Int start = new Vector3Int();
        Vector3Int goal = new Vector3Int();
        int count = 0;
        while (Vector3.Distance(transform.position, player.position) > 1f)
        {
            if (chunkPath.Count > 0)
            {
                Vector3Int[] targets = CalculateNextChunkTargets(chunkPath[count]);
                Debug.Log(targets[0] + " : " + targets[1]);
                if(count == 0)
                {
                    start = WorldPositionHelper.GetIntBlockPosition(transform.position);
                }
                else
                {
                    start = targets[0];
                }
                if(chunkPath.Count-1 == count)
                {
                    goal = WorldPositionHelper.GetIntBlockPosition(player.position);
                }
                else
                {
                    goal = targets[1];
                }
                   
            }
            else
            {
                start = WorldPositionHelper.GetIntBlockPosition(transform.position);
                goal = WorldPositionHelper.GetIntBlockPosition(player.position);
            }

            List<Vector3Int> movePath = AStarBlockPathfinder.FindPath(start, goal, chunkPath[count]);
            if (movePath == null) break;// 경로가 없으면 추적 정지
            yield return StartCoroutine(FollowPath(movePath, chunkPath[count]));
            count++;
            if (count >= chunkPath.Count) break;
        }

    }
    public Vector3Int[] CalculateNextChunkTargets(ChunkPos chunk)
    {
        Vector3Int MonsterMin = new Vector3Int();
        Vector3Int PlayerMin = new Vector3Int();

        Vector3Int Monsrer_Pos = WorldPositionHelper.Vector3ToVector3Int(transform.position);
        Vector3Int Player_Pos = WorldPositionHelper.Vector3ToVector3Int(player.position);

        float MonsterDistance = int.MaxValue;
        float PlayerDistance = int.MaxValue;
        for (int x = 0; x < TerrainChunk.chunkWidth; x++)
        {
            for (int z = 0; z < TerrainChunk.chunkWidth; z++)
            {
                if (x == 0 || x == TerrainChunk.chunkWidth - 1 || z == 0 || z == TerrainChunk.chunkWidth - 1)
                {

                    Vector3Int sidePos = WorldPositionHelper.LocalToWorld(new Vector3Int(x, Monsrer_Pos.y, z), chunk);

                    if (Vector3Int.Distance(Monsrer_Pos, sidePos) < MonsterDistance)
                    {
                        MonsterDistance = Vector3Int.Distance(Monsrer_Pos, sidePos);
                        MonsterMin = new Vector3Int(x, Monsrer_Pos.y, z);
                    }
                    if (Vector3Int.Distance(Player_Pos, sidePos) < PlayerDistance)
                    {
                        PlayerDistance = Vector3Int.Distance(Player_Pos, sidePos);
                        PlayerMin = new Vector3Int(x, Player_Pos.y, z);
                    }
                }

            }
        }

        return new Vector3Int[] { MonsterMin, PlayerMin };
    }

    IEnumerator FollowPath(List<Vector3Int> path, ChunkPos chunk)
    {
        int currentIndex = 0;
        float speed = 5;
        if (path == null || path.Count == 0) yield break;

        while (currentIndex != path.Count)
        {

            Vector3 targetPos = WorldPositionHelper.LocalToWorld(path[currentIndex], chunk);
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
                    break;
                }
            }

            yield return null;
        }

    }


}
