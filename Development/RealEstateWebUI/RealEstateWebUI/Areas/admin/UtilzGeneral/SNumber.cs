namespace RealEstateWebUI.Areas.admin.UtilzGeneral
{
    public class SNumber
    {
        public static bool IsNumber(object value)
        {
            try
            {
                int temp = int.Parse(value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int ToNumber(object value, int intDefault = 0)
        {
            try
            {
                int temp = int.Parse(value.ToString());
                return temp;
            }
            catch 
            {
                return intDefault;
            }
        }

        public static double ToDouble(object value, double doubleDefault = 0)
        {
            try
            {
                double temp = double.Parse(value.ToString());
                return temp;
            }
            catch 
            {
                return doubleDefault;
                throw;
            }
        }
    }
}