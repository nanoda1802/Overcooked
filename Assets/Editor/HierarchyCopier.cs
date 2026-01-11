using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/* Mermaid Node Shapes */
// ((name)) : 원형, Root 노드에 사용
// (name) : 모서리가 둥근 사각형
// [name] : 직사각형
// ))name)) : 깃발 형태
// {{name}} : 육각형

public class HierarchyCopier : Editor
{
    private const string CircleShape = "{0}(({1})):::root";
    private const string RoundedShape = "{0}({1}):::txtAndImg";
    private const string SquareShape = "{0}[{1}]";
    private const string HexShape = "{0}{{{{{1}}}}}:::btn";
    private const string ParallelShape = "{0}[/{1}/]:::slider"; // 평행사변...
    private const string SubRoutineShape = "{0}[[{1}]]:::group";
    private const string DecisionShape = "{0}{{{1}}}:::toggle"; // 마름모
    
    private static string GetNodeName(string objName, int parentInstanceId = 0)
    {
        string safeName = Regex.Replace(objName, @"[^a-zA-Z0-9]", "_");
        return $"{safeName}_{parentInstanceId}";
    }

    private static string GetShapeFormat(string name)
    {
        string type = name.Split('_')[0];
        
        return type switch
        {
            "UI" or "Group" => SubRoutineShape,
            "Txt" or "Img" => RoundedShape,
            "Btn" => HexShape,
            "Toggle" => DecisionShape,
            "Slider" => ParallelShape,
            _ => SquareShape
        };
    }

    [MenuItem("Tools/Copy Hierarchy As Text For Mermaid/Graph TD", false, 0)]
    private static void Copy_Graph()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("Hierarchy에 선택된 오브젝트가 없슴다. [HierarchyCopier]");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("graph TD");
        // Root (부드러운 분홍)
        sb.AppendLine("    classDef root fill:#FCE4EC,stroke:#880E4F,stroke-width:3px;");
        // UI/Group (연한 파랑)
        sb.AppendLine("    classDef group fill:#E1F5FE,stroke:#01579B,stroke-width:2px;");
        // Txt/Img (연한 녹색)
        sb.AppendLine("    classDef txtAndImg fill:#F1F8E9,stroke:#33691E,stroke-width:2px;");
        // Btn (연한 노랑)
        sb.AppendLine("    classDef btn fill:#FFF9C4,stroke:#FBC02D,stroke-width:2px;");
        // Toggle (연한 보라)
        sb.AppendLine("    classDef toggle fill:#F3E5F5,stroke:#7B1FA2,stroke-width:2px;");
        // Slider (연한 살구)
        sb.AppendLine("    classDef slider fill:#FFF3E0,stroke:#E65100,stroke-width:2px;");
        
        string rootNodeName = GetNodeName(selected.name);
        sb.AppendLine($"    {string.Format(CircleShape, rootNodeName, selected.name)}");

        // 2. 루트의 직계 자식들만 subgraph로 분리하여 가로 폭 제어
        foreach (Transform child in selected.transform)
        {
            string childNodeName = GetNodeName(child.name, Mathf.Abs(selected.GetInstanceID()));
            string shapeFormat = GetShapeFormat(child.name);
        
            // 서브그래프 시작 (박스 타이틀은 오브젝트 이름으로)
            sb.AppendLine($"    subgraph SG_{childNodeName} [{child.name}]");
        
            // 루트와 연결
            sb.AppendLine($"        {rootNodeName} --> {string.Format(shapeFormat, childNodeName, child.name)}");

            // 자식의 자식들 탐색 (들여쓰기 적용)
            if (shapeFormat is SubRoutineShape or RoundedShape or HexShape)
            {
                AppendChildren_Graph(child, childNodeName, sb);
            }

            sb.AppendLine("    end"); // 서브그래프 종료
        }
        
        
        // AppendChildren_Graph(selected.transform, rootNodeName, sb);

        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log($"<b>{selected.name}</b>의 계층 구조가 클립보드에 복사되었슴다. [HierarchyCopier]");
    }

    private static void AppendChildren_Graph(Transform parent, string parentNodeName, StringBuilder sb)
    {
        int parentInstanceId = Mathf.Abs(parent.GetInstanceID());
        
        foreach (Transform child in parent)
        {
            string childNodeName = GetNodeName(child.name, parentInstanceId);
            string shapeFormat = GetShapeFormat(child.name);
            
            sb.AppendLine($"        {parentNodeName} --> {string.Format(shapeFormat, childNodeName, child.name)}");

            if (shapeFormat is SubRoutineShape or RoundedShape or HexShape)
            {
                AppendChildren_Graph(child, childNodeName, sb);
            }
        }
    }
    
    [MenuItem("Tools/Copy Hierarchy As Text For Mermaid/MindMap", false, 0)]
    private static void Copy_MindMap()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("There is no selected object in hierarchy. [HierarchyCopier]");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("mindmap");
        sb.AppendLine($"  root(({selected.name}))");

        AppendChildren_MindMap(selected.transform, sb, 2);

        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log($"<b>{selected.name}</b> 구조가 클립보드에 복사되었습니다! (by HierarchyMermaidExporter)");
    }

    private static void AppendChildren_MindMap(Transform parent, StringBuilder sb, int depth)
    {
        foreach (Transform child in parent)
        {
            // depth에 따른 2칸 들여쓰기
            string indent = new string(' ', depth * 2);
            
            // UI 요소 이름 정제 (공백이 있으면 Mermaid에서 에러가 날 수 있으니 _로 치환)
            string cleanName = child.name.Replace(" ", "_");
            
            sb.AppendLine($"{indent}{cleanName}");
            
            // 재귀적으로 자식 탐색
            AppendChildren_MindMap(child, sb, depth + 1);
        }
    }
}
