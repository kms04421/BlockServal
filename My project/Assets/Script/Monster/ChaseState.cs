using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : MonsterState
{
    public ChaseState(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Chase 상태 진입");
        monster.StartCoroutine(monster.MonsterUpdate());
    }

    public override void Update()
    {
        if (Vector3.Distance(monster.transform.position, monster.player.position) < monster.data.attackRange)
        {
            monster.ChangeState(new AttackState(monster));
        }        
    }

    public override void Exit()
    {
        monster.StopAllCoroutines(); 
    }
}
