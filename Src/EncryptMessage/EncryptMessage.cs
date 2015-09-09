using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using BizTalkComponents.Utils;
using BizTalkComponents.Utils.LookupUtils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;

namespace BizTalkComponents.PipelineComponents.EncryptMessage
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("C9605FDF-FCCC-4246-BA74-4109006F308D")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public partial class EncryptMessage : IComponent, IBaseComponent,
                                        IPersistPropertyBag, IComponentUI
    {
        private readonly ISSOConfigRepository _repository;

        public EncryptMessage()
        {
            _repository = new SSOConfigRepository();
        }

        public EncryptMessage(ISSOConfigRepository repository)
        {
            _repository = repository;
        }

        private const string SSOConfigApplicationPropertyName = "SSOConfigApplicationProperty";
        private const string SSOConfigKeyPropertyName = "SSOConfigKey";

        [RequiredRuntime]
        [DisplayName("SSO Config Application")]
        [Description("The name of the SSO Application to read the encryption key from.")]
        public string SSOConfigApplication { get; set; }

        [RequiredRuntime]
        [DisplayName("SSO Config Key")]
        [Description("The key of the SSO configuraiton property to read the encryption key from.")]
        public string SSOConfigKey { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var sr = new SSOConfigReader(_repository);
            var encryptionKeyStr = sr.GetConfigValue(SSOConfigApplication, SSOConfigKey);
            if (string.IsNullOrEmpty(encryptionKeyStr))
            {
                throw new ArgumentException("No encryption key could be found at the specified sso config.");
            }

            var encryptionKey = Convert.FromBase64String(encryptionKeyStr);

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
            SSOConfigApplication = PropertyBagHelper.ReadPropertyBag<string>(propertyBag, SSOConfigApplicationPropertyName);
            SSOConfigKey = PropertyBagHelper.ReadPropertyBag<string>(propertyBag, SSOConfigKeyPropertyName);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, SSOConfigApplicationPropertyName, SSOConfigApplication);
            PropertyBagHelper.WritePropertyBag(propertyBag, SSOConfigKeyPropertyName, SSOConfigKey);
        }
    }
}
