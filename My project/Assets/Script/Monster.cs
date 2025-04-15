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
     
     
     
        int count = 0;
        Vector2Int movementDir = new Vector2Int(0,0);
        Vector3Int goal;
        Vector3Int nextChunkPos = new Vector3Int(0, 0, 0);
        Vector3Int targetPos = new Vector3Int(0,0,0);
        while (true)
        {
            Vector3Int startPos = WorldPositionHelper.GetChunkPosition(transform.position);
            Vector3Int goalPos = WorldPositionHelper.GetChunkPosition(player.position);

            ChunkPos startChunkPos = new ChunkPos(startPos.x, startPos.z);
            ChunkPos goalChunkPos = new ChunkPos(goalPos.x, goalPos.z);

            BlockType[,,] blocks = TerrainGenerator.chunks[startChunkPos].blocks;
          
            if (!startPos.Equals(goalPos))
            {
                movementDir = new Vector2Int((goalChunkPos.x / 16) - (startChunkPos.x / 16),
                                                    (goalChunkPos.z / 16) - (startChunkPos.z / 16));
                goal = WorldPositionHelper.GetIntBlockPosition(GetChunkBasedGoalPosition(movementDir, startChunkPos));
            }
            else
            {
                goal = WorldPositionHelper.GetIntBlockPosition(player.position);
            }
            List<Vector3Int> movePath = AStarBlockPathfinder.FindPath(WorldPositionHelper.GetIntBlockPosition(transform.position), goal, startChunkPos);
            yield return StartCoroutine(FollowPath(movePath));

            if (!startPos.Equals(goalPos))
            {
               
                nextChunkPos = new Vector3Int((movementDir.x * 16) + startPos.x,(int)transform.position.y,(movementDir.y * 16) + startPos.z);
        
                targetPos = NextChunkBlockPos(blocks, goal); // 요건 블럭 포지션 
                yield return StartCoroutine(MoveToPosition(WorldPositionHelper.LocalToWorld(targetPos, nextChunkPos)));     
            }
          
       

         
         
            count++;
            if (count == 2)
            {
                break;
            }// 1.다음청크 값 구하기 
        }

    }

    public Vector3Int NextChunkBlockPos(BlockType[,,] blocks , Vector3Int goal)
    {
        int width = blocks.GetLength(0);
        int height = blocks.GetLength(1);
        int depth = blocks.GetLength(2);
        return  new Vector3Int(
                        width - 1 - goal.x,
                        goal.y,
                        depth - 1 - goal.z
                    );
    }
    public IEnumerator MoveToPosition(Vector3 targetPos, float moveSpeed = 5f)
    {
        float reachThreshold = 0.05f;

        while (Vector3.Distance(transform.position, targetPos) > reachThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
    }

    public Vector3Int GetChunkBasedGoalPosition(Vector2Int movementDir, ChunkPos chunkPos)
    {
        int chunkSize = TerrainChunk.chunkWidth;
        BlockType[,,] blocks = TerrainGenerator.chunks[chunkPos].blocks;
        int width = blocks.GetLength(0);
        int height = blocks.GetLength(1);
        int depth = blocks.GetLength(2);
        if (movementDir.x > 0 && movementDir.y == 0)// → 오른쪽
        {
            return new Vector3Int((chunkPos.x + 1) * chunkSize - 1, Mathf.FloorToInt(player.position.y), chunkPos.z * chunkSize + chunkSize / 2);
        }
        else if (movementDir.x < 0 && movementDir.y == 0)// ← 왼쪽
        {
            return new Vector3Int(chunkPos.x * chunkSize, Mathf.FloorToInt(player.position.y), chunkPos.z * chunkSize + chunkSize / 2);
        }
        else if (movementDir.x == 0 && movementDir.y > 0)// ↑ 위쪽
        {
            return new Vector3Int(chunkPos.x * chunkSize + chunkSize / 2, Mathf.FloorToInt(player.position.y), (chunkPos.z + 1) * chunkSize - 1);
        }
        else if (movementDir.x == 0 && movementDir.y < 0) // ↓ 아래쪽
        {
            return new Vector3Int(chunkPos.x * chunkSize + chunkSize / 2, Mathf.FloorToInt(player.position.y), chunkPos.z * chunkSize);
        }
        else if (movementDir.x > 0 && movementDir.y > 0) // ↗ 오른쪽 위 (대각선)
        {
            return new Vector3Int((chunkPos.x + 1) * chunkSize - 1, Mathf.FloorToInt(player.position.y), (chunkPos.z + 1) * chunkSize - 1);
        }
        else if (movementDir.x > 0 && movementDir.y < 0)// ↘ 오른쪽 아래 (대각선)
        {
            return new Vector3Int((chunkPos.x + 1) * chunkSize - 1, Mathf.FloorToInt(player.position.y), chunkPos.z * chunkSize);
        }
        else if (movementDir.x < 0 && movementDir.y > 0)// ↖ 왼쪽 위 (대각선)
        {
            return new Vector3Int(chunkPos.x * chunkSize, Mathf.FloorToInt(player.position.y), (chunkPos.z + 1) * chunkSize - 1);
        }
        else if (movementDir.x < 0 && movementDir.y < 0) // ↙ 왼쪽 아래 (대각선)
        {
            return new Vector3Int(chunkPos.x * chunkSize, Mathf.FloorToInt(player.position.y), chunkPos.z * chunkSize);
        }
        else// (0, 0) → 도착 지점
        {
            return Vector3Int.zero;
        }
    }
    IEnumerator FollowPath(List<Vector3Int> path)
    {
        int currentIndex = 0;
        float speed = 5;
        if (path == null || path.Count == 0) yield break;

        while (currentIndex != path.Count)
        {

            Vector3 targetPos = WorldPositionHelper.LocalToWorld(path[currentIndex], WorldPositionHelper.GetChunkPosition(transform.position));
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
