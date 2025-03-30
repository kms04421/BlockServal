using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{
    public const int chunkWidth = 16;
    public const int chunkHeight = 64;
    public BlockType[,,] blocks = new BlockType[chunkWidth + 2, chunkHeight, chunkWidth + 2];

    public void BuildMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int x = 1; x < chunkWidth + 1; x++)
            for (int z = 1; z < chunkWidth + 1; z++)
                for (int y = 0; y < chunkHeight; y++)
                {
                    int numFaces = 0; // �������� ���� ����
                    if (blocks[x, y, z] != BlockType.Air)
                    {
                    
                        Vector3 blockPos = new Vector3(x - 1, y, z - 1);
                        //������ �����ΰ� ã�Ƽ� �߰� 
                        //��
                        if (y < chunkHeight - 1 && blocks[x, y + 1, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            numFaces++;

                            uvs.AddRange(GetTextureRect(1));
                        }
                        //�Ʒ�
                        if (y > 0 && blocks[x, y - 1, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(2));
                        }


                        //������
                        if (blocks[x + 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //��
                        if (blocks[x, y, z - 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 1, 0));
                            verts.Add(blockPos + new Vector3(1, 0, 0));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //��
                        if (blocks[x, y, z + 1] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(1, 0, 1));
                            verts.Add(blockPos + new Vector3(1, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        //����
                        if (blocks[x - 1, y, z] == BlockType.Air)
                        {
                            verts.Add(blockPos + new Vector3(0, 0, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 1));
                            verts.Add(blockPos + new Vector3(0, 1, 0));
                            verts.Add(blockPos + new Vector3(0, 0, 0));
                            numFaces++;
                            uvs.AddRange(GetTextureRect(0));
                        }

                        int tl = verts.Count - 4 * numFaces; //�Ѹ�� verts�� 4���� �߰��ϴϱ� verts.Count - 4 * numFaces�� �Ұ�� ���� ó�� �߰��� ����������
                        for (int i = 0; i < numFaces; i++) //�Ѹ�� ������ 4�� ������ �׸鿡 ���� �ﰢ�� �ΰ� �迭�� �����ؼ� tris�߰� 
                        {
                            tris.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                         
                        }
                    }

                }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
       
            // �ؽ�ó ����
        GetComponent<MeshRenderer>().material.mainTexture = SpriteAtlasManager.GetBlockSprite(0).texture;
        
    }
    private Vector2[] GetTextureRect(int i)
    {
        Sprite blockSprite = SpriteAtlasManager.GetBlockSprite(i);
        Rect textureRect = blockSprite.textureRect; // ��� ��������Ʈ
        Texture tex = blockSprite.texture;
        // �ؽ�ó�� UV ��ǥ ���
        float uvXMin = textureRect.x / tex.width;
        float uvXMax = (textureRect.x + textureRect.width) / tex.width;
        float uvYMin = textureRect.y / tex.height;
        float uvYMax = (textureRect.y + textureRect.height) / tex.height;

        // UV ��ǥ ��ȯ
        return new Vector2[]
        {
           new Vector2(uvXMax, uvYMin), // �»�
        new Vector2(uvXMax, uvYMax), // ���
        new Vector2(uvXMin, uvYMax), // ����
        new Vector2(uvXMin, uvYMin)  // ����
        };
    }

}
