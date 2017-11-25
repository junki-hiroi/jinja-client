using UnityEditor;

namespace Jinja.Scripts.Editor
{
public static class JinjaDebugMenu
{
    [MenuItem("Jinja/Field SetLighting")]
    static void Field_SetLighting()
    {
        EditorPrefs.SetBool("_debugLighting", true);
    }
}
}
