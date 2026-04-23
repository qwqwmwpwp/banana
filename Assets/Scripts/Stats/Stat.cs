using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Stat
{
    [SerializeField] private int baseValue;//��ֵ

    public List<int> modifiers = new List<int>();


    public int GetValue()//�����ṩ���� finalValue �Ľӿڡ�
    {
        int finalValue = baseValue;

        foreach (int Value in modifiers)
        {
            finalValue += Value;
        }

        return finalValue;
    }

    public void SetValue(int _Value)
    {
        baseValue = _Value;
    }

    public void AddModifier(int _modifier)//�������η�
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)//ɾ�����η�
    {
        modifiers.Remove(_modifier);
    }
}
