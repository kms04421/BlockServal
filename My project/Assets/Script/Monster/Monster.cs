using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player; // 플레이어좌표
    private Vector3 lastPlayerPos; // 마지막 플레이어좌표
    List<ChunkPos> chunkPath; // 청크 경로 저장용
    public MonsterData data; // 몬스터 데이터
    private float currentHP;

    private MonsterState currentState;
     void Start()
    {
        currentHP = data.maxHP;
        player = GameManager.Instance.player.transform;
        ChangeState(new ChaseState(this));
    }

    void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(MonsterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    public void Hit(float dam)
    {
        if(currentHP - dam < 0)
        {
            currentHP = currentHP - dam;
        }
        else
        {
            currentHP = 0;
            ChangeState(new DieState(this));
        }
      
    }


}
