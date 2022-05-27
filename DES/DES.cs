using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;



class DES
{
    static bool isLog = false; 
    static BitArray ApplyP(BitArray data, byte[] M)
    {
        BitArray R = new BitArray(M.Length);
        for (int i = 0; i < M.Length; i++)
        {
            R[i] = data[M[i] - 1];
        }
        return R;
    }
    static BitArray f(BitArray R, BitArray key)
    {
        BitArray Rexp = ApplyP(R, DESMatrix.MPexp);
        Rexp = Rexp.Xor(key);
        R = ApplyS(Rexp, DESMatrix.SMs);
        R = ApplyP(R, DESMatrix.MPforw);
        return R;
    }
    static BitArray ApplyS(BitArray data, byte[,,] SMs)
    {
        BitArray[] bs = data.Separate(6);
        BitArray[] tbs = new BitArray[8];
        for (int i = 0; i < 8; i++)
        {
            int m = (bs[i][0] ? 2 : 0) + (bs[i][5] ? 1 : 0); //Возможен обратный порядок
            int k = (bs[i][1] ? 8 : 0) + (bs[i][2] ? 4 : 0) + (bs[i][3] ? 2 : 0) + (bs[i][4] ? 1 : 0);
            tbs[i] = BitArrayExtension.FromByte(SMs[i, m, k]);
        }
        BitArray R = BitArrayExtension.JoinBitArrays(tbs, 4);
        return R;
    }

    public static BitArray Encrypt(BitArray data, BitArray key, int isReverse = 0)
    {
        //Делаем первоначальную перестановку
        data = ApplyP(data, DESMatrix.MIP);
        BitArray[] LR = data.Separate(32);
        BitArray key56 = ApplyP(key, DESMatrix.MPkey);
        BitArray[] CD = key56.Separate(28);
        BitArray[] roundKeys = new BitArray[16];
        for (int i = 0; i < 16; i++)
        {
            CD[0] = CD[0].crotl(DESMatrix.shiftBits[i]);
            CD[1] = CD[1].crotl(DESMatrix.shiftBits[i]);
            key56 = BitArrayExtension.JoinBitArrays(CD, 28);
            roundKeys[15 * isReverse + (-2 * isReverse + 1) * i] = ApplyP(key56, DESMatrix.MPcomp);
        }
        for (int i = 0; i < 16; i++)
        {

            LR[0] = f(LR[1], roundKeys[i]).Xor(LR[0]);

            (LR[0], LR[1]) = (LR[1], LR[0]);
            if(isLog) Console.WriteLine(LR[0].ToString("X") + "  " + LR[1].ToString("X") + "  " + roundKeys[i].ToString("X"));
        }
        (LR[0], LR[1]) = (LR[1], LR[0]);
        data = BitArrayExtension.JoinBitArrays(LR, 32);
        data = ApplyP(data, DESMatrix.MIP_1);
        return data;
    }

    public static BitArray Decrypt(BitArray cipher, BitArray key)
    {
        return Encrypt(cipher, key, 1);
    }

    public static BitArray Encrypt(BitArray data, DESKey key)
    {
        return Encrypt(data, key.key64);
    }

    public static BitArray Decrypt(BitArray data, DESKey key)
    {
        return Encrypt(data, key.key64, 1);
    }

    public static string Encrypt(string data, DESKey key)
    {
        return Encrypt(BitArrayExtension.GetFromString(data,"S"), key.key64).ToString("S");
    }

    public static string Encrypt_ECB(string data, DESKey key)
    {
        if(data.Length%8!=0) for(; data.Length % 8 != 0;)  data += " ";
        BitArray bdata = BitArrayExtension.GetFromString(data, "S");
        BitArray[] blocks = bdata.Separate(64);
        BitArray[] cryptBlocks = new BitArray[blocks.Length];
        for(int i=0; i < blocks.Length; i++)
        {
            cryptBlocks[i] = Encrypt(blocks[i], key);
        }
        return Convert.ToBase64String(BitArrayExtension.JoinBitArrays(cryptBlocks, 64).ToBytes());
    }

    //Режим сцепления блоков шифротекста с распространение ошибки
    public static string Encrypt_PCBC(string data, DESKey key, BitArray IV)
    {
        if (data.Length % 8 != 0) for (; data.Length % 8 != 0;) data += " ";
        BitArray bdata = BitArrayExtension.GetFromString(data, "S");
        BitArray[] blocks = bdata.Separate(64);
        BitArray[] cryptBlocks = new BitArray[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            BitArray block = (BitArray) blocks[i].Clone();
            cryptBlocks[i] = Encrypt(block.Xor(IV), key);
            IV = blocks[i].Xor(cryptBlocks[i]);
        }
        return Convert.ToBase64String(BitArrayExtension.JoinBitArrays(cryptBlocks, 64).ToBytes());
    }

    public static string Decrypt_PCBC(string data, DESKey key, BitArray IV)
    {
        byte[] bytes = Convert.FromBase64String(data);
        BitArray bdata = new BitArray(bytes);
        BitArray[] blocks = bdata.Separate(64);
        string crypt = "";
        for (int i = 0; i < blocks.Length; i++)
        {
            BitArray plain =  Decrypt(blocks[i], key).Xor(IV);
            IV = blocks[i].Xor(plain);
            crypt += plain.ToString("S");
        }
        return crypt;
    }

    public static string Decrypt_ECB(string data, DESKey key)
    {
        byte[] bytes = Convert.FromBase64String(data);
        BitArray bdata = new BitArray(bytes);
        BitArray[] blocks = bdata.Separate(64);
        string crypt = "";
        for (int i = 0; i < blocks.Length; i++)
        {
            crypt += Decrypt(blocks[i], key).ToString("S");
        }
        return crypt;
    }
}

