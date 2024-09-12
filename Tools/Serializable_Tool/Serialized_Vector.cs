using System;
using UnityEngine;

[Serializable]
public struct Serialized_Vector3
{
    public float x, y, z;

    #region 构造函数

    
    public Serialized_Vector3(float x = 0, float y = 0, float z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Serialized_Vector3(Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Serialized_Vector3(Vector3Int v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Serialized_Vector3(Vector2 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = 0;
    }

    public Serialized_Vector3(Vector2Int v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = 0;
    }

    #endregion

    public override string ToString()
    {
        return $"({x},{y},{z})";
    }

    #region 隐式转换

    public static implicit operator Serialized_Vector3(Vector3 vector3)
    {
        return new Serialized_Vector3(vector3.x, vector3.y, vector3.z);
    }

    public static implicit operator Vector3(Serialized_Vector3 vector3)
    {
        return new Vector3(vector3.x, vector3.y, vector3.z);
    }

    public static implicit operator Serialized_Vector3(Vector3Int vector3)
    {
        return new Serialized_Vector3(vector3.x, vector3.y, vector3.z);
    }

    public static implicit operator Vector3Int(Serialized_Vector3 vector3)
    {
        return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
    }

    #endregion

    #region 运算重载

    public static Serialized_Vector3 operator +(Serialized_Vector3 v1, Serialized_Vector3 v2)
    {
        return new Serialized_Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Serialized_Vector3 operator -(Serialized_Vector3 v1, Serialized_Vector3 v2)
    {
        return new Serialized_Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static Serialized_Vector3 operator *(Serialized_Vector3 v1, Serialized_Vector3 v2)
    {
        return new Serialized_Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Serialized_Vector3 operator *(Serialized_Vector3 v1, float value)
    {
        return new Serialized_Vector3(v1.x * value, v1.y * value, v1.z * value);
    }

    public static Serialized_Vector3 operator /(Serialized_Vector3 v1, Serialized_Vector3 v2)
    {
        return new Serialized_Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static Serialized_Vector3 operator /(Serialized_Vector3 v1, float value)
    {
        return new Serialized_Vector3(v1.x / value, v1.y / value, v1.z / value);
    }

    #endregion
}

[Serializable]
public struct Serialized_Vector2
{
    public float x, y;

    public Serialized_Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }

    #region 隐式转换

    public static implicit operator Serialized_Vector2(Vector2 vector2)
    {
        return new Serialized_Vector2(vector2.x, vector2.y);
    }

    public static implicit operator Vector2(Serialized_Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y);
    }

    public static implicit operator Serialized_Vector2(Vector2Int vector2)
    {
        return new Serialized_Vector2(vector2.x, vector2.y);
    }

    public static implicit operator Vector2Int(Serialized_Vector2 vector2)
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }

    #endregion

    #region 运算重载

    public static Serialized_Vector2 operator +(Serialized_Vector2 v1, Serialized_Vector2 v2)
    {
        return new Serialized_Vector2(v1.x + v2.x, v1.y + v2.y);
    }

    public static Serialized_Vector2 operator -(Serialized_Vector2 v1, Serialized_Vector2 v2)
    {
        return new Serialized_Vector2(v1.x - v2.x, v1.y - v2.y);
    }

    public static Serialized_Vector2 operator *(Serialized_Vector2 v1, Serialized_Vector2 v2)
    {
        return new Serialized_Vector2(v1.x * v2.x, v1.y * v2.y);
    }

    public static Serialized_Vector2 operator *(Serialized_Vector2 v1, float value)
    {
        return new Serialized_Vector2(v1.x * value, v1.y * value);
    }

    public static Serialized_Vector2 operator /(Serialized_Vector2 v1, Serialized_Vector2 v2)
    {
        return new Serialized_Vector2(v1.x / v2.x, v1.y / v2.y);
    }

    public static Serialized_Vector2 operator /(Serialized_Vector2 v1, float value)
    {
        return new Serialized_Vector2(v1.x / value, v1.y / value);
    }

    #endregion
}

/// <summary>
/// 转换静态拓展
/// </summary>
public static class SerializedVectorExtensions
{
    public static Vector3 ConverToVector3(this Serialized_Vector3 sv3)
    {
        return new Vector3(sv3.x, sv3.y, sv3.z);
    }

    public static Serialized_Vector3 ConverToSVector3(this Vector3 v3)
    {
        return new Serialized_Vector3(v3.x, v3.y, v3.z);
    }

    public static Vector3Int ConverToVector3Int(this Serialized_Vector3 sv3)
    {
        return new Vector3Int((int)sv3.x, (int)sv3.y, (int)sv3.z);
    }

    public static Serialized_Vector3 ConverToSVector3Int(this Vector3Int v3)
    {
        return new Serialized_Vector3(v3.x, v3.y, v3.z);
    }

    public static Vector2 ConverToSVector2(this Serialized_Vector2 sv2)
    {
        return new Vector2(sv2.x, sv2.y);
    }

    public static Vector2Int ConverToVector2Int(this Serialized_Vector2 sv2)
    {
        return new Vector2Int((int)sv2.x, (int)sv2.y);
    }

    public static Serialized_Vector2 ConverToSVector2(this Vector2 v2)
    {
        return new Serialized_Vector2(v2.x, v2.y);
    }

    public static Serialized_Vector2 ConverToSVector2(this Vector2Int v2)
    {
        return new Serialized_Vector2(v2.x, v2.y);
    }
}