using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : MonoBehaviour
{
   public static MonoManager Instance; //单例
   private void Awake()
   {
       if (Instance != null)
       {
           Destroy(gameObject);
           return;
       }
       Instance = this;
   }
   public Action OnUpdate;

   public Action OnLateUpdate;

   public  Action OnFixedUpdate;

   public void AddUpdate(Action action)
   {
       OnUpdate += action;
   }
   public void RemoveUpdate(Action action)
   {
       OnUpdate -= action;
   }
   
   public void AddLateUpdate(Action action)
   {
       OnLateUpdate += action;
   }
    public void RemoveLateUpdate(Action action)
   {
       OnLateUpdate -= action;
   }
    public void AddFixedUpdate(Action action)
   {
       OnFixedUpdate += action;
   }

   

    public void RemoveFixedUpdate(Action action)
   {
       OnFixedUpdate -= action;
   }
    
    void Start()
    {
        
    }

  
    void Update()
    {
        OnUpdate?.Invoke();
    }
    void LateUpdate()
    {
        OnLateUpdate?.Invoke();
    }
    void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
}

