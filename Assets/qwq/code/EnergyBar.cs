using qwq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] List<Slider> sliders;


    private void Update()
    {

        if (Player.playerctx == null) return;

        float energy = Player.playerctx.energy;
        foreach (var slider in sliders) {
            if (energy >= 1)
            {
                slider.value = 1;
                energy -= 1;
            }
            else
            {
                slider.value = energy;
                energy = 0;
            }

        }
    }
}
