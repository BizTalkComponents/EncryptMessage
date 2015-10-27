using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;

namespace BizTalkComponents.PipelineComponents.EncryptMessage
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("C9605FDF-FCCC-4246-BA74-4109006F308D")]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    public partial class EncryptMessage : IComponent, IBaseComponent,
                                        IPersistPropertyBag, IComponentUI
    {
        private const string EncryptionKeyPropertyName = "EncryptionKey";

      
        [RequiredRuntime]
        [DisplayName("Encryption key")]
        [Description("The key to use when encrypting. Should be in base64 format.")]
        public string EncryptionKey { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

           var encryptionKey = Convert.FromBase64String(EncryptionKey);

            var ms = new MemoryStream();

            using (var provider = new AesCryptoServiceProvider())
            {
                provider.Key = encryptionKey;
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;

                using (var encryptor = provider.CreateEncryptor(provider.Key, provider.IV))
                {
                    ms.Write(provider.IV, 0, 16);

                    var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    pInMsg.BodyPart.GetOriginalDataStream().CopyTo(cs);
                    cs.FlushFinalBlock();
                }
            }

            ms.Seek(0, SeekOrigin.Begin);
            ms.Position = 0;

            pInMsg.BodyPart.Data = ms;

            return pInMsg;

        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            EncryptionKey = PropertyBagHelper.ReadPropertyBag<string>(propertyBag, EncryptionKeyPropertyName);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, EncryptionKeyPropertyName, EncryptionKey);
        }
    }
}
