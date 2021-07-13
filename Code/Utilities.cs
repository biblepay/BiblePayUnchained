namespace Unchained
{
    public static class Utils
    {
        public static string GetArrayElementAsString(string data, string delim, int iEle)
        {
            string[] vData = data.Split(delim);
            if (iEle <= vData.Length - 1)
            {
                string d = vData[iEle];
                return d;
            }
            return "";
        }
    }
}
