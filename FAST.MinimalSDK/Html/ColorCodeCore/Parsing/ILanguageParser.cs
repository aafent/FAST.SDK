// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FAST.Html.ColorCode.Parsing
{
    internal /*public*/ interface ILanguageParser
    {
        void Parse(string sourceCode,
                   ILanguage language,
                   Action<string, IList<Scope>> parseHandler);
    }
}