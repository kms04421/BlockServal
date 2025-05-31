using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player; // 플레이어좌표
    private Vector3 lastPlayerPos; // 마지막 플레이어좌표
    List<ChunkPos> chunkPath; // 청크 경로 저장용
    public MonsterData data; // 몬스터 데이터
    private int currentHP;

    private MonsterState currentState;
    void Start()
    {
        currentHP = data.maxHP;
        player = GameManager.Instance.player.transform;
        lastPlayerPos = new Vector3(0, 0, 0);
        StartCoroutine(MonsterUpdate());
    }
    public IEnumerator MonsterUpdate()
    {
        while(true)
        {
            if (Vector3.Distance(player.position, lastPlayerPos) > 1f)
            {             
                yield return StartCoroutine(StartPathfindingLoop());
                lastPlayerPos = player.position;
            }
            yield return null;
        }         
    }
    public void Hit(int dam)
    {
        if(currentHP - dam < 0)
        {
            currentHP = currentHP - dam;
        }
        else
        {
            currentHP = 0;
            ChangeState(new DieState(this));
        }
      
    }
    public void ChangeState(MonsterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    // 경로찾기 시작
    private IEnumerator StartPathfindingLoop() 
    {

        Vector3Int startPos = WorldPositionHelper.GetChunkPosition(transform.position);
        Vector3Int goalPos = WorldPositionHelper.GetChunkPosition(player.position);
        ChunkPos startChunkPos = new ChunkPos(startPos.x, startPos.z);
        ChunkPos goalChunkPos = new ChunkPos(goalPos.x, goalPos.z);
        chunkPath = AStarBlockPathfinder.FindChunkPath(startChunkPos, goalChunkPos); // 청크 경로찾기
        Vector3Int start = new Vector3Int();
        Vector3Int goal = new Vector3Int();
        int count = 0;
        //플레이어와 가까워질때까지 이동
        while (Vector3.Distance(transform.position, player.position) > 1f)
        {
            if (chunkPath.Count > 0)
            {
                Vector3Int[] targets = CalculateNextChunkTargets(chunkPath[count]);
                start = targets[0];  // 현재 청크에서 가장 가까운 위치로 시작점 설정
                
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

            List<Vector3Int> movePath = AStarBlockPathfinder.FindPath(start, goal, chunkPath[count]); // 경로찾기

            if (movePath == null) break;// 경로가 없으면 종료
            yield return StartCoroutine(FollowPath(movePath, chunkPath[count]));
            count++;
            if (count >= chunkPath.Count) break;
        }

    }
    private Vector3Int[] CalculateNextChunkTargets(ChunkPos chunk) // 다음 청크 도착위치 설정
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
        float speed = data.moveSpeed;
        float rotationSpeed = 10f;

        if (path == null || path.Count == 0) yield break;

        while (currentIndex != path.Count)
        {
            Vector3 targetPos = WorldPositionHelper.LocalToWorld(path[currentIndex], chunk);
            float step = speed * Time.deltaTime;
            // 방향 설정
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero) // LookRotation의 zero vector인 경우 종료
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            // 이동거리가 0.05f 이하이고 플레이어와의 거리가 1f 이상이면 다음 경로로 이동
            if (Vector3.Distance(transform.position, targetPos) < 0.05f && Vector3.Distance(transform.position, player.position) > 1f)
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
