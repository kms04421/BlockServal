using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : MonsterState
{
    public ChaseState(Monster monster) : base(monster) { }

    public override void Enter()
    {
        Debug.Log("Chase ���� ����");
    }

    public override void Update()
    {
        monster.ChangeState(new ChaseState(monster));
    }
}
