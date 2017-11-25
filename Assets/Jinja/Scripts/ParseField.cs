using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Jinja.Scripts
{
public static class ParseField
{
    public static FieldInfo Load()
    {
        FieldInfo fieldInfo = new FieldInfo();
#if UNITY_EDITOR
        var jsonText = File.ReadAllText("OutOfUnity/MapData/ghost.json");
        var jsonData = MiniJSON.Json.Deserialize(jsonText);

        try
        {
            var height = (long)jsonData.TryGet("height");
            var width = (long)jsonData.TryGet("width");
            var layers = (List<object>)jsonData.TryGet("layers");

            fieldInfo.Height = (int)height;
            fieldInfo.Width = (int)width;

            foreach (var layer in layers)
            {
                var data = (List<object>)layer.TryGet("data");
                var name = (string)layer.TryGet("name");

                if (name.Equals("floor"))
                {
                    fieldInfo.Floor = data
                                      .Select(o => int.Parse(o.ToString()))
                                      .Select(o => o == 12 ? FieldInfo.FloorNormal : 0)
                                      .ToList();
                }
                else if (name.Equals("object"))
                {
                    fieldInfo.Gimick = data
                                       .Select(o => int.Parse(o.ToString()))
                                       .Select(o =>
                    {
                        if (o == 3)
                        {
                            return FieldInfo.GimickWall;
                        }

                        if (o == 58)
                        {
                            return FieldInfo.GimickStep;
                        }

                        if (o == 57)
                        {
                            return FieldInfo.GimickPlayerStart;
                        }

                        if (o == 49)
                        {
                            return FieldInfo.GimickRedLock;
                        }

                        return 0;
                    })
                    .ToList();
                }
                else
                {
                    fieldInfo.Character = data.Select(o => int.Parse(o.ToString())).ToList();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

#endif
        return fieldInfo;
    }

    private static object TryGet(this object dictionary, string key)
    {
        var dict = dictionary as Dictionary<string, object>;

        if (dict != null)
        {
            return dict[key];
        }

        return null;
    }

}
}
