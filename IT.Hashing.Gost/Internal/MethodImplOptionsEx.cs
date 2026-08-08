using System.Runtime.CompilerServices;

namespace IT.Hashing.Gost.Internal;

internal static class MethodImplOptionsEx
{
    /// <summary>
    /// Optimization hint for hot-path methods that benefit from both inlining and aggressive optimization.
    /// </summary>
    /// <remarks>
    /// On .NET 8+: <c>AggressiveInlining | AggressiveOptimization</c>.
    /// On older frameworks: <see cref="MethodImplOptions.AggressiveInlining"/> only.
    /// </remarks>
#if NET8_0_OR_GREATER
    public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
#else
    public const MethodImplOptions HotPath = MethodImplOptions.AggressiveInlining;
#endif

    /// <summary>
    /// Optimization hint for methods with loops or complex logic that benefit
    /// from aggressive optimization but are too large to inline.
    /// </summary>
    /// <remarks>
    /// On .NET 8+: On .NET 8+: <c>NoInlining | AggressiveOptimization</c>.
    /// On older frameworks: <see cref="MethodImplOptions.NoInlining"/> (best available hint).
    /// </remarks>
#if NET8_0_OR_GREATER
    public const MethodImplOptions OptimizedLoop = MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization;
#else
    public const MethodImplOptions OptimizedLoop = MethodImplOptions.NoInlining;
#endif
}