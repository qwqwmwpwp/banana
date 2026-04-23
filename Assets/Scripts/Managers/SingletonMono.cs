using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//挂载物体的单例模式
public class SingletonMono<T> : MonoBehaviour where T :SingletonMono<T>
{
    public static  T Instance;
    public virtual  void Awake()
    {
        DontDestroyOnLoad(gameObject);
     
        if(Instance== null)
           Instance = (T)this;
        
    }
    public void OnDestroy()
    {
        Destroy();
    }
    public void Destroy()
    {
        Instance= null;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
