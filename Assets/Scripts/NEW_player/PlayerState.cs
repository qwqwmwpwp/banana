using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{

    protected PlayerStateMachine stateMachine; // 状态机

    protected PlayerController player;

    
    protected Rigidbody2D rb; // 用于控制速度

    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
    }

    public virtual void Enter()
    {
        rb = player.rb;


    }

    public virtual void Update()
    {
        if (Time.timeScale == 0)
            return;

       
    }
    

    public virtual void FixedUpdate()
    {

    }
    public virtual void Exit()
    {
    }


}
