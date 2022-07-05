using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TypeExtensions
{

    public static bool ToBoolean(this object o)
    {

        bool v = false;
        if (bool.TryParse(o?.ToString(), out v))
        {
            return v;
        }
        return false;
    }

    public static int ToInt32(this object o)
    {

        int v = 0;
        if (int.TryParse(o?.ToString(), out v))
        {
            return v;
        }
        return 0;
    }


    public static long ToInt64(this object o)
    {
        long v = 0L;
        if (long.TryParse(o?.ToString(), out v))
        {
            return v;
        }
        return 0L;
    }

    public static double ToDouble(this object o)
    {
        double v = 0D;
        if (double.TryParse(o?.ToString(), out v))
        {
            return v;
        }
        return 0D;
    }


    public static float ToSingle(this object o)
    {
        float v = 0F;
        if (float.TryParse(o?.ToString(), out v))
        {
            return v;
        }
        return 0F;
    }

}
