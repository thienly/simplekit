using System;
using System.Globalization;

namespace RoomServices
{
    public partial class BigDecimal
    {
        public static implicit operator BigDecimal(decimal value)
        {
            return new BigDecimal
            {
                Value = value.ToString(CultureInfo.InvariantCulture),
            };
        }

        public static implicit operator decimal(BigDecimal value)
        {
            return decimal.Parse(value.Value, CultureInfo.InvariantCulture);
        }
    }
    public partial class UUID
    {
        public static implicit operator UUID(string value)
        {
            return new UUID
            {
                Value = value.ToString(CultureInfo.InvariantCulture),
            };
        }

        public static implicit operator Guid(UUID value)
        {
            return Guid.Parse(value.Value);
        }
    }
}