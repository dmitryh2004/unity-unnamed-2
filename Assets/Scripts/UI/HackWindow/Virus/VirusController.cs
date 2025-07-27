using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirusController : UpgradableItem
{
    public static VirusController Instance;
    int currentHP;
    int currentAttack;
    bool alive = true;

    float hpRatio, attackRatio;
    const int maxHP = 150, maxAttack = 45;

    [SerializeField] Image hpBar, attackBar;
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
        currentHP = (int)GetUpgradableValue1();
        currentAttack = (int)GetUpgradableValue2();
        alive = true;
        UpdateBars();
    }

    public void TakeDamage(int damage, bool ignoreEncryption = false)
    {
        bool useAEBonus = false;
        Bonus additionalEncryptionBonus = null;
        for (int i = 1; i < 4; i++)
        {
            additionalEncryptionBonus = BonusController.Instance.GetBonus(i);
            if (additionalEncryptionBonus != null && !ignoreEncryption)
            {
                if (additionalEncryptionBonus is AdditionalEncryption ae)
                {
                    if (ae.IsActive())
                    {
                        useAEBonus = true;
                        break;
                    }
                }
            }
        }
        

        if (useAEBonus)
        {
            ((AdditionalEncryption)additionalEncryptionBonus).BlockAttack();
        }
        else
        {
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
        currentAttack = (int)GetUpgradableValue2() - HackWindowController.Instance.GetGridController().GetPacifierDebuff();
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
    }

    private void Update()
    {
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hpRatio, 0.2f);
        attackBar.fillAmount = Mathf.Lerp(attackBar.fillAmount, attackRatio, 0.2f);
    }

    
}
