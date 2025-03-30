using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasManager : MonoBehaviour
{
    public static Sprite[] sprites;

    private void Awake()
    {
        SpriteAtlas spriteAtlas = Resources.Load<SpriteAtlas>("SpriteAtlas/Block");

        //��Ʋ�󽺿��� ��������Ʈ�� �迭�� ����
        if (spriteAtlas != null)
        {
            sprites = new Sprite[spriteAtlas.spriteCount];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = spriteAtlas.GetSprite("block" + i);
            }
        }
    }

    public static Sprite GetBlockSprite(int index) //��������Ʈ ���� �ε����� ã�ƿ���
    {
        if (index >= 0 && index < sprites.Length)
        {
            return sprites[index];  //��ȿ�� �ε��� ���� ���� ��������Ʈ ��ȯ
        }
        else
        {           
            return null;
        }
    }
    
}
