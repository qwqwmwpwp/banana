using System.Collections.Generic;
using Enemy.Boss.Altar_Boos_2_.State;
using GameManager;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace Enemy.Boss.Altar_Boos_2_.Status
{
    public class AltarStatus:CharacterStats
    {
        [Header("血条")]
        public GameObject UI_healthBar;
        [Header("无敌")]
        public bool isInvincible;        //是否无敌
        AltarController controller;
        private Dictionary<AltarStateType,BuffType> altarStates = new Dictionary<AltarStateType, BuffType>();
        private Slider slider;
        protected override void Start()
        {
            altarStates.Add(AltarStateType.FmFire,BuffType.锋芒之火);
            altarStates.Add(AltarStateType.ZrFire,BuffType.焦燃之火);
            base.Start();
            controller = GetComponent<AltarController>();
            slider = UI_healthBar.GetComponentInChildren<Slider>();
            
        }

        public void SetInvincible(bool value)
        {
            isInvincible = value;
        }

        public  void CheckBuff()
        {
            //无敌和死亡状态  不处理
            if (isInvincible || controller.CurrentState == AltarStateType.Die)
            {
                return;
            }
            if (!BuffManager.instance.HasBuff(altarStates[controller.CurrentState]))
            {  
                AudioManager.instance.PlaySFX(37, controller.transform.position);
                // 判断当前状态播放受伤动画
                switch (controller.CurrentState)
                {
                    case AltarStateType.Invincible:
                        controller.SetAnim(controller.InvincibleAttack, false);
                        break;
                    case AltarStateType.ZrFire:
                        controller.SetAnim(controller.ZrAttack, false);
                        break;
                    case AltarStateType.FmFire:
                        controller.SetAnim(controller.FmAttack, false);
                        break;
                }
                EventCenter.Instance.CallEvent("主角死亡");
                return;
            }
            else
            {
                slider.value -= 0.5f;
                // 判断当前状态播放受伤动画
                switch (controller.CurrentState)
                {
                   
                    case AltarStateType.ZrFire:
                        controller.RemoveShill(AltarStateType.ZrFire);
                        controller.ChangeState(controller.GetRandomRarity());
                        break;
                    case AltarStateType.FmFire:
                        controller.RemoveShill(AltarStateType.FmFire);
                        controller.ChangeState(controller.GetRandomRarity());
                        break;
                }
            }
        }
    }
}