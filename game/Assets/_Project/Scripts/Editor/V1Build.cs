// CrewJournal — build Windows via CLI.
// Uso: Unity -batchmode -nographics -quit -projectPath ./game -executeMethod CrewJournal.Editor.V1Build.BuildWindows -logFile -
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CrewJournal.Editor
{
#if UNITY_EDITOR
    public static class V1Build
    {
        public static void BuildWindows()
        {
            string[] scenes = new string[] { "Assets/_Project/Scenes/World.unity" };
            string outPath = System.IO.Path.GetFullPath(
                Application.dataPath + "/../../Build/crew-journal.exe");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outPath));
            BuildPlayerOptions opt = new BuildPlayerOptions();
            opt.scenes = scenes;
            opt.locationPathName = outPath;
            opt.target = BuildTarget.StandaloneWindows64;
            opt.options = BuildOptions.None;
            UnityEditor.BuildPipeline.BuildPlayer(opt);
            Debug.Log("[V1Build] Build em " + outPath);
        }
    }
#endif
}
