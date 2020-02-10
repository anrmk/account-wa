using System;

namespace Core.Extension {
    public static class RandomExtansion {
        static readonly Random rnd = new Random();
        public static DateTime GetRandomDateTime(DateTime from, DateTime to) {
            var range = to - from;
            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));
            return from + randTimeSpan;
        }

        public static int GetRandomNumber(int from, int to) {
            return rnd.Next(from, to);
        }
    }
}
