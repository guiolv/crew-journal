// CrewJournal — bootstrap da cena World via CLI.
// Uso: Unity -batchmode -nographics -quit -projectPath ./game -executeMethod CrewJournal.Editor.V1Bootstrap.Build -logFile -
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace CrewJournal.Editor
{
#if UNITY_EDITOR
    public static class V1Bootstrap
    {
        public static void Build()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cam = new GameObject("Main Camera", typeof(Camera));
            Camera c = cam.GetComponent<Camera>();
            c.orthographic = true;
            c.orthographicSize = 5f;
            c.clearFlags = CameraClearFlags.SolidColor;
            c.backgroundColor = new Color(0.03f, 0.12f, 0.2f);
            cam.transform.position = new Vector3(0, 0, -10);

            GameObject game = new GameObject("Game", typeof(GameManager), typeof(GameUI));

            // EventSystem via Type para nao acoplar o assembly de UI no bootstrap
            Type esType = Type.GetType("UnityEngine.EventSystems.EventSystem, UnityEngine.UI");
            Type siType = Type.GetType("UnityEngine.EventSystems.StandaloneInputModule, UnityEngine.UI");
            if (esType != null && siType != null)
            {
                GameObject es = new GameObject("EventSystem", esType, siType);
            }

            string dir = Application.dataPath + "/_Project/Scenes";
            System.IO.Directory.CreateDirectory(dir);
            EditorSceneManager.SaveScene(
                EditorSceneManager.GetActiveScene(),
                dir + "/World.unity");
            Debug.Log("[V1Bootstrap] Cena World criada.");
        }
    }
#endif
}
