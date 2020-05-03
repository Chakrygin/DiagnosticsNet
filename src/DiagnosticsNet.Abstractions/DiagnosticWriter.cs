using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DiagnosticsNet
{
    internal sealed class DiagnosticWriter
    {
        private readonly MethodInfo _method;
        private readonly object _target;

        private Action<object>? _action;

        public DiagnosticWriter(MethodInfo method, object target)
        {
            _method = method;
            _target = target;
        }

        public void Write(object value)
        {
            if (_action == null)
            {
                lock (this)
                {
                    if (_action == null)
                    {
                        _action = CreateAction(value);
                    }
                }
            }

            _action(value);
        }

        private Action<object> CreateAction(object value)
        {
            var valueType = value.GetType();
            var targetType = _target.GetType();

            var dm = new DynamicMethod("",
                returnType: typeof(void),
                parameterTypes: new[] {targetType, typeof(object)});

            var il = dm.GetILGenerator();

            var properties = valueType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            il.Emit(OpCodes.Ldarg_0);

            var parameters = _method.GetParameters();
            foreach (var parameter in parameters)
            {
                var parameterName = parameter.Name;
                var parameterType = parameter.ParameterType;

                if (!properties.TryGetValue(parameterName, out var property))
                {
                    il.EmitDefault(parameterType);
                    continue;
                }

                if (!parameterType.IsAssignableFrom(property.PropertyType))
                {
                    il.EmitDefault(parameterType);
                    continue;
                }

                if (!property.CanRead)
                {
                    il.EmitDefault(parameterType);
                    continue;
                }

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, property.GetMethod);
            }

            il.Emit(OpCodes.Callvirt, _method);
            il.Emit(OpCodes.Ret);

            var result = dm.CreateDelegate(typeof(Action<object>), _target);

            return (Action<object>) result;
        }
    }
}
