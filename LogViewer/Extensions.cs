namespace LogViewer
{
    public class Extensions
    {
        public static void SetPropertyValue(object obj, string prpName, object value)
        {

            if (obj != null)
            {
                var prp = obj.GetType().GetProperty(prpName);
                if (prp != null)
                {
                    prp.SetValue(obj, value, null);
                }

            }
        }
    }
}
