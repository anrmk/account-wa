using System;
using System.Reflection;

namespace Core.Extension {
    public static class ObjectExtension {
        public static object GetPropValue(this object obj, string name) {
            foreach(string part in name.Split('.')) {
                if(obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if(info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this object obj, string name) {
            object retval = GetPropValue(obj, name);
            if(retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

        public static class TryParseHelper<T> where T : struct {
            private delegate bool TryParseFunc(string str, out T result);

            private static TryParseFunc tryParseFuncCached;

            private static TryParseFunc tryParseCached {
                get {
                    return tryParseFuncCached ?? (tryParseFuncCached = Delegate.CreateDelegate(typeof(TryParseFunc), typeof(T), "TryParse") as TryParseFunc);
                }
            }

            /// <summary>
            /// Tries to convert the specified string representation of a logical value to
            /// its type T equivalent. A return value indicates whether the conversion
            /// succeeded or failed.
            /// </summary>
            /// <param name="value">A string containing the value to try and convert.</param>
            /// <param name="result">If the conversion was successful, the converted value of type T.</param>
            /// <returns>If value was converted successfully, true; otherwise false.</returns>
            public static bool TryParse(string value, out T result) {
                return tryParseCached(value, out result);
            }

            /// <summary>
            /// Tries to convert the specified string representation of a logical value to
            /// its type T equivalent. A return value indicates whether the conversion
            /// succeeded or failed.
            /// </summary>
            /// <param name="value">A string containing the value to try and convert.</param>
            /// <returns>If value was converted successfully, true; otherwise false.</returns>
            public static bool TryParse(string value) {
                T throwaway;
                return TryParse(value, out throwaway);
            }
        }
    }
}
