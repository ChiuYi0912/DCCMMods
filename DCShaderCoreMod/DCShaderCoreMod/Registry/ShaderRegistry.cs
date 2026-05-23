using dc.hxsl;
using System.Collections.Generic;

namespace DCShaderCore.Registry;

public class ShaderRegistry
{
    private readonly List<ShaderEntry> entries = [];
    private readonly Dictionary<string, Shader> namedShaders = [];

    public IReadOnlyList<ShaderEntry> GetEntries() => entries;

    public void Register(string id, Shader shader)
    {
        entries.Add(new ShaderEntry(id, shader));
        namedShaders[id] = shader;
    }

    public Shader? GetShader(string id) =>
        namedShaders.TryGetValue(id, out var shader) ? shader : null;

    public RuntimeShader? GetRuntimeShader(string id) =>
        namedShaders.TryGetValue(id, out var shader)
            ? ShaderCore.Instance.Cache.GetShader(new ShaderList(shader, null))
            : null;
}

public record ShaderEntry(string Id, Shader Shader);
