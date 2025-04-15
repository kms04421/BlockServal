using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Node // ��� Ž���� ����ϴ� ��� Ŭ����
{
    public Vector3Int Pos;      // ����� ��ǥ
    public Node Parent;         // ���� ��� 
    public int G;               // ���������κ����� ���� �Ÿ�
    public int H;               // ��ǥ������ ���� �Ÿ�
    public int F => G + H;      // �� ��� 

    public Node(Vector3Int pos, Node parent, int g, int h) 
    {
        Pos = pos;
        Parent = parent;
        G = g;
        H = h;
    }


}
