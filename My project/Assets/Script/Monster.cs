using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Transform player;
    private Vector3 playerTransform;
    private Coroutine moveCoroutine;
    private ChunkPos saveChunkPos;
    List<ChunkPos> chunkPath;
    public delegate void CotoutineStop();
    void Start()
    {           
        player = GameManager.Instance.player.transform;
        playerTransform = new Vector3(0, 0, 0);
    }
    private void FixedUpdate()
    {
        if (Vector3.Distance(player.position , playerTransform) > 1f)
        {    
            StopAllCoroutines();           
            StartCoroutine(StartPathfindingLoop());
            playerTransform = player.position;
        }
        
    }
    private IEnumerator StartPathfindingLoop() // 경로찾고 이동실행
    {

        Vector3Int startPos = WorldPositionHelper.GetChunkPosition(transform.position);
        Vector3Int goalPos = WorldPositionHelper.GetChunkPosition(player.position);
        ChunkPos startChunkPos = new ChunkPos(startPos.x, startPos.z);
        ChunkPos goalChunkPos = new ChunkPos(goalPos.x, goalPos.z);
        chunkPath = AStarBlockPathfinder.FindChunkPath(startChunkPos, goalChunkPos); // 청크 루트 검색
        Vector3Int start = new Vector3Int();
        Vector3Int goal = new Vector3Int();
        int count = 0;
        //플레이어 추적 
        while (Vector3.Distance(transform.position, player.position) > 1f)
        {
            if (chunkPath.Count > 0)
            {
                Vector3Int[] targets = CalculateNextChunkTargets(chunkPath[count]);
                
                if (count == 0)
                {
                    start = WorldPositionHelper.GetIntBlockPosition(transform.position);
                }
                else
                {
                    start = targets[0];
                }
                if (chunkPath.Count - 1 == count)
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
    private Vector3Int[] CalculateNextChunkTargets(ChunkPos chunk) //청크 넘어갈때 몬스터와 가장 가까운지점 : start, 플레이어와 가장 가까운 지점 찾기 : goal
    {
        Vector3Int[] targetPos = new Vector3Int[2];

        Vector3Int Monsrer_Pos = WorldPositionHelper.Vector3ToVector3Int(transform.position);
        Vector3Int Player_Pos = WorldPositionHelper.Vector3ToVector3Int(player.position);

        float MonsterDistance = int.MaxValue;
        float PlayerDistance = int.MaxValue;
        for (int x = 0; x < TerrainChunk.chunkWidth; x++)       
            for (int z = 0; z < TerrainChunk.chunkWidth; z++)
            {
                if (x == 0 || x == TerrainChunk.chunkWidth - 1 || z == 0 || z == TerrainChunk.chunkWidth - 1)
                {

                    Vector3Int sidePos = WorldPositionHelper.LocalToWorld(new Vector3Int(x, Monsrer_Pos.y, z), chunk);

                    if (Vector3Int.Distance(Monsrer_Pos, sidePos) < MonsterDistance)
                    {
                        MonsterDistance = Vector3Int.Distance(Monsrer_Pos, sidePos);
                        targetPos[0] = new Vector3Int(x, Monsrer_Pos.y, z);
                    }
                    if (Vector3Int.Distance(Player_Pos, sidePos) < PlayerDistance)
                    {
                        PlayerDistance = Vector3Int.Distance(Player_Pos, sidePos);
                        targetPos[1] = new Vector3Int(x, Player_Pos.y, z);
                    }
                }
            }
        
        for (int i = 0; i < targetPos.Length; i++)
        {
            targetPos[i] = FindGroundPosition(chunk, targetPos[i]);
        }

        return targetPos;
    }
    private Vector3Int FindGroundPosition(ChunkPos chunk, Vector3Int targetPos) // 지면 찾기
    {
        BlockType blocks = TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y, targetPos.z];
        if (blocks == BlockType.Air)
        {
            // targetPos의 아래가 공기일경우 공기가 아닐때까지 하단으로 이동
            while (TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y - 1, targetPos.z] == BlockType.Air) 
            {
                targetPos = new Vector3Int(targetPos.x, targetPos.y - 2, targetPos.z);
            }
        }
        else
        {
            // targetPos의 위가 공기아닐경우 공기가 될때까지 상단으로 이동
            while (TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y + 1, targetPos.z] != BlockType.Air)
            {
                targetPos = new Vector3Int(targetPos.x, targetPos.y + 2, targetPos.z);
            }
        }
        return targetPos;
    }
    private IEnumerator FollowPath(List<Vector3Int> path, ChunkPos chunk) //경로 따라 이동
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
            if (Vector3.Distance(transform.position, targetPos) < 0.05f && Vector3.Distance(transform.position, player.position) > 1f)
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
