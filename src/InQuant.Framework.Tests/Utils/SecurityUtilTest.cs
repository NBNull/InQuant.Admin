using InQuant.Framework.Utils;
using System;
using Xunit;

namespace InQuant.Framework.Tests.Utils
{
    public class SecurityUtilTest
    {
        private readonly byte[] _key = new byte[32] { 0, 152, 223, 225, 20, 100, 30, 16, 181, 62, 120, 81, 221, 241, 222, 43, 60, 197, 160, 47, 209, 223, 145, 244, 90, 180, 242, 236, 0, 193, 98, 10 };
        private readonly byte[] _iv = new byte[16] { 132, 214, 162, 127, 217, 77, 254, 22, 144, 190, 20, 60, 48, 60, 172, 206 };

        [InlineData("hello world. 123 !@#@})_.ppp中国", "Durg1IwN+waeDF79XZstn5f7mErscnKesX9UOtz1QecjAG0QHxikTb2lQqBHSLy1")]
        [Theory]
        public async void Encrypt(string plainText, string cipherText)
        {
            var real = await SecurityUtil.Encrypt(plainText, _key, _iv);

            Assert.Equal(cipherText, real);
        }

        [InlineData("Durg1IwN+waeDF79XZstn5f7mErscnKesX9UOtz1QecjAG0QHxikTb2lQqBHSLy1", "hello world. 123 !@#@})_.ppp中国")]
        [Theory]
        public async void Decrypt(string cipherText, string plainText)
        {
            var real = await SecurityUtil.Decrypt(cipherText, _key, _iv);

            Assert.Equal(plainText, real);
        }

        [InlineData("31A5F7E976911D0308023BE4E96A0EE1C117276A1F4A161DEBE6D2B47BE16956E44BC4C63F4CCAC74E4417477DD1A9A9864BC47C8DBC182B73B0C373A69FA7F2", "{\"status\":\"-1\",\"message\":\"无法获取post参数\",\"data\":{}}")]
        [Theory]
        public async void DESDecrypt(string cipherText, string plainText)
        {
            var real = await SecurityUtil.DESDecrypt(cipherText, "shenzhen12345678shenzhen", "12312300");
            Console.WriteLine(real);
            Assert.Equal(plainText, real);
        }

        [InlineData("{\"status\":\"-1\",\"message\":\"无法获取post参数\",\"data\":{}}", "31A5F7E976911D0308023BE4E96A0EE1C117276A1F4A161DEBE6D2B47BE16956E44BC4C63F4CCAC74E4417477DD1A9A9864BC47C8DBC182B73B0C373A69FA7F2")]
        [Theory]
        public async void DESEncrypt(string plainText, string cipherText)
        {
            var real = await SecurityUtil.DESEncrypt(plainText, "shenzhen12345678shenzhen", "12312300");
            Console.WriteLine(real);
            Assert.Equal(cipherText, real);
        }
    }
}
