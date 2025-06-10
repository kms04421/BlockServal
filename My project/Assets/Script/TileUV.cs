using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileUV 
{
    Vector2[] uvs;

    public Vector2[] GetUvs() => uvs;
  
    public TileUV(Tile tile)
    {
        uvs = GetTextureRect((int)tile);
    }
    public Vector2[] GetTextureRect(int i) // ��������Ʈ ��Ʋ�󽺿��� ������ �ؽ��� uv����
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
    public static Dictionary<Tile, TileUV> tiles = new Dictionary<Tile, TileUV>()
    {
        {Tile.Grass,new TileUV(Tile.Grass)},
        {Tile.Dirt,new TileUV(Tile.Dirt)},
        {Tile.GrassSide,new TileUV(Tile.GrassSide)},
        {Tile.Trunk,new TileUV(Tile.Trunk)},
        {Tile.TrunkSide,new TileUV(Tile.TrunkSide)},
        {Tile.Leaves,new TileUV(Tile.Leaves)},
        {Tile.Stone,new TileUV(Tile.Stone)}
    };
}
public enum Tile {GrassSide, Grass, Dirt,Trunk,TrunkSide,Leaves,Stone}