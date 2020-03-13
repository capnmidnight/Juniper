using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Veldrid;

namespace Juniper.VeldridIntegration
{
    public static class VertexTypeCache
    {
        private static readonly Dictionary<Type, (VertexLayoutDescription description, uint size)> cache = new Dictionary<Type, (VertexLayoutDescription layout, uint size)>();

        public static (VertexLayoutDescription layout, uint size) GetDescription<VertexT>()
            where VertexT : struct
        {
            lock (cache)
            {
                var type = typeof(VertexT);

                if (!cache.ContainsKey(type))
                {
                    var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m is FieldInfo || m is PropertyInfo)
                        .Select(m => (member: m, info: m.GetCustomAttribute<VertexElementAttribute>()))
                        .Where(p => p.info is object)
                        .ToArray();

                    var descriptions = new VertexElementDescription[members.Length];
                    uint size = 0;
                    for(var i = 0; i < members.Length; ++i)
                    {
                        var m = members[i];
                        m.info.Resolve(m.member);
                        descriptions[i] = new VertexElementDescription(
                            m.info.Name,
                            m.info.Format.Value,
                            m.info.Semantics);
                        size += m.info.Size;
                    }

                    var layout = new VertexLayoutDescription(descriptions);
                    cache[type] = (layout, size);
                }

                return cache[type];
            }
        }
    }
}
