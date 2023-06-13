using Newtonsoft.Json.Linq;

namespace Genesis.App.Contract.Models.Forms
{
    public class ControlValue
    {
        public EntityType EntityType { get; set; }

        public JToken Value { get; set; }

        public bool TryGet<T>(out T value)
        {
            try
            {
                if (Value != null)
                {
                    value = Value.ToObject<T>();
                    return true;
                }
            }
            catch (Exception) { }

            value = default;
            return false;
        }
    }
}
