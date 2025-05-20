using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MonsterState
{
    public IdleState(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("IdleState ���� ����");
    }

    public override void Update()
    {
         monster.ChangeState(new ChaseState(monster));
    }
}
