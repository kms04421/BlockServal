using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : MonsterState
{
    public AttackState(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("AttackState 상태 진입");
    }

    public override void Update()
    {
        monster.ChangeState(new ChaseState(monster));
    }
}
