using System;
using UnityEngine;

namespace Jinja.Scripts
{
public class JinjaAppManager : MonoBehaviour
{
    private GameObject cameraGameObject;
    private GameObject playerGameObject;
    private int frameCount = 0;

    private void Start ()
    {
        CreateFieldScript.FieldInfo fieldInfo = ParseField.Load();
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
