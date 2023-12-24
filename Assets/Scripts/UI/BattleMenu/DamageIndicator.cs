using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum DamageIndicatorType
{
    HERO,
    ENEMY
}

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Image numImage;

    [SerializeField] private Sprite[] numSprites;

    private readonly Color enemyHurtColorBG = new(1, 0.8087637f, 0.2039216f);
    private readonly Color enemyHurtColorNum = new(0.8195055f, 1, 0);
    private readonly Color heroHurtColorBG = new(1, 0.2028302f, 0.2028302f);
    private readonly Color heroHurtColorNum = new(1, 0.8620365f, 0.2078431f);

    public void SetData(DamageIndicatorType type, int num)
    {
        if(type == DamageIndicatorType.HERO)
        {
            bgImage.color = heroHurtColorBG;
            numImage.color = heroHurtColorNum;
        }
        else
        {
            bgImage.color = enemyHurtColorBG;
            numImage.color = enemyHurtColorNum;
        }

        numImage.sprite = numSprites[num];
    }
    public void OnAnimationOver()
    {
        Destroy(gameObject);
    }
}
