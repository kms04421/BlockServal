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
        Debug.Log(chunkPath.Count);
        // �� ûũ�� ������� ����
        foreach (var chunk in chunkPath)
        {
            Debug.Log("�̵��� ûũ: " + chunk.x  + " : " + chunk.z);
        }
        int test = 0;
        while (Vector3.Distance(transform.position,player.position) > 1f)
        {
            List<Vector3Int> movePath = AStarBlockPathfinder.FindPath(WorldPositionHelper.GetIntBlockPosition(transform.position), WorldPositionHelper.GetIntBlockPosition(player.position), startChunkPos);
            if (movePath == null) break;// ��ΰ� ������ ���� ����
            test++;
            yield return StartCoroutine(FollowPath(movePath));
            if (test == 2) break;
        }

    }
    // ���� ���� �Ÿ��� ���� �������� �̵�

    IEnumerator FollowPath(List<Vector3Int> path)
    {
        int currentIndex = 0;
        float speed = 5;
        if (path == null || path.Count == 0) yield break;

        while (currentIndex != path.Count)
        {

            Vector3 targetPos = WorldPositionHelper.LocalToWorld(path[currentIndex], WorldPositionHelper.GetChunkPosition(transform.position));
            float step = speed * Time.deltaTime;

            // �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            // ���� �������� �Ѿ��
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                currentIndex++;

                if (currentIndex >= path.Count)
                {
                    path.Clear(); // ��� ����
                    Debug.Log("����!");
                    break;
                }
            }

            yield return null;
        }

    }


}
