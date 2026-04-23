using UnityEngine;

public interface IPassiveInteratable
{
    void OnPlayerEnter(PlayerStats player);
    void OnPlayerExit(PlayerStats player);
}
