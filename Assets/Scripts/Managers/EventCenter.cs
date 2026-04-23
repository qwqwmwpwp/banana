using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
   public class EventCenter :SingletonMono<EventCenter>
   {
      public interface IEvent
      {
      
      }
      public Dictionary<string, IEvent> eventsCenter = new Dictionary<string, IEvent>();
      public class Event : IEvent
      {
         private event Action _action;
         public Event(Action action)
         {
            _action = action;
         }

         public void Call()
         {
            _action?.Invoke();
         }
         public void Add(Action action)
         {
            _action += action;
         }
         public void Remove(Action action)
         {
            _action -= action;
         }
         public void Clear()
         {
            _action = null;
         }
      }
      public class Event<T>:IEvent
      {
         private event Action<T> _action;
         public Event(Action<T> action)
         {
            _action = action;
         }

         public void Call(T  parameter)
         {
            _action?.Invoke(parameter);
         }
         public void Add(Action<T> action)
         {
            _action += action;
         }
         public void Remove(Action<T> action)
         {
            _action -= action;
         }
         public void Clear()
         {
            _action = null;
         }
      }
      public class Event<T,T1>:IEvent
      {
         private event Action<T,T1> _action;
         public Event(Action<T,T1> action)
         {
            _action = action;
         }

         public void Call(T  parameter,T1 parameter1)
         {
            _action?.Invoke(parameter,parameter1);
         }
         public void Add(Action<T,T1> action)
         {
            _action += action;
         }
         public void Remove(Action<T,T1> action)
         {
            _action -= action;
         }
         public void Clear()
         {
            _action = null;
         }
      }
      //三个
      public class Event<T1, T2, T3> : IEvent
      {
         private event Action<T1, T2, T3> _action;
         public Event(Action<T1, T2, T3> action)
         {
            _action = action;
         }
         public void Call(T1 parameter1, T2 parameter2, T3 parameter3)
         {
            _action?.Invoke(parameter1, parameter2, parameter3);
         }

         public void Add(Action<T1, T2, T3> action)
         {
            _action += action;
         }
         public void Remove(Action<T1,T2,T3> action)
         {
            _action -= action;
         }
      }
      //五个
      public class Event<T1, T2, T3, T4, T5> : IEvent
      {
         private event Action<T1, T2, T3, T4, T5> _action;
    
         public Event(Action<T1, T2, T3, T4, T5> action)
         {
            _action = action;
         }

         public void Call(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5)
         {
            _action?.Invoke(parameter1, parameter2, parameter3, parameter4, parameter5);
         }
    
         public void Add(Action<T1, T2, T3, T4, T5> action)
         {
            _action += action;
         }
    
         public void Remove(Action<T1, T2, T3, T4, T5> action)
         {
            _action -= action;
         }
    
         public void Clear()
         {
            _action = null;
         }
      }
  

      #region 无参

      public void CallEvent(string eventName)
      {
         if(eventsCenter.TryGetValue(eventName,out var _event))
         {
            (_event as Event)?.Call();
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      public void AddEventListener(string eventName, Action action)
      {
         if (!eventsCenter.TryGetValue(eventName, out var _event))
         {
            //没有
            eventsCenter.Add(eventName, new Event(action));
         }
         else
         {
            (_event as Event)?.Add(action);
         }
      }
      public void RemoveEventListener(string eventName, Action action)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event)?.Remove(action);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      #endregion
  
      #region 有一个参数

      public void AddEventListener<T>(string eventName,Action<T> action)
      {
         if (!eventsCenter.TryGetValue(eventName, out var _event))
         {
            eventsCenter.Add(eventName, new Event<T>(action));
         }
         else
         {
            (_event as Event<T>)?.Add(action);
         }
      }
      public void RemoveEventListener<T>(string eventName, Action<T> action)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T>)?.Remove(action);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      public void CallEvent<T>(string eventName,T parameter)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T>)?.Call(parameter);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      #endregion
   
      #region 有两个参数
   
      public void AddEventListener<T,T1>(string eventName,Action<T,T1> action)
      {
         if (!eventsCenter.TryGetValue(eventName, out var _event))
         {
            eventsCenter.Add(eventName, new Event<T,T1>(action));
         }
         else
         {
            (_event as Event<T,T1>)?.Add(action);
         }
      }
      public void RemoveEventListener<T,T1>(string eventName, Action<T,T1> action)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T,T1>)?.Remove(action);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      public void CallEvent<T,T1>(string eventName,T parameter,T1 parameter1)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T,T1>)?.Call(parameter,parameter1);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      #endregion

      #region 三个参数
      public void AddEventListener<T1,T2,T3> (string eventName, Action<T1,T2,T3> action)
      {
         if (!eventsCenter.TryGetValue(eventName, out var _event))
         {
            eventsCenter.Add(eventName, new Event<T1,T2,T3>(action));
         }
         else
         {
            (_event as Event<T1,T2,T3>)?.Add(action);
         }
      }
      public void RemoveEventListener<T1,T2,T3> (string eventName, Action<T1,T2,T3> action)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T1,T2,T3>)?.Remove(action);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      public void CallEvent<T1,T2,T3>(string eventName,T1 parameter1,T2 parameter2,T3 parameter3)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T1,T2,T3>)?.Call(parameter1,parameter2,parameter3);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
   
   

      #endregion
      #region 五个参数
      public void AddEventListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
      {
         if (!eventsCenter.TryGetValue(eventName, out var _event))
         {
            eventsCenter.Add(eventName, new Event<T1, T2, T3, T4, T5>(action));
         }
         else
         {
            (_event as Event<T1, T2, T3, T4, T5>)?.Add(action);
         }
      }

      public void RemoveEventListener<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> action)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T1, T2, T3, T4, T5>)?.Remove(action);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }

      public void CallEvent<T1, T2, T3, T4, T5>(string eventName, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5)
      {
         if (eventsCenter.TryGetValue(eventName, out var _event))
         {
            (_event as Event<T1, T2, T3, T4, T5>)?.Call(parameter1, parameter2, parameter3, parameter4, parameter5);
         }
         else
         {
            Debug.LogError("事件不存在");
         }
      }
      #endregion
   
   }
}
