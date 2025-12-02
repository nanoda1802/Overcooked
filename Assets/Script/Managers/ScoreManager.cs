using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _curScore = 0;
    private int baseScore = 100; // 임시
    
    public void ApplyScore(float ratio)
    {
        _curScore += CalculateScore(ratio);
        Debug.Log($"획득 점수는? {CalculateScore(ratio)}");
        Debug.Log($"현재 점수는 {_curScore}!");
    }

    private int CalculateScore(float ratio)
    {
        if (ratio < 0) return -baseScore;
        return (int) (baseScore * (1 + ratio));
    }
}
