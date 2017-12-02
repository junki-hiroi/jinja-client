using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Jinja.Scripts
{
public class MapSelect : MonoBehaviour
{
    private Func<bool>[] _buttonPressing;

    void Start ()
    {
        var canvas = GameObject.Find("Canvas");
        canvas.GetComponent<RectTransform>().localScale = Vector3.one;
        _buttonPressing = new Func<bool>[2];

        for (int i = 0; i < 2; i++)
        {
            var button = new GameObject("button" + i);
            button.transform.parent = canvas.transform;
            var rectTransform = button.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Vector2 centerPosition = new Vector2(0.5f, 0.8f) +
            new Vector2(
                0.0f,
                - i * 0.25f
            );

            rectTransform.anchorMin = centerPosition - Vector2.one * 0.1f;
            rectTransform.anchorMax = centerPosition + Vector2.one * 0.1f;

            button.AddComponent<CanvasRenderer>();
            var image = button.AddComponent<Image>();
#if UNITY_EDITOR
            image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
#else
            image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
#endif
            image.type = Image.Type.Sliced;

            var pressedButton = button.AddComponent<PressedButton>();
            _buttonPressing[i] = () => pressedButton.IsPressed;
        }
    }

    void Update ()
    {
        int findIndex = Array.FindIndex(_buttonPressing, func => func());

        if (findIndex != -1)
        {
            if (findIndex == 0)
            {
                ParseField.LoadPath = "OutOfUnity/MapData/stage1.json";
            }
            else
            {
                ParseField.LoadPath = "OutOfUnity/MapData/stage2.json";
            }

            JinjaSceneManager.Instanse.RequestLoadSceneAsync(JinjaSceneManager.Main, LoadSceneMode.Single);
        }
    }
}
}
