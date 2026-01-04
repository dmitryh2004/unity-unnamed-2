using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirusController : UpgradableItem
{
    public static VirusController Instance;
    int currentHP;
    int currentAttack;
    bool alive = true;

    [Header("Hover Animation")]
    bool hoverAnimationActive = false;
    int hoverAnimationHP = 0;
    bool hoverAnimationNotEnoughHP = false;
    float hoverAnimationTimer = 0f;
    [SerializeField] float hoverAnimationDuration = 0.5f;
    [Space]
    [SerializeField] float hoverAnimationHPBarMinAlpha = .5f;
    [Space]
    [SerializeField] Color NotEnoughHPColor1, NotEnoughHPColor2;
    [Space]
    [Tooltip("Задержка в проигрывании анимации при изменении хп")]
    [SerializeField] float hoverAnimationDelay = 0.5f;

    Coroutine delayCoroutine = null;

    [Header("Low HP Animation")]
    [SerializeField] int lowHPMaxValue = 20;
    [SerializeField] float lowHPAnimationDuration = .5f;
    [SerializeField] Color lowHPColor1, lowHPColor2;
    bool lowHPAnimationActive = false;
    float lowHPAnimationTimer = 0f;

    float hpRatio, attackRatio;
    const int maxHP = 150, maxAttack = 45;

    [Header("Links")]
    [SerializeField] Image hpBar;
    [SerializeField] Image attackBar;
    [SerializeField] TMP_Text hpText, attackText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ResetToStart();
    }

    protected override void OnSetLevel()
    {
        base.OnSetLevel();
        ResetToStart();
    }

    public void ResetToStart()
    {
        currentHP = (int)GetUpgradableValue(0);
        currentAttack = (int)GetUpgradableValue(1);
        alive = true;
        hoverAnimationActive = false;
        hoverAnimationHP = 0;
        hoverAnimationNotEnoughHP = false;
        hoverAnimationTimer = 0f;

        lowHPAnimationActive = false;
        lowHPAnimationTimer = 0f;

        UpdateBars();
    }

    bool HasActiveEncryptionBonus()
    {
        Bonus additionalEncryptionBonus = null;

        for (int i = 1; i < 4; i++)
        {
            additionalEncryptionBonus = BonusController.Instance.GetBonus(i);
            if (additionalEncryptionBonus != null)
            {
                if (additionalEncryptionBonus is AdditionalEncryption ae)
                {
                    if (ae.IsActive())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    Bonus GetActiveEncryptionBonus()
    {
        Bonus additionalEncryptionBonus = null;

        for (int i = 1; i < 4; i++)
        {
            additionalEncryptionBonus = BonusController.Instance.GetBonus(i);
            if (additionalEncryptionBonus != null)
            {
                if (additionalEncryptionBonus is AdditionalEncryption ae)
                {
                    if (ae.IsActive())
                    {
                        return ae;
                    }
                }
            }
        }
        return null;
    }

    public void SetHoverAnimation(bool active, int nodeAttack, int nodeHP)
    {
        hoverAnimationActive = active;
        if (!active && delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
        hoverAnimationHP = hoverAnimationActive ? currentHP - nodeAttack : currentHP;

        bool canKill = nodeHP <= currentAttack && !HasActiveEncryptionBonus();

        hoverAnimationNotEnoughHP = hoverAnimationActive && hoverAnimationHP <= 0 && !canKill;
    }

    public void TakeDamage(int damage, bool ignoreEncryption = false)
    {
        bool useAEBonus = HasActiveEncryptionBonus();

        if (useAEBonus)
        {
            HackWindowController.Instance.audioPlayer.PlayShieldBlockDamageAudio();
            ((AdditionalEncryption)GetActiveEncryptionBonus()).BlockAttack();
        }
        else
        {
            HackWindowController.Instance.audioPlayer.PlayTakeDamageAudio();
            currentHP -= damage;
            if (currentHP <= 0) currentHP = 0;
            UpdateBars();
            if (currentHP == 0 && alive) { alive = false; HackWindowController.Instance.FailLock(); }
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        UpdateBars();
    }

    public void RecalculateAttack()
    {
        currentAttack = (int)GetUpgradableValue(1) - HackWindowController.Instance.GetGridController().GetPacifierDebuff();
        if (currentAttack <= 10) currentAttack = 10;
        UpdateBars();
    }

    public int GetCurrentAttack()
    {
        return currentAttack;
    }

    public void UpdateBars()
    {
        hpRatio = (float)currentHP / maxHP;
        attackRatio = (float) currentAttack / maxAttack;

        hpText.text = $"{currentHP}";
        attackText.text = $"{currentAttack}";

        lowHPAnimationActive = currentHP <= lowHPMaxValue;

        if (hoverAnimationActive)
            delayCoroutine = StartCoroutine(DelayHoverAnimation());
    }

    IEnumerator DelayHoverAnimation()
    {
        hoverAnimationActive = false;
        yield return new WaitForSeconds(hoverAnimationDelay);
        hoverAnimationActive = true;
        delayCoroutine = null;
    }

    private void Update()
    {
        if (HackWindowController.Instance == null) return;
        if (!HackWindowController.Instance.IsHacking()) return;
        if (hoverAnimationActive)
        {
            lowHPAnimationTimer = 0f;

            //update timer
            hoverAnimationTimer += Time.deltaTime;
            if (hoverAnimationTimer > hoverAnimationDuration) hoverAnimationTimer = 0f;

            //calculate mix value
            float mixValue = Mathf.Abs(hoverAnimationTimer / hoverAnimationDuration * 2 - 1);

            //calculate color
            Color hpTextColor = (hoverAnimationNotEnoughHP) ? Color.Lerp(NotEnoughHPColor1, NotEnoughHPColor2, mixValue) : ((lowHPAnimationActive) ? Color.Lerp(lowHPColor1, lowHPColor2, mixValue) : Color.white);

            //calculate hp bar
            float fillAmountMin = (float)hoverAnimationHP / maxHP;

            float currentFillAmount = fillAmountMin + mixValue * (hpRatio - fillAmountMin);
            float hpBarAlpha = hoverAnimationHPBarMinAlpha + mixValue * (1f - hoverAnimationHPBarMinAlpha);
            Color hpBarColor = hpBar.color;
            hpBarColor.a = hpBarAlpha;

            //apply values
            hpText.color = hpTextColor;
            hpBar.fillAmount = currentFillAmount;
            hpBar.color = hpBarColor;
        }
        else
        {
            hoverAnimationTimer = 0f;
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hpRatio, 0.2f);

            if (lowHPAnimationActive)
            {
                //update timer
                lowHPAnimationTimer += Time.deltaTime;
                if (lowHPAnimationTimer > lowHPAnimationDuration) 
                {
                    lowHPAnimationTimer = 0f;
                    HackWindowController.Instance.audioPlayer.PlayTakeDamageAudio();
                }

                //calculate mix value
                float mixValue = Mathf.Abs(lowHPAnimationTimer / lowHPAnimationDuration * 2 - 1);

                hpText.color = Color.Lerp(lowHPColor1, lowHPColor2, mixValue);
            }
            else
            {
                lowHPAnimationTimer = 0f;
                hpText.color = Color.white;
            }
            
            hpBar.color = Color.white;
        }
        attackBar.fillAmount = Mathf.Lerp(attackBar.fillAmount, attackRatio, 0.2f);
    }
}
