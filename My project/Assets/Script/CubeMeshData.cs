using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMeshData 
{
    public static readonly Vector3[] Vertices = {
        new Vector3 (0, 0, 0),
        new Vector3 (1, 0, 0),
        new Vector3 (1, 1, 0),
        new Vector3 (0, 1, 0),
        new Vector3 (0, 1, 1),
        new Vector3 (1, 1, 1),
        new Vector3 (1, 0, 1),
        new Vector3 (0, 0, 1),
    };

    public static readonly int[] Triangles = {
        0, 2, 1,  0, 3, 2,
        2, 3, 4,  2, 4, 5,
        1, 2, 5,  1, 5, 6,
        0, 7, 4,  0, 4, 3,
        5, 4, 7,  5, 7, 6,
        0, 6, 7,  0, 1, 6
    };
}
