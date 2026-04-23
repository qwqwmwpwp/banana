using System;
using System.Reflection;
using UnityEngine;

public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock) // 线程安全
            {
                if (_instance == null)
                {
                    _instance = new T();

                }
                return _instance;
            }
        }
    }


    protected Singleton()
    {
        
    }

 
}