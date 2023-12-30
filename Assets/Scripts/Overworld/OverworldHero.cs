using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OverworldHero : MonoBehaviour
{

    private HeroType _type;
    private GameObject _model;
    /**
     *  Sets the model. Replaces the old model if it's of a new HeroType
     */
    public void SetCharacterModel(GameObject model, HeroType type)
    {
        if(_model != null)
        {
            if(type == _type) { return; }
            Destroy(_model);
        }
        _model = Instantiate(model);
        _model.transform.SetParent(transform, false);
        _model.transform.localPosition = Vector3.zero;
        _model.GetComponent<CharacterModel>().SetAsOverworldModel();
        _type = type;
    }
}
