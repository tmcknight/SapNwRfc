using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class DateOnlyField : Field<DateOnly?>
    {
        private const string RfcDateFormat = "yyyyMMdd";
        private static readonly string ZeroRfcDateString = new string('0', 8);
        private static readonly string EmptyRfcDateString = new string(' ', 8);

        public DateOnlyField(string name, DateOnly? value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            string stringValue = Value?.ToString(RfcDateFormat, CultureInfo.InvariantCulture) ?? ZeroRfcDateString;

            RfcResultCode resultCode = interop.SetDate(
                dataHandle: dataHandle,
                name: Name,
                date: stringValue.ToCharArray(),
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static DateOnlyField Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            char[] buffer = EmptyRfcDateString.ToCharArray();

            RfcResultCode resultCode = interop.GetDate(
                dataHandle: dataHandle,
                name: name,
                emptyDate: buffer,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            string dateString = new string(buffer);

            if (dateString == EmptyRfcDateString || dateString == ZeroRfcDateString)
                return new DateOnlyField(name, null);

            if (!DateOnly.TryParseExact(dateString, RfcDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
                return new DateOnlyField(name, null);

            return new DateOnlyField(name, date);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => Value.HasValue ? $"{Name} = {Value:yyyy-MM-dd}" : $"{Name} = No date";
    }
}
