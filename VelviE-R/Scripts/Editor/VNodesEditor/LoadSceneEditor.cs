using UnityEditor;
using VelvieR;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System;


namespace VIEditor
{
    [CustomEditor(typeof(LoadScene))]
    public class LoadSceneEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as LoadScene;
            root.Add(DrawSceneMode(t));
            root.Add(DrawDropField(t));

            //Always add this at the end!
            VUITemplate.DrawSummary(root, t, () => t.OnVSummary());
            return root;
        }
        public VisualElement DrawDropField(LoadScene t)
        {
            var rootBox = VUITemplate.GetTemplate("Scene : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.load;

            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] scenes = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            }

            objField.choices = scenes.ToList();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    t.load = x.newValue;
                });
            }

            return rootBox;
        }
        public VisualElement DrawSceneMode(LoadScene t)
        {
            var rootBox = VUITemplate.GetTemplate("Mode : ");
            var field = VUITemplate.GetField(rootBox);
            var objField = new DropdownField();
            objField.style.width = field.style.width;
            field.Add(objField);

            objField.value = t.sceneMode.ToString();
            objField.choices = Enum.GetNames(typeof(LoadSceneMode)).ToList();

            if (!PortsUtils.PlayMode)
            {
                objField.RegisterValueChangedCallback((x) =>
                {
                    foreach (var tt in Enum.GetValues(typeof(LoadSceneMode)))
                    {
                        var astype = (LoadSceneMode)tt;

                        if (tt.ToString() == x.newValue)
                        {
                            t.sceneMode = astype;
                        }
                    }
                });
            }

            return rootBox;
        }
    }
}