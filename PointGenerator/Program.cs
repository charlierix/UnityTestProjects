using System.Numerics;

/// <summary>
/// Helps make chains
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        Vector3 from = Vector3.Zero;
        Vector3 to = new Vector3(0, 0, -2);

        int internal_points = 4;        // if 3, there will be: { from, int0, int1, int2, to }
        float pow = 0.9f;      // allows the points to be placed non linear

        Vector3[] points = GetPoints(from, to, internal_points, pow);

        Console.WriteLine("points (global)");

        for (int i = 0; i < points.Length; i++)
        {
            Console.WriteLine(FormatPoint(points[i]));
        }

        Console.WriteLine();
        Console.WriteLine("half points (global)");

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 half_point = points[i] + ((points[i + 1] - points[i]) / 2);
            Console.WriteLine(FormatPoint(half_point));
        }

        Console.WriteLine();
        Console.WriteLine("points (relative)");

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 rel_point = points[i + 1] - points[i];
            Console.WriteLine(FormatPoint(rel_point));
        }

        Console.WriteLine();
        Console.WriteLine("half points (relative)");

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 half_point = (points[i + 1] - points[i]) / 2;
            Console.WriteLine(FormatPoint(half_point));
        }

        Console.WriteLine();
        Console.WriteLine("distance | half distance");

        for (int i = 0; i < points.Length - 1; i++)
        {
            float distance = (points[i + 1] - points[i]).Length();
            Console.WriteLine($"{FormatScalar(distance)}\t{FormatScalar(distance / 2f)}");
        }
    }

    private static Vector3[] GetPoints(Vector3 from, Vector3 to, int internal_points, float pow)
    {
        float[] x = GetPoints(from.X, to.X, internal_points, pow);
        float[] y = GetPoints(from.Y, to.Y, internal_points, pow);
        float[] z = GetPoints(from.Z, to.Z, internal_points, pow);

        Vector3[] retVal = new Vector3[x.Length];

        for (int i = 0; i < x.Length; i++)
        {
            retVal[i] = new Vector3(x[i], y[i], z[i]);
        }

        return retVal;
    }
    private static float[] GetPoints(float from, float to, int internal_points, float pow)
    {
        float[] retVal = new float[internal_points + 2];

        float step = 1f / (retVal.Length - 1);
        float gap = to - from;

        for (int i = 0; i < retVal.Length; i++)
        {
            float percent = i * step;

            retVal[i] = from + ((float)Math.Pow(percent, pow) * gap);
            //retVal[i] = from + (percent * gap);
        }

        return retVal;
    }

    private static string FormatPoint(Vector3 point)
    {
        return $"{FormatScalar(point.X)}\t{FormatScalar(point.Y)}\t{FormatScalar(point.Z)}";
    }
    private static string FormatScalar(float value)
    {
        return Math.Round(value, 3).ToString();
    }
}
