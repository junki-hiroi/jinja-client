using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        return Gimick[index] == GimickWall ||
               Gimick[index] == GimickRedLock; // いったん、扉も通れない。
    }

    public bool IsStep(int index)
    {
        return Gimick[index] == GimickStep ||
               Gimick[index] == GimickPlayerStart; // いったん、プレイヤーのスタート地点も階段。
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

public class CharacterInfo
{
    public const int DirectionUp = 0;
    public const int DirectionRight = 1;
    public const int DirectionDown = 2;
    public const int DirectionLeft = 3;

    public string Id;
    public Vector2Int Position;
    public GameObject CharacterGameObject;
    public int Direction = 0;
}

public class EnemyInfo : CharacterInfo
{
    private static readonly Dictionary<int, Vector2Int> MoveDefine = new Dictionary<int, Vector2Int>()
    {
        {1, Vector2Int.down},
        {2, Vector2Int.right},
        {3, Vector2Int.up},
        {4, Vector2Int.left},
        {0, Vector2Int.zero},
    };

    private static readonly Dictionary<int, int> MoveDirectionDefine = new Dictionary<int, int>()
    {
        {1, DirectionUp},
        {2, DirectionRight},
        {3, DirectionDown},
        {4, DirectionLeft},
    };

    public Vector2Int StartPosition;
    public List<int> AutoMove = new List<int>()
    {
        0, 1, 2, 0, 3, 4,
    };

    private int _frameCount = 12;
    private int _counter;

    public EnemyInfo(CharacterInfo characterInfo)
    {
        Id = characterInfo.Id;
        Position = characterInfo.Position;
        CharacterGameObject = characterInfo.CharacterGameObject;
        StartPosition = characterInfo.Position;
        _frameCount = 12;
        _counter = 0;
    }

    public void EnemyInit()
    {
        Position = StartPosition;
        _frameCount = 12;
        _counter = 0;
    }

    public void EnemyUpdate()
    {
        if (_frameCount != 0)
        {
            _frameCount = _frameCount - 1;
            return;
        }

        _frameCount = 12;
        _counter = (_counter + 1) % AutoMove.Count;
        Position = Position + MoveDefine[AutoMove[_counter]];

        if (AutoMove[_counter] != 0)
        {
            Direction = MoveDirectionDefine[AutoMove[_counter]];
        }
    }

    public bool InLightingArea(Vector2Int targetPosition)
    {
#if UNITY_EDITOR

        if (UnityEditor.EditorPrefs.HasKey("_debugLighting"))
        {
            UnityEditor.EditorPrefs.DeleteKey("_debugLighting");
            return true;
        }

#endif

        if (3 < Vector2Int.Distance(Position, targetPosition))
        {
            return false;
        }

        if (Direction == DirectionUp)
        {
            if (targetPosition.x != Position.x)
            {
                return false;
            }

            if (InRange(Position.y, targetPosition.y, Position.y - 3))
            {
                return true;
            }
        }

        if (Direction == DirectionDown)
        {
            if (targetPosition.x != Position.x)
            {
                return false;
            }

            if (InRange(Position.y, targetPosition.y, Position.y + 3))
            {
                return true;
            }
        }

        if (Direction == DirectionRight)
        {
            if (targetPosition.y != Position.y)
            {
                return false;
            }

            if (InRange(Position.x, targetPosition.x, Position.x + 3))
            {
                return true;
            }
        }

        if (Direction == DirectionLeft)
        {
            if (targetPosition.y != Position.y)
            {
                return false;
            }

            if (InRange(Position.x, targetPosition.x, Position.x - 3))
            {
                return true;
            }
        }

        return false;
    }

    public static bool InRange(int a, int target, int b)
    {
        int min = Math.Min(a, b);
        int max = Math.Max(a, b);
        return min <= target && target <= max;
    }
}

public class JinjaAppManager : MonoBehaviour
{
    [Header("カメラ設定")]
    [SerializeField, HeaderAttribute("プレイヤーとの位置関係")]
    private Vector3 _cameraOffset = new Vector3(0, 10, -6);
    [SerializeField, HeaderAttribute("カメラの画角")]
    private float _cameraFieldOfView = 60.0f;

    private Camera _mainCamera;
    private GameObject _playerGameObject;
    private int _frameCount = 0;
    private int _playerHideTime = 0;
    const int PlayerMaxHideTime = 200;
    private CharacterInfo _playerInfo;
    private FieldInfo _fieldInfo;
    private Func<bool>[] _buttonPressing;
    private int _stageCatchObakeCount = 0;

    private Vector2Int _playerResponePosition;
    private Action<string> _obakeCountUpdate;
    private Action<string> _hideTimeUpdate;
    private List<CharacterInfo> _obakeInfos;
    // 一時的に捕まえたおばけの情報
    private List<CharacterInfo> _tmpCatchedObake;
    private List<EnemyInfo> _enemyInfos;

    private void Start ()
    {
        JinjaSoundManager.Instance.PlayBgm("bgm");

        _fieldInfo = ParseField.Load();
        var characterInfos = CreateFieldScript.CreateField(_fieldInfo);


        var playerInfo = characterInfos.Find(o => o.Id.Equals("player"));
        _playerInfo = playerInfo;
        _playerResponePosition = playerInfo.Position;
        characterInfos.Remove(playerInfo);

        var enemyInfos = characterInfos.Where(o => o.Id.Contains("enemy")).ToList();
        _enemyInfos = enemyInfos.Select(o => new EnemyInfo(o)).ToList();
        Debug.Log(_enemyInfos.Count);
        enemyInfos.ForEach(o => characterInfos.Remove(o));

        _obakeInfos = characterInfos;

        _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _playerGameObject = _playerInfo.CharacterGameObject;

        var canvas = GameObject.Find("Canvas");
        _buttonPressing = new Func<bool>[5];

        for (int i = 0; i < 4; i++)
        {
            var button = new GameObject("button" + i);
            button.transform.parent = canvas.transform;
            var rectTransform = button.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Vector2 centerPosition = new Vector2(0.7f, 0.5f) +
            new Vector2(
                Mathf.Sin(i * 0.5f * Mathf.PI) * 0.15f,
                Mathf.Cos(i * 0.5f * Mathf.PI) * 0.25f
            );

            rectTransform.anchorMin = centerPosition - Vector2.one * 0.1f;
            rectTransform.anchorMax = centerPosition + Vector2.one * 0.1f;
            rectTransform.localScale = Vector3.one;

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

        {
            var button = new GameObject("button" + "Hide");
            button.transform.parent = canvas.transform;
            var rectTransform = button.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Vector2 centerPosition = new Vector2(0.9f, 0.1f);

            rectTransform.anchorMin = centerPosition - Vector2.one * 0.1f;
            rectTransform.anchorMax = centerPosition + Vector2.one * 0.1f;
            rectTransform.localScale = Vector3.one;

            button.AddComponent<CanvasRenderer>();
            var image = button.AddComponent<Image>();
#if UNITY_EDITOR
            image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
#else
            image.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
#endif
            image.type = Image.Type.Sliced;

            var pressedButton = button.AddComponent<PressedButton>();
            _buttonPressing[4] = () => pressedButton.IsPressed;

        }

        {
            var obakeCounter = new GameObject("obake_counter");
            obakeCounter.transform.parent = canvas.transform;
            var rectTransform = obakeCounter.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Vector2 centerPosition = new Vector2(0.2f, 0.20f);
            rectTransform.anchorMin = centerPosition - Vector2.one * 0.2f;
            rectTransform.anchorMax = centerPosition + Vector2.one * 0.2f;
            rectTransform.localScale = Vector3.one;
            obakeCounter.AddComponent<CanvasRenderer>();
            var text = obakeCounter.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleRight;
            text.font = Resources.Load<Font>("ipaexg");
            text.color = Color.red;
            text.text = "Start";
            text.raycastTarget = false;
            _obakeCountUpdate = (s) => text.text = s;
        }

        {
            var hideTimeObject = new GameObject("hide_time");
            hideTimeObject.transform.parent = canvas.transform;
            var rectTransform = hideTimeObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Vector2 centerPosition = new Vector2(0.20f, 0.60f);
            rectTransform.anchorMin = centerPosition - Vector2.one * 0.2f;
            rectTransform.anchorMax = centerPosition + Vector2.one * 0.2f;
            rectTransform.localScale = Vector3.one;
            hideTimeObject.AddComponent<CanvasRenderer>();
            var text = hideTimeObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleRight;
            text.font = Resources.Load<Font>("ipaexg");
            text.color = Color.red;
            text.text = "Start";
            text.raycastTarget = false;
            _hideTimeUpdate = (s) => text.text = s;
        }


        _tmpCatchedObake = new List<CharacterInfo>();
    }

    /// <summary>
    ///   Limit a value to a certain range. When the value is smaller/bigger than the range, snap it to the range border.
    /// </summary>
    /// <typeparam name = "T">The type of the value to limit.</typeparam>
    /// <param name = "source">The source for this extension method.</param>
    /// <param name = "start">The start of the interval, included in the interval.</param>
    /// <param name = "end">The end of the interval, included in the interval.</param>
    private static T Clamp<T>(T source, T start, T end )
    where T : IComparable
    {
        bool isReversed = start.CompareTo( end ) > 0;
        T smallest = isReversed ? end : start;
        T biggest = isReversed ? start : end;

        return source.CompareTo( smallest ) < 0
               ? smallest
               : source.CompareTo( biggest ) > 0
               ? biggest
               : source;
    }

    private void Update()
    {
        bool isHide = Input.GetKey(KeyCode.Space);
        isHide = isHide ||  _buttonPressing[4]();

        if (isHide)
        {
            _playerHideTime = _playerHideTime + 1;
        }
        else
        {
            _playerHideTime = _playerHideTime - 1;
        }

        _playerHideTime = Clamp(_playerHideTime, 0, PlayerMaxHideTime);

        if (_playerHideTime == PlayerMaxHideTime)
        {
            isHide = false;
        }

        _hideTimeUpdate(string.Format("{0} / {1}", _playerHideTime, PlayerMaxHideTime));


        if (_frameCount == 0 && !isHide)
        {
            var dx = (int) Input.GetAxisRaw("Horizontal");
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

                    var collisionObake = _obakeInfos.Where(o => o.Position == next && o.CharacterGameObject.activeSelf)
                                         .ToList();

                    if (collisionObake.Count != 0)
                    {
                        _tmpCatchedObake.AddRange(collisionObake);

                        collisionObake.ForEach(o =>
                        {
                            o.CharacterGameObject.SetActive(false);
                        }
                                              );

                        JinjaSoundManager.Instance.PlayOneShot("encount");
                        _obakeCountUpdate(string.Format("捕えた {0}/ 送った {1}", _tmpCatchedObake.Count,
                                                        _stageCatchObakeCount));
                    }

                    if (_fieldInfo.IsStep(_fieldInfo.Vector2ToIndex(next)))
                    {
                        _playerResponePosition = _playerInfo.Position;
                        _stageCatchObakeCount += _tmpCatchedObake.Count;
                        _tmpCatchedObake.RemoveAll(o => true);
                        _obakeCountUpdate(string.Format("捕えた {0}/ 送った {1}", _tmpCatchedObake.Count,
                                                        _stageCatchObakeCount));

                        // TODO: マップクリア判定修正. 現在、3以上捕まえたとき.
                        if (2 < _stageCatchObakeCount)
                        {
                            JinjaSceneManager.Instanse.RequestLoadSceneAsync(JinjaSceneManager.MapSelect,
                                    LoadSceneMode.Single);
                        }
                    }

                    _frameCount = 7;
                }
            }
        }

        _frameCount = Math.Max(0, --_frameCount);

        _enemyInfos.ForEach(o =>
        {
            o.EnemyUpdate();
            o.CharacterGameObject.transform.position = new Vector3(
                o.Position.x,
                0.5f,
                -o.Position.y
            );
            o.CharacterGameObject.transform.localRotation = Quaternion.Euler(0, o.Direction * 90, 0);
        });

        var isInLightingArea = _enemyInfos.Any(o => o.InLightingArea(_playerInfo.Position));

        if (isHide)
        {
            _playerGameObject.transform.position = new Vector3(
                _playerInfo.Position.x,
                -10,
                -_playerInfo.Position.y);
        }
        else
        {
            if (isInLightingArea)
            {
                _playerInfo.Position = _playerResponePosition;
                _tmpCatchedObake.ForEach(o =>
                                         o.CharacterGameObject.SetActive(true)
                                        );
                _tmpCatchedObake.RemoveAll(o => true);
                _obakeCountUpdate(string.Format("捕えた {0}/ 送った {1}", _tmpCatchedObake.Count, _stageCatchObakeCount));
                _enemyInfos.ForEach(o => o.EnemyInit());
            }

            _playerGameObject.transform.position = new Vector3(
                _playerInfo.Position.x,
                0,
                -_playerInfo.Position.y);

            _mainCamera.transform.position = _playerGameObject.transform.position + _cameraOffset;
            _mainCamera.transform.LookAt(_playerGameObject.transform);
            _mainCamera.fieldOfView = _cameraFieldOfView;
        }
    }
}
}
