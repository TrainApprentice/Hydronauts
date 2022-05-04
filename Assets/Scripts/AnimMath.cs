using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimMath 
{
    /// <summary>
    /// Linearly interpolates between two floats, returning the percentage input between the two
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="percent"></param>
    /// <param name="allowExtrapolation"></param>
    /// <returns>The interpolated value as a float</returns>
    public static float Lerp(float a, float b, float percent, bool allowExtrapolation = false)
    {
        if (!allowExtrapolation)
        {
            if (percent > 1) percent = 1;
            if (percent < 0) percent = 0;
        }

        return (b - a) * percent + a;
    }
    /// <summary>
    /// Linearly interpolates between two vectors, returning the percentage input between the two
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="percent"></param>
    /// <param name="allowExtrapolation"></param>
    /// <returns>The interpolated value as a Vector3</returns>
    public static Vector3 Lerp(Vector3 a, Vector3 b, float percent, bool allowExtrapolation = true)
    {
        if (!allowExtrapolation)
        {
            if (percent > 1) percent = 1;
            if (percent < 0) percent = 0;
        }

        return (b - a) * percent + a;
    }
    /// <summary>
    /// Linearly interpolates between two Quaternions, returning the percentage input between the two
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="percent"></param>
    /// <param name="allowExtrapolation"></param>
    /// <returns>The interpolated value as a Quaternion</returns>
    public static Quaternion Lerp(Quaternion a, Quaternion b, float percent, bool allowExtrapolation = false, bool doWrap = true)
    {
        if(doWrap) b = WrapQuaternion(a, b);
        
        if (!allowExtrapolation)
        {
            if (percent > 1) percent = 1;
            if (percent < 0) percent = 0;
        }

        var lx = Lerp(a.x, b.x, percent, allowExtrapolation);
        var ly = Lerp(a.y, b.y, percent, allowExtrapolation);
        var lz = Lerp(a.z, b.z, percent, allowExtrapolation);
        var lw = Lerp(a.w, b.w, percent, allowExtrapolation);

        return new Quaternion(lx, ly, lz, lw);
        
    }

    /// <summary>
    /// Maps an input value between two ranges, returning its mapped value from the second range
    /// </summary>
    /// <param name="val"></param>
    /// <param name="inMin"></param>
    /// <param name="inMax"></param>
    /// <param name="outMin"></param>
    /// <param name="outMax"></param>
    /// <returns>The mapped value</returns>
    public static float Map(float val, float inMin, float inMax, float outMin, float outMax)
    {
        float p = (val - inMin) / (inMax - inMin);

        return Lerp(outMin, outMax, p);
    }
    /// <summary>
    /// Eases from the current input value toward its target value, returning the lerp'd value between them
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="percentLeftAfter1Second"></param>
    /// <param name="dt"></param>
    /// <returns>The lerp'd current value as a float</returns>
    public static float Ease(float current, float target, float percentLeftAfter1Second, float dt = -1)
    {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, dt);

        return Lerp(current, target, p);
    }
    /// <summary>
    /// Eases from the current input value toward its target value, returning the lerp'd value between them
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="percentLeftAfter1Second"></param>
    /// <param name="dt"></param>
    /// <returns>The lerp'd current value as a Vector3</returns>
    public static Vector3 Ease(Vector3 current, Vector3 target, float percentLeftAfter1Second, float dt = -1)
    {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, dt);

        return Lerp(current, target, p);
    }
    /// <summary>
    /// Eases from the current input value toward its target value, returning the lerp'd value between them
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="percentLeftAfter1Second"></param>
    /// <param name="dt"></param>
    /// <returns>The lerp'd current value as a Quaternion</returns>
    public static Quaternion Ease(Quaternion current, Quaternion target, float percentLeftAfter1Second, bool doWrap = true, float dt = -1, bool allowExtrapolation = false)
    {
        if (dt < 0) dt = Time.deltaTime;
        float p = 1 - Mathf.Pow(percentLeftAfter1Second, dt);

        return Lerp(current, target, p, allowExtrapolation, doWrap);
    }
    
    /// <summary>
    /// Trying to ease between angles > 180 deg? You need to wrap your angles!
    /// </summary>
    /// <param name="baseAngle">This angle won't change</param>
    /// <param name="angleToWrap">This angle will change so that it is relative to baseAngle</param>
    /// <returns>The wrapped angle in degrees</returns>
    public static float AngleWrapDegrees(float baseAngle, float angleToWrap)
    {
        while (angleToWrap > baseAngle + 180) angleToWrap -= 360;
        while (angleToWrap < baseAngle - 180) angleToWrap += 360;

        return angleToWrap;
    }
    /// <summary>
    /// Trying to ease between angles > 180 deg? You need to wrap your angles!
    /// </summary>
    /// <param name="baseAngle">This angle won't change</param>
    /// <param name="angleToWrap">This angle will change so that it is relative to baseAngle</param>
    /// <returns>The wrapped angle as a Quaternion</returns>
    public static Quaternion WrapQuaternion(Quaternion baseAngle, Quaternion angleToWrap)
    {
        float alignment = Quaternion.Dot(baseAngle, angleToWrap);

        if(alignment < 0)
        {
            angleToWrap.x *= -1;
            angleToWrap.y *= -1;
            angleToWrap.z *= -1;
            angleToWrap.w *= -1;
        }
        return angleToWrap;
    }
    
}
