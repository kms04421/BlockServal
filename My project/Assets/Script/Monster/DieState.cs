using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : MonsterState
{
    public DieState(Monster monster) : base(monster) { }


    public override void Enter()
    {
        Debug.Log("Die 상태 진입");
        GameObject.Destroy(monster.gameObject); // 간단 처리
    }
}
