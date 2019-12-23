using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

using Line = System.Collections.Generic.List<Juniper.Primrose.Token>;

namespace Juniper.Primrose
{
    public partial class Grammar
    {
        /// <summary>
        /// A grammar and an interpreter for a BASIC-like language.
        /// </summary>
        public static readonly Grammar Basic = new Basic();
    }
}