using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public int maxHP;
    public int damage;
    public float moveSpeed;
    public float attackRange;
    //public GameObject dropItemPrefab; //추후 추가
}
