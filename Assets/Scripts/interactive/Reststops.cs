using System.Collections;
using interactive.Interface;
using UnityEngine;

public class Reststops : MonoBehaviour, IInteractable
{
    private PlayerStats _plStats;
    private PlayerController pl;
    private bool _isInteractable;
    private Coroutine recoverIE;
    [Header("UI")]
    public GameObject RestUItext;
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed = 1f;
    
    [Header("恢复速度")]
    public float recoverSpeed = 5f;

    private void Update()
    {
       // if (_isInteractable && pl != null && pl.playerControls.GamePlay.Interact.triggered)
        if (_isInteractable && pl != null)
        {
            // 协程未启动时才允许触发
            if (recoverIE == null)
            {
                Debug.Log("开始恢复");
                recoverIE = StartCoroutine(Recover());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (CanInteract() && other.TryGetComponent(out PlayerController player))
        {
            StartCoroutine(ShowPanel(RestUItext));
            _isInteractable = true;
            pl = player;
            _plStats = pl.playerStats;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            _isInteractable = false;
            StartCoroutine(HidePanel(RestUItext));
            if (recoverIE != null)
            {
                Debug.Log("停止恢复");
                StopCoroutine(recoverIE);
                recoverIE = null;
            }
        }
    }

  private  IEnumerator Recover()
    {
        while (true)
        {
            Debug.Log("恢复");
            _plStats.RestoreHealth(0.1f);
            yield return new WaitForSeconds(recoverSpeed);
        }
    }
//UI显示关闭协程
    IEnumerator ShowPanel(GameObject panel)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            panel.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }
    IEnumerator HidePanel(GameObject panel)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            panel.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.deltaTime * animationSpeed;
            yield return null;
        }
    }
    public bool CanInteract()
    {
        return true;
    }
}