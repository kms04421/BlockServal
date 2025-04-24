using System.Collections.Generic;
using UnityEngine;

public static class AStarBlockPathfinder
{
    // 탐색할 6방향
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

        List<Node> opneList = new List<Node>(); //아직 평가되지 않은 리스트
   
        HashSet<Vector3Int> colseSet = new HashSet<Vector3Int>(); //이미 평가된 리스트
      
        Node startNode = new Node(start, null, 0, Heuristic(start, goal));

        opneList.Add(startNode);
        while (opneList.Count > 0)
        {
            opneList.Sort((a, b) => a.F.CompareTo(b.F)); //Node.F값이 작은순으로 정렬
            Node current = opneList[0];
            opneList.RemoveAt(0);
            // 목표도달시 중지
            if (current.Pos == goal)
            {
                return ReconstructPath(current);
            }

            colseSet.Add(current.Pos);
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighborPos = current.Pos + dir; // 이동할 위치값
                if (colseSet.Contains(neighborPos)) continue; // 이미 탐색한 경로일때
               
                if (!IsInBounds(neighborPos, width, height, depth)) continue; // 배열 벗어나는거 방지

                if (blocks[neighborPos.x, neighborPos.y+1, neighborPos.z] != BlockType.Air) continue; // 이동한 위치 블럭이 air일때 
                
                int tentiveG = current.G + 1; // 임시 이동값

                Node existingNode = opneList.Find(a => a.Pos == neighborPos); // openList에 neighborPos와 같은 값이 있을경우 저장
                if (existingNode == null) // 없으면 새노드 추가
                {
                    Node newNode = new Node(neighborPos, current, tentiveG, Heuristic(neighborPos, goal));
                    opneList.Add(newNode);
                }
                else if (tentiveG < existingNode.G)//더짧은 노드 발견시 변경
                {
                    existingNode.G = tentiveG;
                    existingNode.Parent = current;
                }

            }
        }
        Debug.Log("경로없음");
        return null;
    }
    private static bool IsInBounds(Vector3Int pos, int w, int y, int z) // 이동값이 배열 내부인지 확인
    {
        return pos.x >= 0 && pos.x < w &&
               pos.y >= 0 && pos.y < y-1 &&
               pos.z >= 0 && pos.z < z;
    }


    private static List<Vector3Int> ReconstructPath(Node endPos)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node current = endPos;
        while (current != null) //start지점까지 node부모 추가
        {
            path.Add(current.Pos); // 경로 담기
            current = current.Parent; // 현재 노드에 parent가 있다면 current에 적용 없으면 null
        }
        path.Reverse();

        return path;
    }

    // 휴리스틱 함수:
    //                  직선 거리	        유클리디안 거리 sqrt((dx)^2 + (dy)^2)
    //                  대각선 이동 허용	체비셰프 거리 max(abs(dx), abs(dy))
    //                  격자 맵             맨해튼 거리 abs(dx) + abs(dy)
    private static int Heuristic(Vector3Int a, Vector3Int b) // a = start ,b = goal
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
  
}
