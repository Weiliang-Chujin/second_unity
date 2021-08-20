namespace Core.Scripts.AnimationCollector.Assembly
{
    public static class Ext
    {
        public static bool IsNull(this  object @object)
        {
            return @object == null || @object.Equals(null);
        }
    }
}