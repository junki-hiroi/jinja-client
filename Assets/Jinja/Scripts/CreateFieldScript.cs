using System.Collections.Generic;
using UnityEngine;

namespace Jinja.Scripts
{
public class CreateFieldScript
{
    public static void CreateField(FieldInfo fieldInfo)
    {
        var obake = Resources.Load<GameObject>("Obake");
        var wall = Resources.Load<GameObject>("Wall");
        var yuka = Resources.Load<GameObject>("YukaBox");
        var player = Resources.Load<GameObject>("Player");
        var redLock = Resources.Load<GameObject>("LockRed");
        var stepGameObject = Resources.Load<GameObject>("Step");

        var fieldRoot = new GameObject("FieldRoot");

        for (int y = 0; y < fieldInfo.Height; y++)
        {
            for (int x = 0; x < fieldInfo.Width; x++)
            {
                int i = y * fieldInfo.Width + x;

                if (fieldInfo.Floor[i] == FieldInfo.FloorNormal)
                {
                    var gameObject = GameObject.Instantiate(yuka);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, -scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Character[i] != 0)
                {
                    var gameObject = GameObject.Instantiate(obake);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == FieldInfo.GimickWall)
                {
                    var gameObject = GameObject.Instantiate(wall);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == FieldInfo.GimickPlayerStart)
                {
                    var gameObject = GameObject.Instantiate(player);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == FieldInfo.GimickStep)
                {
                    var gameObject = GameObject.Instantiate(stepGameObject);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == FieldInfo.GimickRedLock)
                {
                    var gameObject = GameObject.Instantiate(redLock);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }
            }
        }
    }
}
}
