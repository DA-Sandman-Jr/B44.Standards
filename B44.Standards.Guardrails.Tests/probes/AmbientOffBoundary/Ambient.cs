using System;
using System.Security.Cryptography;

namespace AmbientOffBoundary;

internal static class Ambient
{
    public static long Date() => DateTime.Today.Ticks;

    public static long Ticks() => Environment.TickCount64;

    public static byte[] Entropy()
    {
        byte[] buffer = new byte[8];
        RandomNumberGenerator.Fill(buffer);
        return buffer;
    }
}
