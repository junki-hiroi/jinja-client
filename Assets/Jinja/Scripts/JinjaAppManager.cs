using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2 IndexToVector2(int index)
    {
        int x = index % Width;
        int y = index / Width;
        return new Vector2(x, y);
    }
}

public class PlayerInfo
{
    public Vector2 Position = Vector2.zero;
}

public class JinjaAppManager : MonoBehaviour
{
    private GameObject _cameraGameObject;
    private GameObject _playerGameObject;
    private int _frameCount = 0;
    private PlayerInfo _playerInfo;
    private FieldInfo _fieldInfo;

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
    }

    private void Update()
    {
        bool isHide = Input.GetKey(KeyCode.Space);

        if (_frameCount == 0 && !isHide)
        {
            var dx = (int)Input.GetAxisRaw("Horizontal");
            var dy = -(int)Input.GetAxisRaw("Vertical");

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
            {
                _playerInfo.Position = _playerInfo.Position + new Vector2(dx, dy);
                _frameCount = 7;
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
