using System;

public static class Encryptor
{
    public static string EncryptBase64(string value)
    {
        string result = "";

        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                byte[] bytes = System.Text.UnicodeEncoding.UTF8.GetBytes(value);
                result = Convert.ToBase64String(bytes);
            }
        }
        catch { }

        return result;
    }

    public static string DecryptBase64(string value)
    {
        string result = "";

        try
        {
            byte[] bytes = Convert.FromBase64String(value);

            result = System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch { }

        return result;
    }
}
