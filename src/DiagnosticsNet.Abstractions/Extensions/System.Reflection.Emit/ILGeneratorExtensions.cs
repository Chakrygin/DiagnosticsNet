// ReSharper disable once CheckNamespace

namespace System.Reflection.Emit
{
    internal static class ILGeneratorExtensions
    {
        private static readonly FieldInfo DefaultDecimalField =
            typeof(Decimal).GetField("Zero");

        private static readonly FieldInfo DefaultGuidField =
            typeof(Guid).GetField("Empty");

        public static void EmitDefault(this ILGenerator il, Type type)
        {
            if (!type.IsValueType)
            {
                // null
                il.Emit(OpCodes.Ldnull);
                return;
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                // default(T?)
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Unbox_Any, type);
                return;
            }

            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }

            if (type == typeof(bool) ||
                type == typeof(byte) ||
                type == typeof(char) ||
                type == typeof(int) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(uint) ||
                type == typeof(ushort))
            {
                // 0
                il.Emit(OpCodes.Ldc_I4_0);
                return;
            }

            if (type == typeof(long) ||
                type == typeof(ulong))
            {
                // 0L
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Conv_I8);
                return;
            }

            if (type == typeof(float))
            {
                // 0.0f
                il.Emit(OpCodes.Ldc_R4, 0.0f);
                return;
            }

            if (type == typeof(double))
            {
                // 0.0d
                il.Emit(OpCodes.Ldc_R8, 0.0d);
                return;
            }

            if (type == typeof(decimal))
            {
                // Decimal.Zero
                il.Emit(OpCodes.Ldsfld, DefaultDecimalField);
                return;
            }

            if (type == typeof(Guid))
            {
                // Guid.Empty
                il.Emit(OpCodes.Ldsfld, DefaultGuidField);
                return;
            }

            var local = il.DeclareLocal(type);

            // default(T)
            il.Emit(OpCodes.Ldloca, local);
            il.Emit(OpCodes.Initobj, type);
            il.Emit(OpCodes.Ldloc, local);
        }
    }
}
