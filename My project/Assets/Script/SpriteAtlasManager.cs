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

        //아틀라스에서 스프라이트를 배열에 저장
        if (spriteAtlas != null)
        {
            sprites = new Sprite[spriteAtlas.spriteCount];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = spriteAtlas.GetSprite("block" + i);
            }
        }
    }

    public static Sprite GetBlockSprite(int index) //스프라이트 정보 인덱스로 찾아오기
    {
        if (index >= 0 && index < sprites.Length)
        {
            return sprites[index];  //유효한 인덱스 범위 내의 스프라이트 반환
        }
        else
        {           
            return null;
        }
    }
    
}
