using pieskibackend;
using pieskibackend.Models.Dictionaries.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace tests;

public class PasswordTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void VerifyPassword_ShouldReturnTrue()
    {
        var hashString = "FC41F923D89E3DCC147EEC42F30004D3A8DC80B99A850C451C00BA5BE6C9DE9810A8F67B45873D6A07C854F39CC0C3A9CEB082968A795405509521E562DB4FAB";
        var saltString = "603A7D6397876A059D06506EB9EC978338507C6D906BEA08B111C42E019E770D88090A8F0D34C9FED59C9A3286346DC0894771A4C6F751B5F6257AFC38D9227D999CAE9FDE100C165A59AAECCA69A200D1AC697A0E81EEE03805CD1A0C9B080120C0A06068E2298C3ECE62AEEC7491094ED6A2A1341B5813FE87639EB9E9705D";

        var passwordHash = BigInteger.Parse(hashString, NumberStyles.HexNumber).ToByteArray();
        var passwordSalt = BigInteger.Parse(saltString, NumberStyles.HexNumber).ToByteArray();

        Array.Reverse(passwordHash, 0, passwordHash.Length);
        Array.Reverse(passwordSalt, 0, passwordSalt.Length);

        var password = "user";
        var result = ApiEndpoints.VerifyPasswordHash(password, passwordHash, passwordSalt);
        Assert.IsTrue(result);
    }
    [Test]
    public void VerifyPassword_ShouldReturnFalse()
    {
        var hashString = "AC41F923D89E3DCC147EEC42F30004D3A8DC80B99A850C451C00BA5BE6C9DE9810A8F67B45873D6A07C854F39CC0C3A9CEB082968A795405509521E562DB4FAB";
        var saltString = "003A7D6397876A059D06506EB9EC978338507C6D906BEA08B111C42E019E770D88090A8F0D34C9FED59C9A3286346DC0894771A4C6F751B5F6257AFC38D9227D999CAE9FDE100C165A59AAECCA69A200D1AC697A0E81EEE03805CD1A0C9B080120C0A06068E2298C3ECE62AEEC7491094ED6A2A1341B5813FE87639EB9E9705D";

        var passwordHash = BigInteger.Parse(hashString, NumberStyles.HexNumber).ToByteArray();
        var passwordSalt = BigInteger.Parse(saltString, NumberStyles.HexNumber).ToByteArray();

        Array.Reverse(passwordHash, 0, passwordHash.Length);
        Array.Reverse(passwordSalt, 0, passwordSalt.Length);

        var password = "user";
        var result = ApiEndpoints.VerifyPasswordHash(password, passwordHash, passwordSalt);
        Assert.IsFalse(result);
    }
}
