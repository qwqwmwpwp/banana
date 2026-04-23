using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimationEvent : MonoBehaviour
{
    public void SetActiveTrue() => gameObject.SetActive(true); 
    public void SetActiveFalse() => gameObject.SetActive(false); 
    public void SetActiveParentFalse() => transform.parent.gameObject.SetActive(false); 
    public void SetDestroy() => Destroy(gameObject); 

}
