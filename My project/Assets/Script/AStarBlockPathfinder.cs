using System.Collections.Generic;
using UnityEngine;

public static class AStarBlockPathfinder
{
    // Ž���� 6����
    public static Vector3Int[] directions = {
            Vector3Int.right, Vector3Int.left,
            Vector3Int.forward, Vector3Int.back,
            Vector3Int.up, Vector3Int.down
      };


    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, ChunkPos startChunkPos )
    {
        Vector3Int d = new Vector3Int(startChunkPos.x, 0, startChunkPos.z);

        BlockType[,,] blocks = TerrainGenerator.chunks[startChunkPos].blocks;
        int width = blocks.GetLength(0);
        int height = blocks.GetLength(1);
        int depth = blocks.GetLength(2);

        List<Node> opneList = new List<Node>(); //���� �򰡵��� ���� ����Ʈ
   
        HashSet<Vector3Int> colseSet = new HashSet<Vector3Int>(); //�̹� �򰡵� ����Ʈ
      
        Node startNode = new Node(start, null, 0, Heuristic(start, goal));

        opneList.Add(startNode);
        while (opneList.Count > 0)
        {
            opneList.Sort((a, b) => a.F.CompareTo(b.F)); //Node.F���� ���������� ����
            Node current = opneList[0];
            opneList.RemoveAt(0);
            // ��ǥ���޽� ����
            if (current.Pos == goal)
            {
                return ReconstructPath(current);
            }

            colseSet.Add(current.Pos);
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighborPos = current.Pos + dir; // �̵��� ��ġ��
                if (colseSet.Contains(neighborPos)) continue; // �̹� Ž���� ����϶�
               
                if (!IsInBounds(neighborPos, width, height, depth)) continue; // �迭 ����°� ����

                if (blocks[neighborPos.x, neighborPos.y+1, neighborPos.z] != BlockType.Air) continue; // �̵��� ��ġ ���� air�϶� 
                
                int tentiveG = current.G + 1; // �ӽ� �̵���

                Node existingNode = opneList.Find(a => a.Pos == neighborPos); // openList�� neighborPos�� ���� ���� ������� ����
                if (existingNode == null) // ������ ����� �߰�
                {
                    Node newNode = new Node(neighborPos, current, tentiveG, Heuristic(neighborPos, goal));
                    opneList.Add(newNode);
                }
                else if (tentiveG < existingNode.G)//��ª�� ��� �߽߰� ����
                {
                    existingNode.G = tentiveG;
                    existingNode.Parent = current;
                }

            }
        }
        Debug.Log("��ξ���");
        return null;
    }
    private static bool IsInBounds(Vector3Int pos, int w, int y, int z) // �̵����� �迭 �������� Ȯ��
    {
        return pos.x >= 0 && pos.x < w &&
               pos.y >= 0 && pos.y < y-1 &&
               pos.z >= 0 && pos.z < z;
    }


    private static List<Vector3Int> ReconstructPath(Node endPos)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node current = endPos;
        while (current != null) //start�������� node�θ� �߰�
        {
            path.Add(current.Pos); // ��� ���
            current = current.Parent; // ���� ��忡 parent�� �ִٸ� current�� ���� ������ null
        }
        path.Reverse();

        return path;
    }

    // �޸���ƽ �Լ�:
    //                  ���� �Ÿ�	        ��Ŭ����� �Ÿ� sqrt((dx)^2 + (dy)^2)
    //                  �밢�� �̵� ���	ü����� �Ÿ� max(abs(dx), abs(dy))
    //                  ���� ��             ����ư �Ÿ� abs(dx) + abs(dy)
    private static int Heuristic(Vector3Int a, Vector3Int b) // a = start ,b = goal
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
  
}
