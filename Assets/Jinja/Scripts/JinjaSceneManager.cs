using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jinja.Scripts
{
public class JinjaSceneManager
{
    public const int Main = 0;
    public const int MapSelect = 1;


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

    public bool RequestLoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode)
    {
        if (_asyncOperation != null)
        {
            if (!_asyncOperation.isDone)
            {
                return false;
            }
        }

        _asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, loadSceneMode);
        return true;
    }
}
}
