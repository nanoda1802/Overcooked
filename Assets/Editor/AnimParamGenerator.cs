using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// 새로운 시도...!

// [생성방법]
// 1. 원하는 AnimatorController 파일 선택하고 오른쪽 클릭
// 2. Generate -> AnimParam File 선택
// 3. 선택한 AnimatorController와 같은 디렉토리에 cs파일 생성됨

public class AnimParamGenerator
{
   [MenuItem("Assets/Generate/AnimParam File")]
   public static void Generate()
   {
      AnimatorController animCtrl = Selection.activeObject as AnimatorController;
      if (animCtrl is null)
      {
         Debug.LogError("선택된 Animator Controller가 없읍니다.");
         return;
      }

      string className = animCtrl.name.Replace(" ", "_") + "Param";
      string filePath = AssetDatabase.GetAssetPath(animCtrl);
      string directory = Path.GetDirectoryName(filePath);
      string fullPath = Path.Combine(directory, className + ".cs");
      
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("using UnityEngine;");
      sb.AppendLine("");
      sb.AppendLine($"public class {className}\n");
      sb.AppendLine("{");

      foreach (var param in animCtrl.parameters)
      {
         sb.Append($"public readonly int {param.name.Replace(" ", "_")} = Animator.StringToHash(\"{param.name}\");\n");
      }

      sb.AppendLine("}");

      File.WriteAllText(fullPath, sb.ToString());
      AssetDatabase.Refresh();
      
      Debug.Log($"{animCtrl.name}의 Parameter Hash 모음 cs파일이 생성되었읍니다! \n경로 : {fullPath}");
   }
}
