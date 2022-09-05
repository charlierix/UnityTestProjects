using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoggerTester : MonoBehaviour
{
    public string Folder = @"d:\temp";

    public Button btnNewFrame;
    public Button btnSave;

    public Button btnDot;
    public Button btnLine;
    public Button btnCircle;
    public Button btnSquare;
    public Button btnAxisLines;

    public Button btnFrameText;
    public Button btnGlobalText;

    public Toggle chkPopulated;

    private DebugLogger _logger = null;

    private void Start()
    {
        _logger = new DebugLogger(Folder, true);
        _logger.DefineCategory("a", StaticRandom.ColorHSV(), StaticRandom.NextFloat(0.5f, 2f));
        _logger.DefineCategory("b", StaticRandom.ColorHSV(), StaticRandom.NextFloat(0.5f, 2f));
        _logger.DefineCategory("c", StaticRandom.ColorHSV(), StaticRandom.NextFloat(0.5f, 2f));

        if (btnNewFrame != null)
            btnNewFrame.onClick.AddListener(NewFrame_Click);

        if (btnSave != null)
            btnSave.onClick.AddListener(Save_Click);

        if (btnDot != null)
            btnDot.onClick.AddListener(Dot_Click);

        if (btnLine != null)
            btnLine.onClick.AddListener(Line_Click);

        if (btnCircle != null)
            btnCircle.onClick.AddListener(Circle_Click);

        if (btnSquare != null)
            btnSquare.onClick.AddListener(Square_Click);

        if (btnAxisLines != null)
            btnAxisLines.onClick.AddListener(AxisLines_Click);

        if (btnFrameText != null)
            btnFrameText.onClick.AddListener(FrameText_Click);

        if (btnGlobalText != null)
            btnGlobalText.onClick.AddListener(GlobalText_Click);
    }

    private void Update()
    {
        if (_logger != null && chkPopulated != null)
            chkPopulated.isOn = _logger.IsPopulated();
    }

    private void NewFrame_Click()
    {
        _logger.NewFrame(DateTime.Now.ToString("HH:mm:ss"), StaticRandom.ColorHSV());
    }
    private void Save_Click()
    {
        _logger.Save(Guid.NewGuid().ToString());
    }
    private void Dot_Click()
    {
        _logger.Add_Dot(Math3D.GetRandomVector_Spherical(12));
    }
    private void Line_Click()
    {
        _logger.Add_Line(Math3D.GetRandomVector_Spherical(12), Math3D.GetRandomVector_Spherical(12));
    }
    private void Circle_Click()
    {
        _logger.Add_Circle(Math3D.GetRandomVector_Spherical(12), Math3D.GetRandomVector_Spherical_Shell(1), StaticRandom.NextFloat(0.5f, 3f));
    }
    private void Square_Click()
    {
        _logger.Add_Square(Math3D.GetRandomVector_Spherical(12), Math3D.GetRandomVector_Spherical_Shell(1), StaticRandom.NextFloat(0.5f, 3f), StaticRandom.NextFloat(0.5f, 3f));
    }
    private void AxisLines_Click()
    {
        _logger.Add_AxisLines(Math3D.GetRandomVector_Spherical(12), StaticRandom.RotationUniform(), StaticRandom.NextFloat(0.5f, 1.5f));
    }
    private void FrameText_Click()
    {
        _logger.WriteLine_Frame(Guid.NewGuid().ToString());
    }
    private void GlobalText_Click()
    {
        _logger.WriteLine_Global(Guid.NewGuid().ToString());
    }
}
