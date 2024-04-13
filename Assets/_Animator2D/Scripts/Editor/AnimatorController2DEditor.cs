using Etienne.Animator2D;
using System.IO;
using UnityEditor;

namespace EtienneEditor.Animator2D
{
    [CustomEditor(typeof(AnimatorController2D))]
    public class AnimatorController2DEditor : Editor<AnimatorController2D>
    {
        [MenuItem("Assets/Create/Etienne/2D/Animator/Animator Controller")]
        static void CreateAnimation2DEtienne() => CreateAnimation2D();

        [MenuItem("Assets/Create/2D/Animator/Animator Controller")]
        static void CreateAnimation2D()
        {
            AnimatorController2D animatorController = CreateInstance<AnimatorController2D>();
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            path = (ProjectWindowUtil.IsFolder(Selection.activeInstanceID) ? path : Path.GetDirectoryName(path));
            ProjectWindowUtil.CreateAsset(animatorController, $"{path}/New AnimatorController2D.asset");
        }
    }
}