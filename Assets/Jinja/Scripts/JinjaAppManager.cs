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
}

public class JinjaAppManager : MonoBehaviour
{
    private GameObject cameraGameObject;
    private GameObject playerGameObject;
    private int frameCount = 0;



    private void Start ()
    {
        FieldInfo fieldInfo = ParseField.Load();
        CreateFieldScript.CreateField(fieldInfo);

        cameraGameObject = GameObject.FindWithTag("MainCamera");
        playerGameObject = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        bool isHide = Input.GetKey(KeyCode.Space);

        if (frameCount == 0 && !isHide)
        {
            var dx = (int)Input.GetAxisRaw("Horizontal");
            var dy = (int)Input.GetAxisRaw("Vertical");

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
            {
                playerGameObject.transform.Translate(dx, 0, dy);
                frameCount = 7;
            }
        }
        else
        {
            frameCount = Math.Max(0, --frameCount);
        }

        if (isHide)
        {
            playerGameObject.transform.position = new Vector3(
                playerGameObject.transform.position.x,
                -10,
                playerGameObject.transform.position.z);
        }
        else
        {
            playerGameObject.transform.position = new Vector3(
                playerGameObject.transform.position.x,
                0,
                playerGameObject.transform.position.z);
            cameraGameObject.transform.position = playerGameObject.transform.position + new Vector3(0, 10, -6);
            cameraGameObject.transform.LookAt(playerGameObject.transform);
        }
    }
}
}
