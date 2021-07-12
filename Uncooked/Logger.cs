using System;
using System.IO;
using UnityEngine;

namespace Uncooked
{

    internal static class Logger
    {

        private readonly static string PATH = "C:\\Users\\shivv\\Nextcloud\\Downloads\\OvercookedAI-master\\OvercookedAI-Mono\\bin\\Output.txt";

        public static void Log(String message)
        {
            String str = File.ReadAllText(PATH);
            File.WriteAllText(PATH,
                str + message + "\n");
        }

        public static String FormatPosition(Vector3 location)
        {
            return "x=" + location.x + ", y=" + location.y + ", z=" + location.z;
        }

        public static void Clear()
        {
            File.WriteAllText(PATH,
                "");
        }

    }
}
