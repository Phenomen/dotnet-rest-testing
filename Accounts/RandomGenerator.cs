using System;
using System.IO;

namespace Accounts {
    static class RandomGenerator
    {
        static Random _r = new Random();

        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName(); 
            path = path.Replace(".", ""); // Remove period.
            path = path + _r.Next().ToString(); // Add random number
            return path;
        }
    }
}