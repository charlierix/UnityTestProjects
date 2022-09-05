using PerfectlyNormalUnity.DebugLogger_Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SerializeTester : MonoBehaviour
{
    private const string INDENT = "    ";

    void Start()
    {
        //string vec = new Vector3(1, 2, 3).ToString();

        //TestDot();
        //TestFrame();

        TestScene();
    }

    private static void TestDot()
    {
        var dot = new ItemDot()
        {
            color = "FFFFFF",
            position = "1, 2, 3",
            size_mult = 6,
        };

        string json = JsonUtility.ToJson(dot, true);
    }
    private static void TestFrame()
    {
        var frame = GetFrame1();

        // items isn't in the json.  unity's json doesn't support arrays, will need to be manually stitched together
        string json_frame = JsonUtility.ToJson(frame, true);

        string[] json_items = frame.items.
            Select(o => JsonUtility.ToJson(o, true)).
            ToArray();
    }
    private static void TestScene()
    {
        var categories = GetCategories();
        var frames = GetFrames();
        var global_text = GetGlobalText();

        var scene = new LogScene()
        {
            categories = categories,
            frames = frames,
            text = global_text,
        };

        string json1 = JsonUtility.ToJson(scene, true);
        string json2 = ToJSON(scene);
    }

    private static Category[] GetCategories()
    {
        return new[]
        {
            new Category()
            {
                name = "a",
                size_mult = 1,
            },
            new Category()
            {
                name = "b",
                color = "000000",
            },
            new Category()
            {
                name = "c",
                color = "80808080",
                size_mult = 3,
            },
        };
    }

    private static Text[] GetGlobalText()
    {
        return new[]
        {
            new Text()
            {
                color = "20406080",
                fontsize_mult = 2.3f,
                text = "hello there",
            },
            new Text()
            {
                color = "FFFF00",
                fontsize_mult = 0.1f,
                text = "one more",
            },
        };
    }

    private static LogFrame[] GetFrames()
    {
        return new[]
        {
            GetFrame1(),
            GetFrame2(),
        };
    }
    private static LogFrame GetFrame1()
    {
        return new LogFrame()
        {
            back_color = "FF0000",
            items = new ItemBase[]
            {
                new ItemDot()
                {
                    color = "FFFFFF",
                    position = "1, 2, 3",
                    size_mult = 6,
                },
                new ItemCircle_Edge()
                {
                    color = "40000000",
                    center = "1, 2, 3",
                    normal = "0, 0, 1",
                    radius = 3,
                    size_mult = 6,
                    tooltip = "it's a circle",
                },
            },

            name = "frame1",
        };
    }
    private static LogFrame GetFrame2()
    {
        return new LogFrame()
        {
            back_color = "00FF00",
            items = new ItemBase[]
            {
                new ItemLine()
                {
                    category = "CCCCCC",
                    color = "FF00FF",
                    point1 = "1, 2, 3",
                    point2 = "4, 5, 6",
                    size_mult = 0.6f,
                },
                new ItemSquare_Filled()
                {
                    center = "0, 0, 0",
                    normal = "0, 1, 0",
                    size_x = 1,
                    size_y = 2,
                },
            },

            name = "frame2",

            text = new[]
            {
                new Text()
                {
                    text = "text 1",
                },
                new Text()
                {
                    text = "text 2",
                    fontsize_mult = 3,
                },
                new Text()
                {
                    text = "text 3",
                    color = "A0808080",
                },
            },
        };
    }

    private static string ToJSON(LogScene scene)
    {
        string[] lines = new[]
        {
            ToJSON(nameof(scene.categories), scene.categories, ""),
            ToJSON(nameof(scene.frames), scene.frames, ""),
            ToJSON(nameof(scene.text), scene.text, ""),
        }.
        Where(o => !string.IsNullOrWhiteSpace(o)).
        ToArray();

        var retVal = new StringBuilder();

        retVal.AppendLine("{");
        retVal.AppendLine(CombineJSONs(lines, INDENT));
        retVal.Append("}");

        return retVal.ToString();
    }

    private static string ToJSON(string name, Category[] categories, string indent)
    {
        if (categories == null || categories.Length == 0)
            return "";

        string[] lines = categories.
            Select(o => ToJSON(o, indent)).
            ToArray();

        return GetJsonArray(name, lines, indent);
    }
    private static string ToJSON(Category category, string indent)
    {
        var lines = new List<string>();

        AddJsonStringProp(lines, nameof(category.name), category.name);
        AddJsonStringProp(lines, nameof(category.color), category.color);
        AddJsonFloatProp(lines, nameof(category.size_mult), category.size_mult);

        return GetJsonObject(lines.ToArray(), indent);
    }

    private static string ToJSON(string name, LogFrame[] frames, string indent)
    {
        if (frames == null || frames.Length == 0)
            return "";

        string[] lines = frames.
            Select(o => ToJSON(o, indent)).     // no need to add to indent here, that will be done in CombineJSONs
            ToArray();

        return GetJsonArray(name, lines, indent);
    }
    private static string ToJSON(LogFrame frame, string indent)
    {
        var lines = new List<string>();

        AddJsonStringProp(lines, nameof(frame.name), frame.name);
        AddJsonStringProp(lines, nameof(frame.back_color), frame.back_color);

        if (frame.items != null && frame.items.Length > 0)
            lines.Add(ToJSON(frame.items, ""));

        if (frame.text != null && frame.text.Length > 0)
            lines.Add(ToJSON(nameof(frame.text), frame.text, ""));

        return GetJsonObject(lines.ToArray(), indent);
    }

    private static string ToJSON(ItemBase[] items, string indent)
    {
        if (items == null || items.Length == 0)
            return "";

        string[] lines = items.
            Select(o => ToJSON(o, indent)).
            ToArray();

        return GetJsonArray("items", lines, indent);
    }
    private static string ToJSON(ItemBase item, string indent)
    {
        var lines = new List<string>();

        AddJsonStringProp(lines, nameof(item.category), item.category);
        AddJsonStringProp(lines, nameof(item.color), item.color);
        AddJsonFloatProp(lines, nameof(item.size_mult), item.size_mult);
        AddJsonStringProp(lines, nameof(item.tooltip), item.tooltip);

        if (item is ItemDot dot)
        {
            AddJsonStringProp(lines, nameof(dot.position), dot.position);
        }
        else if(item is ItemLine line)
        {
            AddJsonStringProp(lines, nameof(line.point1), line.point1);
            AddJsonStringProp(lines, nameof(line.point2), line.point2);
        }
        else if(item is ItemCircle_Edge circle)
        {
            AddJsonStringProp(lines, nameof(circle.center), circle.center);
            AddJsonStringProp(lines, nameof(circle.normal), circle.normal);
            AddJsonFloatProp(lines, nameof(circle.radius), circle.radius);
        }
        else if(item is ItemSquare_Filled square)
        {
            AddJsonStringProp(lines, nameof(square.center), square.center);
            AddJsonStringProp(lines, nameof(square.normal), square.normal);
            AddJsonFloatProp(lines, nameof(square.size_x), square.size_x);
            AddJsonFloatProp(lines, nameof(square.size_y), square.size_y);
        }
        else
        {
            return JsonUtility.ToJson(item, true);      // should never happen.  this would be ugly json, but would still work (default includes all types regardless if they are null or not)
        }

        return GetJsonObject(lines.ToArray(), indent);
    }

    private static string ToJSON(string name, Text[] texts, string indent)
    {
        if (texts == null || texts.Length == 0)
            return "";

        string[] lines = texts.
            Select(o => ToJSON(o, indent)).
            ToArray();

        return GetJsonArray(name, lines, indent);
    }
    private static string ToJSON(Text text, string indent)
    {
        var lines = new List<string>();

        AddJsonStringProp(lines, nameof(text.text), text.text);
        AddJsonStringProp(lines, nameof(text.color), text.color);
        AddJsonFloatProp(lines, nameof(text.fontsize_mult), text.fontsize_mult);

        return GetJsonObject(lines.ToArray(), indent);
    }

    private static string GetJsonArray(string name, string[] lines, string indent)
    {
        var retVal = new StringBuilder();

        retVal.Append(indent);
        retVal.AppendLine($"\"{name}\": [");

        retVal.AppendLine(CombineJSONs(lines, indent + INDENT));

        retVal.Append(indent);
        retVal.Append("]");

        return retVal.ToString();
    }
    private static string GetJsonObject(string[] lines, string indent)
    {
        var retVal = new StringBuilder();

        retVal.Append(indent);
        retVal.AppendLine("{");

        retVal.AppendLine(CombineJSONs(lines.ToArray(), indent + INDENT));

        retVal.Append(indent);
        retVal.Append("}");

        return retVal.ToString();
    }

    private static void AddJsonStringProp(List<string> list, string name, string value)
    {
        if (!string.IsNullOrEmpty(value))
            list.Add($"\"{name}\": \"{value}\"");
    }
    private static void AddJsonFloatProp(List<string> list, string name, float? value)
    {
        if (value != null)
            list.Add($"\"{name}\": {value}");
    }

    private static string CombineJSONs(string[] jsons, string indent)
    {
        var retVal = new StringBuilder();

        for (int i = 0; i < jsons.Length; i++)
        {
            //retVal.Append(jsons[i]);      // can't do this directly, because the indent needs to be injected at the front of each line

            string[] lines = jsons[i].Replace("\r\n", "\n").Split('\n');
            for (int j = 0; j < lines.Length; j++)
            {
                retVal.Append(indent);
                retVal.Append(lines[j]);

                if (j < lines.Length - 1)
                    retVal.AppendLine();
            }

            if (i < jsons.Length - 1)
                retVal.AppendLine(",");
        }

        return retVal.ToString();
    }
}
