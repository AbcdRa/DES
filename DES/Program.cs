using System;
using System.Collections;
using System.Text;


class Program
{


    static void Test1()
    {
        //string message = "iamnorma";
        Console.WriteLine(new DESKey("1010101010111011000010010001100000100111001101101100110011011101"));
        //BitArray data = new BitArray(Encoding.UTF8.GetBytes(message));
        BitArray data = BitArrayExtension.GetFromString("0001001000110100010101101010101111001101000100110010010100110110");
        BitArray key64 = BitArrayExtension.GetFromString("1010101010111011000010010001100000100111001101101100110011011101");
        //BitArray key64 = BitArrayExtension.GetFromString("01010101010101010101010101010101010101010101010101010101");
        DESKey test = new DESKey("01FE01FE01FE01FE", "X");
        DESKey test1 = new DESKey("FE01FE01FE01FE01", "X");
        Console.WriteLine(test.IsWeakKey());
        Console.WriteLine(test.key56.ToString("B"));
        Console.WriteLine(data.ToString("X"));
        Console.WriteLine(key64.ToString("X"));
        var crypt = DES.Encrypt(data, test1.key64);
        Console.WriteLine("РЕЗУЛЬТАТ - " + crypt.ToString("X"));
        Console.WriteLine("РЕЗУЛЬТАТ ДЕШИФРОВКИ - " + DES.Encrypt(crypt, test.key64).ToString("X"));
    }


    static void Main(string[] args)
    {
        string message = System.IO.File.ReadAllText(@"D:\Projects\C#\DES\DES\data.txt");
        DESKey key = DESKey.GetFromFile(@"D:\Projects\C#\DES\DES\key.txt", "B");
        Console.WriteLine("Сообщение - " + message);
        Console.WriteLine("Ключ - " + key);
        var start = DateTime.Now;
        string crypt = DES.Encrypt_ECB(message, key);
        Console.WriteLine("Время шифрования - " + (DateTime.Now - start).TotalMilliseconds + " мс");
        Console.WriteLine("Зашифрованные данные - " + crypt);
        //System.IO.File.WriteAllText(@"D:\Projects\C#\DES\DES\cryptdata.txt", crypt);
        Console.WriteLine("Расшифрованные данные - " + DES.Decrypt_ECB(crypt, key));

    }
}
