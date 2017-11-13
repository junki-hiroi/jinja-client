using System.Collections.Generic;
using UnityEngine;

namespace Jinja.Scripts
{
public class CreateFieldScript
{
    public struct FieldInfo
    {
        public List<int> Floor;
        public List<int> Gimick;
        public List<int> Character;
        public int Height;
        public int Width;
    }

    public static void CreateField(FieldInfo fieldInfo)
    {
        var obake = Resources.Load<GameObject>("Obake");
        var wall = Resources.Load<GameObject>("Wall");
        var yuka = Resources.Load<GameObject>("YukaBox");
        var player = Resources.Load<GameObject>("Player");

        var fieldRoot = new GameObject("FieldRoot");

        for (int y = 0; y < fieldInfo.Height; y++)
        {
            for (int x = 0; x < fieldInfo.Width; x++)
            {
                int i = y * fieldInfo.Width + x;

                if (fieldInfo.Floor[i] == 12)
                {
                    var gameObject = GameObject.Instantiate(yuka);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, -scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == 3)
                {
                    var gameObject = GameObject.Instantiate(wall);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Character[i] == 61)
                {
                    var gameObject = GameObject.Instantiate(obake);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }

                if (fieldInfo.Gimick[i] == 57)
                {
                    var gameObject = GameObject.Instantiate(player);
                    var scaleY = gameObject.transform.localScale.y;
                    gameObject.transform.localPosition = new Vector3(x, scaleY / 2, -y);
                    gameObject.transform.parent = fieldRoot.transform;
                }
            }
        }
    }
}
}
