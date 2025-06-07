using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : MonsterState
{   private Vector3 lastPlayerPos;
    private List<ChunkPos> chunkPath;
    private Coroutine chaseCoroutine;
    public ChaseState(Monster monster) : base(monster) 
    {
         lastPlayerPos = new Vector3(0, 0, 0);
     }

    public override void Enter()
    {
        Debug.Log("Chase ");
        chaseCoroutine = monster.StartCoroutine(ChaseUpdate());
    }

    public override void Update()
    {
        if (Vector3.Distance(monster.transform.position, monster.player.position) < monster.data.attackRange)
        {
            monster.ChangeState(new AttackState(monster));
        }        
    }

    public override void Exit()
    {
        monster.StopAllCoroutines(); 
    }

      private IEnumerator ChaseUpdate()
    {
        while(true)
        {
            if (Vector3.Distance(monster.player.position, lastPlayerPos) > 1f)
            {             
                yield return monster.StartCoroutine(StartPathfindingLoop());
                lastPlayerPos = monster.player.position;
            }
            yield return null;
        }         
    }

    private IEnumerator StartPathfindingLoop()
    {
        Vector3Int startPos = WorldPositionHelper.GetChunkPosition(monster.transform.position);
        Vector3Int goalPos = WorldPositionHelper.GetChunkPosition(monster.player.position);
        ChunkPos startChunkPos = new ChunkPos(startPos.x, startPos.z);
        ChunkPos goalChunkPos = new ChunkPos(goalPos.x, goalPos.z);
        chunkPath = AStarBlockPathfinder.FindChunkPath(startChunkPos, goalChunkPos);

        Vector3Int start = new Vector3Int();
        Vector3Int goal = new Vector3Int();
        int count = 0;

        while (Vector3.Distance(monster.transform.position, monster.player.position) > 1f)
        {
            if (chunkPath.Count > 0)
            {
                Vector3Int[] targets = CalculateNextChunkTargets(chunkPath[count]);
                
                if (count == 0)
                {
                    start = WorldPositionHelper.GetIntBlockPosition(monster.transform.position);
                }
                else
                {
                    start = targets[0];
                }

                if (chunkPath.Count - 1 == count)
                {
                    goal = WorldPositionHelper.GetIntBlockPosition(monster.player.position);
                }
                else
                {
                    goal = targets[1];
                }
            }
            else
            {
                start = WorldPositionHelper.GetIntBlockPosition(monster.transform.position);
                goal = WorldPositionHelper.GetIntBlockPosition(monster.player.position);
            }

            List<Vector3Int> movePath = AStarBlockPathfinder.FindPath(start, goal, chunkPath[count]);

            if (movePath == null) break;
            yield return monster.StartCoroutine(FollowPath(movePath, chunkPath[count]));
            count++;
            if (count >= chunkPath.Count) break;
        }
    }
       private Vector3Int[] CalculateNextChunkTargets(ChunkPos chunk) // 다음 청크 도착위치 설정
    {
        Vector3Int[] targetPos = new Vector3Int[2];

        Vector3Int Monsrer_Pos = WorldPositionHelper.Vector3ToVector3Int(monster.transform.position);
        Vector3Int Player_Pos = WorldPositionHelper.Vector3ToVector3Int(monster.player.position);

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
    private Vector3Int FindGroundPosition(ChunkPos chunk, Vector3Int targetPos) // 땅 찾기
    {
        BlockType blocks = TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y, targetPos.z];
        if (blocks == BlockType.Air)
        {
            // targetPos의 위치가 비어있으면 땅을 찾을때까지 아래로 이동
            while (TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y - 1, targetPos.z] == BlockType.Air) 
            {
                targetPos = new Vector3Int(targetPos.x, targetPos.y - 2, targetPos.z);
            }
        }
        else
        {
            // targetPos의 위치가 비어있으면 땅을 찾을때까지 위로 이동
            while (TerrainGenerator.chunks[chunk].blocks[targetPos.x, targetPos.y + 1, targetPos.z] != BlockType.Air)
            {
                targetPos = new Vector3Int(targetPos.x, targetPos.y + 2, targetPos.z);
            }
        }
        return targetPos;
    }
    private IEnumerator FollowPath(List<Vector3Int> path, ChunkPos chunk) // 경로 따라 이동
    {
        int currentIndex = 0;
        float speed = monster.data.moveSpeed;
        float rotationSpeed = 10f;

        if (path == null || path.Count == 0) yield break;

        while (currentIndex != path.Count)
        {
            Vector3 targetPos = WorldPositionHelper.LocalToWorld(path[currentIndex], chunk);
            float step = speed * Time.deltaTime;
            // 방향 설정
            Vector3 direction = (targetPos - monster.transform.position).normalized;
            if (direction != Vector3.zero) // LookRotation의 zero vector인 경우 종료
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 이동
            monster.transform.position = Vector3.MoveTowards(monster.transform.position, targetPos, step);

            // 이동거리가 0.05f 이하이고 플레이어와의 거리가 1f 이상이면 다음 경로로 이동
            if (Vector3.Distance(monster.transform.position, targetPos) < 0.05f && Vector3.Distance(monster.transform.position, monster.player.position) > 1f)
            {
                currentIndex++;
                if (currentIndex >= path.Count)
                {
                    path.Clear(); // 경로 초기화
                    Debug.Log("도착");
                    break;
                }
            }

            yield return null;
        }

    }
}
