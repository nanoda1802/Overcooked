using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class ScoreManager : MonoBehaviour
{
    private int _curScore = 0;
    private int _comboCount = 0;
    private const float COMBO_MODIFIER = 0.2f;
    
    [SF] private Text scoreTxt;
    [SF] private Text comboTxt;
    [SF] private Image fireImg;
    
    private Animator _scoreTxtAnim;
    private readonly int _addParamHash = Animator.StringToHash("Add");
    private readonly int _deductParamHash = Animator.StringToHash("Deduct");
    
    [SF] private Color[] comboTxtColors;

    private void Awake()
    {
        _scoreTxtAnim = scoreTxt.GetComponent<Animator>();
    }

    private void Start()
    {
        ResetComboCount();
    }

    public void ApplyScore(int baseScore, float ratio)
    {
        int point = CalculatePoint(baseScore, ratio);
        _scoreTxtAnim.SetTrigger(point <= 0 ? _deductParamHash : _addParamHash);
        _curScore += point;
        if (_curScore < 0) _curScore = 0;
        scoreTxt.text = $"{_curScore}";
    }

    private int CalculatePoint(int baseScore, float ratio)
    {
        if (ratio < 0)
        {
            ResetComboCount();
            return (int) (baseScore * -0.5f);
        }
        
        AddComboCount();
        return (int) (baseScore * (1 + ratio + (_comboCount * COMBO_MODIFIER)));
    }

    private void AddComboCount()
    {
        _comboCount++;
        fireImg.gameObject.SetActive(true);
        UpdateComboText();
    }

    private void ResetComboCount()
    {
        _comboCount = 0;
        fireImg.gameObject.SetActive(false);
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        comboTxt.text = $"{_comboCount} Combo";
        int colorIdx = Mathf.Clamp(_comboCount,0,comboTxtColors.Length - 1);
        comboTxt.color = comboTxtColors[colorIdx];
    }
}
