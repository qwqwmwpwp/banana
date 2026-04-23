using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealthBar : MonoBehaviour
{
    private Slider slider;
    private CharacterStats myStats;

    private void Start()
    {
        myStats = PlayerManager.instance.player.playerStats;

        slider = GetComponentInChildren<Slider>();

        myStats.onHealthChanged += UpdateHealthUI;
    }



    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.maxHealth.GetValue();
        slider.value = myStats.currrentHealth;
    }

    private void OnDisable()
    {
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}
