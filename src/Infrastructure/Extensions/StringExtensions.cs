﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicSoftware.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }   
    }
}
