using System;
using System.ComponentModel;

namespace SPALM.SPSF.Library.Converters
{
    /// <summary>
    /// returns available scopes for a feature
    /// </summary>
    public class EventReceiverSynchronizationConverter : TypeConverter
    {
        static string[] validValues = new string[] {
            "", "Synchronous", "Asynchronous" };

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; //no other values allowed
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(validValues);
        }
    }
}
