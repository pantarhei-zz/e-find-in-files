using System;
using JetBrains.Annotations;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This class is required for extension methods to work when targeting .NET 2.0.
    /// </summary>
    [UsedImplicitly]
    sealed class ExtensionAttribute : Attribute { }
}