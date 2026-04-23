using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStateMachine : MonoBehaviour
{
    // 当前状态
    public PlayerState currentState; 

    public void Initialize(PlayerState playerState)
    {
        currentState = playerState;
        currentState.Enter();

    }

    public void ChangeState(PlayerState _newState)//更改状态
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();

        Debug.Log(currentState);
    }

}
