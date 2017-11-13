﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Jinja.Scripts
{
public static class ParseField
{
    public static CreateFieldScript.FieldInfo Load()
    {
        CreateFieldScript.FieldInfo fieldInfo = new CreateFieldScript.FieldInfo();
#if UNITY_EDITOR
        var jsonText = File.ReadAllText("OutOfUnity/MapData/ghost.json");
        var jsonData = MiniJSON.Json.Deserialize(jsonText);
        Debug.Log(jsonData);

        try
        {
            var height = (long)jsonData.TryGet("height");
            var width = (long)jsonData.TryGet("width");
            var layers = (List<object>)jsonData.TryGet("layers");

            fieldInfo.Height = (int)height;
            fieldInfo.Width = (int)width;

            foreach (var layer in layers)
            {
                Debug.Log(layer);
                var data = (List<object>)layer.TryGet("data");
                Debug.Log(data);
                var name = (string)layer.TryGet("name");
                Debug.Log(name);

                if (name.Equals("floor"))
                {
                    fieldInfo.Floor = data.Select(o => int.Parse(o.ToString())).ToList();
                }
                else if (name.Equals("object"))
                {
                    fieldInfo.Gimick = data.Select(o => int.Parse(o.ToString())).ToList();
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