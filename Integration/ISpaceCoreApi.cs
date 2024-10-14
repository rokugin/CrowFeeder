using System.Reflection;

namespace CrowFeeder.Integration;

public interface ISpaceCoreApi {

    void RegisterSerializerType(Type type);
    void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);

}