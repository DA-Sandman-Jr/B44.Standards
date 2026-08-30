using System;
using System.Security.Cryptography;

namespace AmbientTime;

internal static class Ambient
{
    public static long Date() => DateTime.Today.Ticks;

    public static long Ticks() => Environment.TickCount64;

    public static int Ticks32() => Environment.TickCount;

    public static byte[] Entropy()
    {
        byte[] buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        return buffer;
    }
}
