﻿namespace GoldJewelry.IO.Contracts
{
    public interface IWriter 
    {
        void WriteLine(string value);

        void Write(string value);
    }
}
