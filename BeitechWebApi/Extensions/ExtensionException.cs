using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeitechWebApi.Extensions
{
    public static partial class ExtensionException
    {
        public static Exception GetFirstException(this Exception value)
        {
            Exception baseEX, firstEX;
            baseEX = firstEX = value.GetBaseException();
            var baseEXType = value.GetBaseException().GetType();

            if (baseEX.InnerException.IsNotNull())
            {
                firstEX = baseEX.InnerException;
            }

            return firstEX;
        }

    }
}