#if NETSTANDARD2_1_OR_GREATER
global using IReadOnlySetDecorationEntry = System.Collections.Generic.IReadOnlySet<cilspirv.Spirv.DecorationEntry>;
global using IReadOnlySetCapability = System.Collections.Generic.IReadOnlySet<cilspirv.Spirv.Capability>;
#else
global using IReadOnlySetDecorationEntry = System.Collections.Generic.ISet<cilspirv.Spirv.DecorationEntry>;
global using IReadOnlySetCapability = System.Collections.Generic.ISet<cilspirv.Spirv.Capability>;
#endif
