﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extension {
    class NumberExtension {
    }
    public static class NumberExtensionMethods {
        public static bool IsBetween(this int value, int Min, int Max) {
            // return (value >= Min && value <= Max);
            if(value >= Min && value <= Max) return true;
            else return false;
        }
    }
}
