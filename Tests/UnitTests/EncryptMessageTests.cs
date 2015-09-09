using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BizTalkComponents.Utils.LookupUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Winterdom.BizTalk.PipelineTesting;

namespace BizTalkComponents.PipelineComponents.EncryptMessage.Tests.UnitTests
{
    [TestClass]
    public class EncryptMessageTests
    {
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        [TestMethod]
        public void TestMethod1()
        {
            //var key = new byte[32];
            //new RNGCryptoServiceProvider().GetBytes(key);

            var keyStr = "AAECAwQFBgcICQoLDA0ODw==";

            var mock = new Mock<ISSOConfigRepository>();
            mock.Setup(r => r.Read("SSOApplication", "SSOKey")).Returns(keyStr);

            var pipeline = PipelineFactory.CreateEmptySendPipeline();

            var em = new EncryptMessage(mock.Object)
            {
                SSOConfigApplication = "SSOApplication",
                SSOConfigKey = "SSOKey"
            };

            pipeline.AddComponent(em, PipelineStage.Encode);
            var message = MessageHelper.Create("<test></test>");

            var output = pipeline.Execute(message);
            var result = DecryptMessage(output.BodyPart.Data, keyStr);
            string re;
            using (var sr = new StreamReader(result))
            {
                re = sr.ReadToEnd();
            }

            re = re.Replace("\0", "");
            Assert.AreEqual("<test></test>",re);
        }

        private Stream DecryptMessage(Stream outputStream, string keyString)
        {
            var key = Convert.FromBase64String(keyString);
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = key;
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                var ms = new MemoryStream();
                outputStream.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 0;
                byte[] iv = new byte[16];
                ms.Read(iv, 0, 16);

                provider.IV = iv;
                Trace.WriteLine("IV decrypt " + provider.IV);
                var decryptor = provider.CreateDecryptor(provider.Key, provider.IV);

                return new CryptoStream(ms, decryptor, CryptoStreamMode.Read);


            }
        }
    }
}
