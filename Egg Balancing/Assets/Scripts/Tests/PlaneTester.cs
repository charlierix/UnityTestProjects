using PerfectlyNormalUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTester : MonoBehaviour
{
    private const string PLANE = "plane";
    private const string HIT = "hit";
    private const string TEST_POINT = "test point";
    private const string PARENT = "parent";
    private const string INV_PLANE = "inverse plane";

    public Transform Parent_Obj;
    public Transform Child_Obj;

    private DebugLogger _log;
    private int _frame_count = 0;

    void Start()
    {
        // Test 1
        //var log = new DebugLogger(@"d:\temp\plane tests", true);

        //log.DefineCategory(PLANE, Color.white);
        //log.DefineCategory(HIT, Color.green);
        //log.DefineCategory(TEST_POINT, Color.gray);

        //for (int i = 0; i < 12; i++)
        //{
        //    TestFrame1(log);
        //}

        //log.Save();

        // Test 2
        _log = new DebugLogger(@"d:\temp\plane tests 2", true);

        _log.DefineCategory(PLANE, Color.white);
        _log.DefineCategory(HIT, Color.green);
        _log.DefineCategory(TEST_POINT, Color.gray);
        _log.DefineCategory(PARENT, Color.magenta);
        _log.DefineCategory(INV_PLANE, Color.cyan);
    }

    private void Update()
    {
        const int MAX_COUNT = 12;

        if (_frame_count >= MAX_COUNT)
            return;

        Parent_Obj.position = Random.insideUnitSphere * 12f;
        Parent_Obj.rotation = Random.rotationUniform;

        _log.NewFrame();

        _log.Add_Dot(Parent_Obj.position, PARENT);
        _log.Add_Circle(Parent_Obj.position, Parent_Obj.up, 1, PARENT);

        _log.Add_Dot(Child_Obj.position, PLANE);
        _log.Add_Circle(Child_Obj.position, Child_Obj.up, 1, PLANE);

        // ----------- World -----------

        Plane plane = new Plane(Child_Obj.up, Child_Obj.position);

        for (int i = 0; i < 12; i++)
        {
            Vector3 test_point = Child_Obj.position + (Random.insideUnitSphere * 4f);

            Vector3 hit = Math3D.GetClosestPoint_Plane_Point(plane, test_point);

            _log.Add_Dot(test_point, TEST_POINT);
            _log.Add_Line(test_point, hit, TEST_POINT);

            _log.Add_Dot(hit, HIT);
            _log.Add_Line(Child_Obj.position, hit, HIT);
        }

        // ----------- Model -----------

        Vector3 center = Parent_Obj.InverseTransformPoint(Child_Obj.position);
        Vector3 normal = Parent_Obj.InverseTransformDirection(Child_Obj.up);
        plane = new Plane(normal, center);

        _log.Add_Dot(center, INV_PLANE);
        _log.Add_Circle(center, normal, 1, INV_PLANE);
        _log.Add_Line(center, center + Vector3.up, color: Color.black);

        for (int i = 0; i < 12; i++)
        {
            Vector3 test_point = center + (Random.insideUnitSphere * 4f);

            Vector3 hit = Math3D.GetClosestPoint_Plane_Point(plane, test_point);

            _log.Add_Dot(test_point, TEST_POINT);
            _log.Add_Line(test_point, hit, TEST_POINT);

            _log.Add_Dot(hit, HIT);
            _log.Add_Line(center, hit, HIT);
        }

        _frame_count++;
        if (_frame_count >= MAX_COUNT)
            _log.Save();
    }

    private static void TestFrame1(DebugLogger log)
    {
        log.NewFrame();

        Vector3 center = Random.insideUnitSphere * 12f;
        Vector3 normal = Random.onUnitSphere;
        Plane plane = new Plane(normal, center);

        log.Add_Dot(center, PLANE);
        log.Add_Circle(center, normal, 1, PLANE);

        for (int i = 0; i < 12; i++)
        {
            Vector3 test_point = center + (Random.insideUnitSphere * 4f);

            Vector3 hit = Math3D.GetClosestPoint_Plane_Point(plane, test_point);

            log.Add_Dot(test_point, TEST_POINT);
            log.Add_Line(test_point, hit, TEST_POINT);

            log.Add_Dot(hit, HIT);
            log.Add_Line(center, hit, HIT);
        }
    }
}
