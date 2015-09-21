[![Build status](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/encryptmessage?branch=master)](https://ci.appveyor.com/api/projects/status/github/BizTalkComponents/encryptmessage/branch/master)

##Description
Encrypts a message using AES encryption.
The encryption key needs to base64 encoded. Sample code for generating a new, random key can be found [here](https://goo.gl/Cj9Iv1)

| Parameter          | Description                                                                             | Type | Validation                        |
| -------------------|-----------------------------------------------------------------------------------------|------|-----------------------------------|
|Encryption Key|The encryption key to use. Needs to be a valid AES encryption key and should be base64 encoded.|String|Required|
