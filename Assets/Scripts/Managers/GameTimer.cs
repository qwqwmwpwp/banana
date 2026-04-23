using System;
using UnityEngine;

namespace GameManager
{

    public enum TimerState
    {
        NotWorking,
        Working,
        Done,
    }
    public class GameTimer
    {
        //开始计时
        //重置计时
        //计时完成执行任务
        // 停止计时
        private float _time;      //需要时间
        private bool _isStoping;  //是否停止
        private Action _task;   
        private TimerState _state;   //当前状态
        public GameTimer()
        {
            ResetTimer();
        }
        public TimerState GetTimerState() => _state;
        public void ResetTimer()
        {
            _time = 0f;
            _isStoping = true;
            _task = null;
            _state = TimerState.NotWorking;
        }
        public void StartTimer(float time, Action action)
        {
            _time  = time;
            _isStoping = false;
            _task = action;
            _state = TimerState.Working;
        }

        public void UpdateTimer()
        {
            if(_isStoping) return;
            _time-=  Time.deltaTime;
            if (_time <= 0f)
            {
                _task?.Invoke();
                _state = TimerState.Done;
                _isStoping  = true;
            }
           
        }
    }
}