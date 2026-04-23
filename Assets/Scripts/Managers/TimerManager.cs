using System;

using System.Collections.Generic;
using GameManager;
using UnityEngine;


public class TimerManager : SingletonMono<TimerManager>
{
    [SerializeField] private int initMaxTimer;   //  初始化最大计时器数量

    private Queue<GameTimer> _notworkTimers = new Queue<GameTimer>();  
    private List<GameTimer> _workTimers = new List<GameTimer>();

    private void Start()
    {
        InitTimers();
    }
    private void Update()
    {
        UpdateWorkingTimer();
    }

    private void InitTimers()
    {
        for (int i = 0; i < initMaxTimer; i++)
            CreateTimer();
    }

    private void CreateTimer()
    {
        var Timer = new GameTimer();
        _notworkTimers.Enqueue(Timer);
    }
    /// <summary>
    /// 尝试获取计时器
    /// </summary>
    /// <param name="time">计时器时间</param>
    /// <param name="action">计时完成执行事件</param>
    public void TryGetTimer(float time, Action action)
    {
        if (_notworkTimers.Count == 0)
        {
            CreateTimer();
            var timer = _notworkTimers.Dequeue();
            timer.StartTimer(time, action);
            _workTimers.Add(timer);
               
        }
        else
        {
            var timer = _notworkTimers.Dequeue();
            timer.StartTimer(time, action);
            _workTimers.Add(timer);
        }
    }

    public void UpdateWorkingTimer()
    {
        for (int i = 0; i < _workTimers.Count; i++)
        {
            var Timer = _workTimers[i];
            if (Timer.GetTimerState() == TimerState.Working)
            {
                Timer.UpdateTimer();
                  
            }
            else
            {
                _workTimers[i].ResetTimer();
                _workTimers.Remove(Timer);
                _notworkTimers.Enqueue(Timer);
            }
        }
    }
}