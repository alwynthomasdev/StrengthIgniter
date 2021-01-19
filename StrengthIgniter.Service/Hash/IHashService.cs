using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Hash
{
    public interface IHashService
    {
        string Generate(string plainText);
        bool Validate(string plainText, string hashText);
        string GenerateFakeHash();
    }
}
