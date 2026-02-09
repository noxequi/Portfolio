using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHPBarController : MonoBehaviour
{
    [SerializeField] private PlayerDamage player;

    [SerializeField] private Image hpBar;
    [SerializeField] private Image burnBar;

    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float strength = 20f;
    [SerializeField] private int vibrate = 100;
    [SerializeField] private float burnDelay = 0.5f;
    [SerializeField] private float burnDuration = 0.25f;

    private void Start()
    {
        player.OnHPChanged += UpdateGauge;
        UpdateGauge(player.HP, player.MaxHP);
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHPChanged -= UpdateGauge;
        }
    }

    private void UpdateGauge(float currentHp, float maxHp)
    {
        float fillValue = (maxHp > 0) ? currentHp / maxHp : 0f;

        if (hpBar.fillAmount > fillValue)
        {
            transform.DOShakePosition(duration / 2f, strength, vibrate);
        }

        hpBar.DOFillAmount(fillValue, duration)
            .OnComplete(() =>
            {
                burnBar.DOFillAmount(fillValue, burnDuration)
                    .SetDelay(burnDelay);
            });
    }
}
