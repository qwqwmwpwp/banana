﻿using System.Collections;
using System.Collections.Generic;
using GameManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    private PlayerController player;
    
   
    [Header("护甲模块")]
    public float recoverStateCooldown; //恢复状态冷却
    private bool isRecoverArmor;       //是否正在恢复
    public float ArmorCooldown;        //回复冷却
    public float ArmorCooldownTimer;

    [Header("大招模块")]
    public float currrentUltimateEnergy; //目前大招能量
    public float UltimateDuration = 10;  //大招持续时间
    private float UltimateEnergy = 100;  //最大能量

    public Stat naturalRecovery;         //自然恢复量
    [SerializeField] private float RecoveryCooldown = 2; 
    private float RecoveryCooldownTimer; 

    public Stat ultimateFreezeDuration;  //大招冻结时间

    public bool isInvincibility; // 是否处于无敌帧

    private void Awake()
    {
        armor.SetValue(12);
    }

    protected override void Start()
    {
        base.Start();
        player = GetComponent<PlayerController>();
        EventCenter.Instance.AddEventListener("主角死亡", Die);

    }

    protected override void Update()
    {
        ArmorCooldownTimer -= Time.deltaTime;
        RecoveryCooldownTimer -= Time.deltaTime;

        ArmorNaturalRecovery();    //护甲自然恢复
        UltimateRecovery();        //大招自然恢复

    }

    private void ArmorNaturalRecovery()//护甲自然恢复
    {
        if (currrentArmor < armor.GetValue() && !isRecoverArmor)
        {
            ArmorCooldownTimer = ArmorCooldown + recoverStateCooldown;

            isRecoverArmor = true;
        }

        if (isRecoverArmor && ArmorCooldownTimer < 0 && currrentArmor < armor.GetValue())
        {
            currrentArmor++;
            UIManager.instance.GameUI.UpdateArmorUI();

            ArmorCooldownTimer = ArmorCooldown;
        }
    }

    public void SetArmorRecovery(int _armor)//护甲主动回复
    {
        if (currrentArmor < armor.GetValue())
        {
            currrentArmor += _armor;

            if (currrentArmor > armor.GetValue())
                currrentArmor = armor.GetValue();

            UIManager.instance.GameUI.UpdateArmorUI();

        }
    }

    private void UltimateRecovery()
    {
        if (RecoveryCooldownTimer < 0 && !player.ultimateIsFreeze && !player.DoingUltimateSkill)
        {
            RestoreEnergy(naturalRecovery.GetValue());
            RecoveryCooldownTimer = RecoveryCooldown;
        }
    }

    public void RestoreEnergy(int restoreAmount)
    {
       if(currrentUltimateEnergy < UltimateEnergy && !player.ultimateIsFreeze)
       {
            currrentUltimateEnergy += restoreAmount;

            if(currrentUltimateEnergy > UltimateEnergy)
            {
                currrentUltimateEnergy = UltimateEnergy;
                AudioManager.instance.PlaySFX(22);

            }
        }
        UIManager.instance.GameUI.UpdateUltimateEnergyUI();
    }

    public override void DoDamage(CharacterStats _targetStats, AttackType attackType, float AttackMultiplier)
    {
        _targetStats.GetComponent<EnemyBase>().SetKnockbackDir(transform);

        base.DoDamage(_targetStats, attackType, AttackMultiplier);

    }

   public override void TakeDamage(int _damage, AttackType attackType)
    {
        if (isInvincibility)
            return;

        //若有护甲剩余，伤害为0并扣除一层护甲，否则将根据伤害扣除血量
        _damage = ChackAramor(_damage);

        if(_damage!=0)
            player.stateMachine.ChangeState(player.hitState);
        else
        {
            player.AramorFX.SetActive(true);
        }

        //更新生命值
        UpdateHealth(_damage);

        if (currrentHealth <= 0 && !isDie)
        {
            Die();
            Debug.Log("Die");
        }

        if (currrentHealth <= 0)
            return;

        
        UIManager.instance.GameUI.UpdateArmorUI();
    }



    public override void Die()
    {
        if (isDie) return;
        isDie = true;

        // 当前关卡死亡次数加1
        PlayerManager.instance.deathNumber++;

        player.stateMachine.ChangeState(player.deadState);
    }
    

    
    

}