using System;
using System.Collections;
using System.Text;


class Program
{
    static string message;
    static DESKey key;
    static BitArray IV;
    static string mode;
    static string cryptFile;

    private static void ParseArgs(string[] args)
    {
        //ДА я в курсе про switch
        switch (args.Length) {
            case 0:
      
                Console.WriteLine("Введите сообщение: ");
                message = Console.ReadLine();
                key = new DESKey();
                IV = new DESKey().key64;
                mode = "ECB";
                cryptFile = "";
                return;

            case 1:
                Console.WriteLine("Введите сообщение: ");
                message = Console.ReadLine();
                key = new DESKey();
                IV = new DESKey().key64;
                mode = args[0];
                cryptFile = "";
                return;

            case 2:
                message = System.IO.File.ReadAllText(args[1]);
                key = new DESKey();
                IV = new DESKey().key64;
                mode = args[0];
                cryptFile = "";
                return;
            case 3:
                message = System.IO.File.ReadAllText(args[1]);
                key = new DESKey(System.IO.File.ReadAllText(args[2]));
                IV = new DESKey().key64;
                mode = args[0];
                cryptFile = "";
                return;
            case 4:
                message = System.IO.File.ReadAllText(args[1]);
                key = new DESKey(System.IO.File.ReadAllText(args[2]));
                IV = new DESKey().key64;
                mode = args[0];
                cryptFile = args[3];
                return;
            case 5:
                message = System.IO.File.ReadAllText(args[1]);
                key = new DESKey(System.IO.File.ReadAllText(args[2]));
                IV = BitArrayExtension.GetFromString(System.IO.File.ReadAllText(args[4]));
                if (IV.Length != 64) Console.WriteLine("Недопустимая длина вектора - " + IV.Length);
                IV = new DESKey().key64;
                mode = args[0];
                cryptFile = args[3];
                return;


        }
    }


    static void Main(string[] args)
    {
        Console.WriteLine("DES.exe mode dataFile keyFile cryptFile IVfile");
        ParseArgs(args);
        Console.WriteLine("Сообщение - " + message);
        Console.WriteLine("Ключ - " + key);
        
        Console.WriteLine("Режим кодирования - " + mode);
        string crypt;
        var start = DateTime.Now;
        if (mode == "PCBC")
        {
            crypt = DES.Encrypt_PCBC(message, key, IV);
            
        } else
        {
            crypt = DES.Encrypt_ECB(message, key);
        }
        Console.WriteLine("Время шифрования - " + (DateTime.Now - start).TotalMilliseconds + " мс");
        if(mode == "PCBC") Console.WriteLine("IV - " + IV.ToString("X"));
        Console.WriteLine("Зашифрованные данные - " + crypt);
        if(cryptFile != "") System.IO.File.WriteAllText(@"cryptdata.txt", crypt);
        if (mode == "PCBC")
        {
            crypt = DES.Decrypt_PCBC(crypt, key, IV);

        }
        else
        {
            crypt = DES.Decrypt_ECB(crypt, key);
        }
        Console.WriteLine("Расшифрованные данные - " + crypt);
    }
}
