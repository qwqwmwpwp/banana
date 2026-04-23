using System.Collections.Generic;
using UnityEngine;

public interface IAttackInteractable 
{
    bool CanBeAttacked(AttackContext ctx);
    void OnAttacked(AttackContext ctx);
}

public class AttackContext
{
    public GameObject Attacker;
    public Dictionary<string, string> Context { get; private set; }

    public AttackContext(GameObject attacker)
    {
        Attacker = attacker;
        Context = new Dictionary<string, string>();
    }

    public void AddContext(string key, string value)
    {
        if (Context.ContainsKey(key))
        {
            Context[key] = value;
        }
        else
        {
            Context.Add(key, value);
        }
    }

    public bool HasContext(string key, string exceptedValue)
    {
        return Context.TryGetValue(key, out var actualValue) && actualValue == exceptedValue;
    }

    public string GetContext(string key)
    {
        if (Context.ContainsKey(key))
        {
            return Context[key];
        }
        else
        {
            return null;
        }
    }
}
