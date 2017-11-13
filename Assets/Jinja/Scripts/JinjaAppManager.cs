using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Jinja.Scripts
{
public class JinjaAppManager : MonoBehaviour
{
    private GameObject cameraGameObject;
    private GameObject playerGameObject;
    private int frameCount = 0;

    private void Start ()
    {
        CreateFieldScript.FieldInfo fieldInfo = new CreateFieldScript.FieldInfo();

        fieldInfo.Masu = new List<int>();
        fieldInfo.Height = 10;
        fieldInfo.Width = 10;

        var playerIndex = Random.Range(0, fieldInfo.Height * fieldInfo.Width);

        for (int i = 0; i < fieldInfo.Height * fieldInfo.Width; i++)
        {
            var fieldProperty = Random.Range(0, 10);

            if (i == playerIndex)
            {
                fieldProperty = 99;
            }

            fieldInfo.Masu.Add(fieldProperty);
        }

        CreateFieldScript.CreateField(fieldInfo);

        cameraGameObject = GameObject.FindWithTag("MainCamera");
        playerGameObject = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (frameCount == 0)
        {
            var dx = (int)Input.GetAxisRaw("Horizontal");
            var dy = (int)Input.GetAxisRaw("Vertical");

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
            {
                playerGameObject.transform.Translate(dx, 0, dy);
                frameCount = 20;
            }
        }
        else
        {
            frameCount--;
        }

        cameraGameObject.transform.position = playerGameObject.transform.position + new Vector3(0, 10, -6);
        cameraGameObject.transform.LookAt(playerGameObject.transform);
    }
}
}
