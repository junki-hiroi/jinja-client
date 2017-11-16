using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jinja.Scripts
{
public class FieldInfo
{
    public const int FloorNormal = 1;

    public const int GimickWall = 1;
    public const int GimickPlayerStart = 2;
    public const int GimickStep = 3;
    public const int GimickRedLock = 4;

    public List<int> Floor;
    public List<int> Gimick;
    public List<int> Character;
    public int Height;
    public int Width;

    public int GetPlayerStartIndex()
    {
        return Gimick.FindIndex(o => o == GimickPlayerStart);
    }

    public bool IsWall(int index)
    {
        return Gimick[index] == GimickWall;
    }


    public Vector2Int IndexToVector2(int index)
    {
        int x = index % Width;
        int y = index / Width;
        return new Vector2Int(x, y);
    }

    public int Vector2ToIndex(Vector2Int vector2)
    {
        return vector2.y * Width + (int)vector2.x;
    }
}

public class PlayerInfo
{
    public Vector2Int Position;
}

public class JinjaAppManager : MonoBehaviour
{
    private GameObject _cameraGameObject;
    private GameObject _playerGameObject;
    private int _frameCount = 0;
    private PlayerInfo _playerInfo;
    private FieldInfo _fieldInfo;
    private Func<bool>[] _buttonPressing;

    private void Start ()
    {
        _fieldInfo = ParseField.Load();
        CreateFieldScript.CreateField(_fieldInfo);

        _playerInfo = new PlayerInfo
        {
            Position = _fieldInfo.IndexToVector2(_fieldInfo.GetPlayerStartIndex())
        };
        _cameraGameObject = GameObject.FindWithTag("MainCamera");
        _playerGameObject = GameObject.FindWithTag("Player");

        var canvas = GameObject.Find("Canvas");
        _buttonPressing = new Func<bool>[4];

        for (int i = 0; i < 4; i++)
        {
            var button = new GameObject("button" + i);
            button.transform.parent = canvas.transform;
            var rectTransform = button.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.one * 20;

            Vector2 centerPosition = new Vector2(0.70f, 0.5f) +
            new Vector2(
                Mathf.Sin(i * 0.5f * Mathf.PI) * 0.15f,
                Mathf.Cos(i * 0.5f * Mathf.PI) * 0.15f
            );

            rectTransform.anchorMin = centerPosition;
            rectTransform.anchorMax = centerPosition;

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

    private void Update()
    {
        bool isHide = Input.GetKey(KeyCode.Space);

        if (_frameCount == 0 && !isHide)
        {
            var dx = (int)Input.GetAxisRaw("Horizontal");
            var dy = (int) - Input.GetAxisRaw("Vertical");

            dy = _buttonPressing[0]() ? -1 : dy;
            dy = _buttonPressing[2]() ? 1 : dy;
            dx = _buttonPressing[1]() ? 1 : dx;
            dx = _buttonPressing[3]() ? -1 : dx;

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
            {
                var next = _playerInfo.Position + new Vector2Int(dx, dy);

                if (!_fieldInfo.IsWall(_fieldInfo.Vector2ToIndex(next)))
                {
                    _playerInfo.Position = next;
                    _frameCount = 7;
                }
            }
        }
        else
        {
            _frameCount = Math.Max(0, --_frameCount);
        }



        if (isHide)
        {
            _playerGameObject.transform.position = new Vector3(
                _playerInfo.Position.x,
                -10,
                -_playerInfo.Position.y);
        }
        else
        {
            _playerGameObject.transform.position = new Vector3(
                _playerInfo.Position.x,
                0,
                -_playerInfo.Position.y);
            _cameraGameObject.transform.position = _playerGameObject.transform.position + new Vector3(0, 10, -6);
            _cameraGameObject.transform.LookAt(_playerGameObject.transform);
        }
    }
}
}
