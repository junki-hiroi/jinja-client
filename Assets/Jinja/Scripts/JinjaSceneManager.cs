using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jinja.Scripts
{
public class JinjaSceneManager
{
    public const string Main = "Jinja/_start";
    public const string MapSelect = "Jinja/_map_select";


    private static JinjaSceneManager _instanse = null;
    private AsyncOperation _asyncOperation = null;

    public AsyncOperation LatestAsyncOperation
    {
        get { return _asyncOperation; }
    }

    public static JinjaSceneManager Instanse
    {
        get
        {
            if (_instanse == null)
            {
                _instanse = new JinjaSceneManager();
            }

            return _instanse;
        }
    }

    public bool RequestLoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (_asyncOperation != null)
        {
            if (!_asyncOperation.isDone)
            {
                return false;
            }
        }

        _asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        return true;
    }


}
}
