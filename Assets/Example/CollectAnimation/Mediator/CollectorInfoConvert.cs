using System;
using Core.Scripts.AnimationCollector.Assembly.Core;
using Example.CollectAnimation;

namespace AnimationCollector.Mediator
{
    public static class CollectorInfoConvert
    {
        public static CollectorInfo Create(CurrencyType currencyType, long count)
        {
            if (Enum.TryParse(currencyType.ToString(), out CollectType collectType))
            {
                CollectorInfo info = CollectorInfo.Create(collectType, count);
                return info;
            }

            return null;
        }

        public static CollectType ConvertCollectType(CurrencyType currencyType)
        {
            return (CollectType) currencyType;
        }
    }
}