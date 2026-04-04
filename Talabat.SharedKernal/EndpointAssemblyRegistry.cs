using System.Reflection;

namespace Talabat.SharedKernal;

public static class EndpointAssemblyRegistry
{
	private static readonly List<Assembly> _assemblies = [];

	public static IReadOnlyCollection<Assembly> Assemblies => _assemblies.AsReadOnly();

	public static void Register(Assembly assembly)
	{
		if (!_assemblies.Contains(assembly))
			_assemblies.Add(assembly);
	}
}
