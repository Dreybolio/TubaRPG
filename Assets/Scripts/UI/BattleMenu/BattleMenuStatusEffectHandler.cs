using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuStatusEffectHandler : MonoBehaviour
{
    [SerializeField] private GameObject statusEffectPrefab;

    private List<GameObject> list;

    private readonly int DISTANCE_BETWEEN_EACH_ELEMENT = 80;
    private void Start()
    {
        list = new();
    }
    public void SetStatusEffects(Dictionary<StatusEffect, int> effects)
    {
        ClearAll();
        if(effects != null)
        {
            int effectsFound = 0;
            foreach (KeyValuePair<StatusEffect, int> effect in effects)
            {
                GameObject instance = Instantiate(statusEffectPrefab, transform);
                instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, DISTANCE_BETWEEN_EACH_ELEMENT * effectsFound);
                BattleMenuStatusEffect script = instance.GetComponent<BattleMenuStatusEffect>();
                script.SetIcon(effect.Key); script.SetNumber(effect.Value);
                list.Add(instance);

                effectsFound++;
            }
        }
    }

    public void ClearAll()
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }
        list.Clear();
        list.TrimExcess();
    }
}
