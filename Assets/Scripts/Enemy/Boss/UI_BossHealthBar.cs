using UnityEngine;
using UnityEngine.UI;

public class UI_BossHealthBar : MonoBehaviour
{
    private Slider slider;
    public CharacterStats myStats;

    private void Start()
    {
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