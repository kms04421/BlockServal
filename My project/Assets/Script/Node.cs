using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Node // 경로 탐색에 사용하는 노드 클래스
{
    public Vector3Int Pos;      // 노드의 좌표
    public Node Parent;         // 이전 노드 
    public int G;               // 시작점으로부터의 실제 거리
    public int H;               // 목표까지의 예상 거리
    public int F => G + H;      // 총 비용 

    public Node(Vector3Int pos, Node parent, int g, int h) 
    {
        Pos = pos;
        Parent = parent;
        G = g;
        H = h;
    }


}
