using qwq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour, IInteraction
{
    public void Trigger(GameObject gObj)
    {
        if (Player.playerctx == null) return;

        Player.playerctx.EnergyValueUPdate(Player.playerctx.energy_max / 2);
        Destroy(gameObject);
    }
}
