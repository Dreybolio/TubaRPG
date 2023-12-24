using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DescriptionBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private InputManager inputManager;
    private void Start()
    {
        inputManager = InputManager.Instance;
        SetSpriteSheet();
    }

    public void SetText(string text)
    {
        if (text != null)
        {
            string parsedText = ParseTextForTags(text);
            textMesh.text = parsedText;
        }
    }
    private string ParseTextForTags(string text)
    {
        string newTxt = text;
        newTxt = newTxt.Replace("<A>", "<sprite=0>");
        newTxt = newTxt.Replace("<B>", "<sprite=1>");
        newTxt = newTxt.Replace("<X>", "<sprite=2>");
        newTxt = newTxt.Replace("<Y>", "<sprite=3>");
        newTxt = newTxt.Replace("<UP>", "<sprite=4>");
        newTxt = newTxt.Replace("<DOWN>", "<sprite=5>");
        newTxt = newTxt.Replace("<LEFT>", "<sprite=6>");
        newTxt = newTxt.Replace("<RIGHT>", "<sprite=7>");
        return newTxt;
    }

    private void SetSpriteSheet()
    {
        textMesh.spriteAsset = inputManager.GetTextSpriteSheet();
    }
}
