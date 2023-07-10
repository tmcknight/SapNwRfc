using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class BooleanField : Field<bool>
    {
        private static readonly char RfcTrueChar = 'X';
        private static readonly char RfcFalseChar = ' ';

        public BooleanField(string name, bool value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            RfcResultCode resultCode = interop.SetChars(
                dataHandle: dataHandle,
                name: Name,
                value: new char[] { Value ? RfcTrueChar : RfcFalseChar },
                valueLength: 1,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static BooleanField Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            var buffer = new char[1];
            RfcResultCode resultCode = interop.GetChars(
                dataHandle: dataHandle,
                name: name,
                charBuffer: buffer,
                bufferLength: (uint)buffer.Length,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new BooleanField(name, buffer[0] == RfcTrueChar ? true : false);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value}";
    }
}
