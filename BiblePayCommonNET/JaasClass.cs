using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BiblePayCommonNET
{

    public enum PKType
        {
            // PKCS#1 type key
            PKCS1,
            // PKCS#8 type key
            PKCS8
        }

        public class JAASProgram
        {
            /// Placeholder helper string.
            public const String BEGIN_PKCS1_PRIVATE_KEY = "-----BEGIN RSA PRIVATE KEY-----";
            /// Placeholder helper string.
            public const String END_PKCS1_PRIVATE_KEY = "-----END RSA PRIVATE KEY-----";

            /// Placeholder helper string.
            public const String BEGIN_PKCS8_PRIVATE_KEY = "-----BEGIN PRIVATE KEY-----";
            /// Placeholder helper string.
            public const String END_PKCS8_PRIVATE_KEY = "-----END PRIVATE KEY-----";

            /// <summary>
            /// JaaSJwtBuilder class that helps generate JaaS tokens.
            /// </summary>
            class JaaSJwtBuilder
            {

                /// <summary>
                /// To be used with exp value.
                /// The time after which the JWT expires.
                /// </summary>
                public const double EXP_TIME_DELAY_SEC = 7200;

                /// <summary>
                /// To be used with nbf value.
                /// The time before which the JWT must not be accepted for processing.
                /// </summary>
                public const double NBF_TIME_DELAY_SEC = 10;

                private readonly IDictionary<String, Object> userClaims = new Dictionary<String, Object>();
                private readonly IDictionary<String, Object> featureClaims = new Dictionary<String, Object>();
                private Dictionary<string,object> payload = new Dictionary<string, object>();

                //                private readonly JwtPayload payload = new JwtPayload();
                private String apiKey = String.Empty;
            //system identity model tokens jwt
                private JaaSJwtBuilder() { }

                /// <summary
                /// Creates a new JaaSJwtBuilder.
                /// <returns>
                /// A new builder that needs to be setup.
                /// </returns>
                /// </summary>
                public static JaaSJwtBuilder Builder()
                {
                    var jJB = new JaaSJwtBuilder();
                    jJB.payload.Add("aud", "jitsi");
                    jJB.payload.Add("iss", "chat");
                    return jJB;
                }

                /// <summary>
                /// Sets the value for the kid header claim. Represents the JaaS api key.
                /// You can find the api key here : https://jaas.8x8.vc/#/apikeys
                /// </summary>
                /// <param name="apiKey">Your API Key</param>
                /// <returns>
                /// A builder with kid claim set.
                /// </returns>
                public JaaSJwtBuilder WithApiKey(String apiKey)
                {
                    this.apiKey = apiKey;
                    return this;
                }

                /// <summary>
                /// Sets the value for the user avatar url as a string.
                /// </summary>
                /// <param name="url">Url for user avatar</param>
                /// <returns>
                /// A builder with avatar claim set.
                /// </returns>
                public JaaSJwtBuilder WithUserAvatar(String url)
                {
                    userClaims.Add("avatar", url);
                    return this;
                }

                /// <summary>
                /// Sets the value for the moderator claim.
                /// </summary>
                /// <param name="isModerator">
                /// If value is true user is moderator, false otherwise.
                /// </param>
                /// <returns>
                /// A builder with moderator claim set.
                /// </returns>
                public JaaSJwtBuilder WithModerator(bool isModerator)
                {
                    userClaims.Add("moderator", isModerator ? "true" : "false");
                    return this;
                }

                /// <summary>
                /// Sets the value for the user name to be displayed in the meeting.
                /// </summary>
                /// <param name="userName">
                /// User name to be displayed in meeting.
                /// </param>
                /// <returns>
                /// A builder with name claim set.
                /// </returns>
                public JaaSJwtBuilder WithUserName(String userName)
                {
                    userClaims.Add("name", userName);
                    return this;
                }

                /// <summary>
                /// Sets the value for the user email claim.
                /// </summary>
                /// <param name="userEmail">
                /// User email to be used in meeting.
                /// </param>
                /// <returns>
                /// A builder with email claim set.
                /// </returns>
                public JaaSJwtBuilder WithUserEmail(String userEmail)
                {
                    userClaims.Add("email", userEmail);
                    return this;
                }

                /// <summary>
                /// Sets the value for the live streaming feature claim.
                /// </summary>
                /// <param name="isEnabled">
                /// If value is true recording is enabled, false otherwise.
                /// </param>
                /// <returns>
                /// A builder with livestreaming claim set.
                /// </returns>
                public JaaSJwtBuilder WithLiveStreamingEnabled(bool isEnabled)
                {
                    featureClaims.Add("livestreaming", (isEnabled ? "true" : "false"));
                    return this;
                }

                /// <summary>
                /// Sets the value for the recording feature claim.
                /// </summary>
                /// <param name="isEnabled">
                /// If value is true recording is enabled, false otherwise.
                /// </param>
                /// <returns>
                /// A builder with recording claim set.
                /// </returns>
                public JaaSJwtBuilder WithRecordingEnabled(bool isEnabled)
                {
                    featureClaims.Add("recording", isEnabled ? "true" : "false");
                    return this;
                }

                /// <summary>
                /// Sets the value for the outbound-call feature claim.
                /// </summary>
                /// <param name="isEnabled">
                /// If value is true outbound calls are enabled, false otherwise.
                /// </param>
                /// <returns>
                /// A builder with outbound-call claim set.
                /// </returns>
                public JaaSJwtBuilder WithOutboundCallEnabled(bool isEnabled)
                {
                    featureClaims.Add("outbound-call", (isEnabled ? "true" : "false"));
                    return this;
                }

                /// <summary>
                /// Sets the value for the transcription feature claim.
                /// </summary>
                /// <param name="isEnabled">
                /// If value is true transcription is enabled, false otherwise.
                /// </param>
                /// <returns>
                /// A builder with transcription claim set.
                /// </returns>
                public JaaSJwtBuilder WithTranscriptionEnabled(bool isEnabled)
                {
                    featureClaims.Add("transcription", (isEnabled ? "true" : "false"));
                    return this;
                }

                /// <summary>
                /// Sets the value for the exp claim representing the time until the token expires.
                /// You don't have to change this value too much, use defaults.
                /// </summary>
                /// <param name="expTime"></param>
                /// <returns>
                /// A builder with exp claim set.
                /// </returns>
                public JaaSJwtBuilder WithExpTime(DateTime expTime)
                {
                    payload.Add("exp", new DateTimeOffset(expTime).ToUnixTimeSeconds());
                    return this;
                }

                /// <summary>
                /// Sets the value for the nbf claim.
                /// You don't have to change this value too much, use defaults.
                /// </summary>
                /// <param name="nbfTime"></param>
                /// <returns>
                /// A builder with nbf claim set.
                /// </returns>
                public JaaSJwtBuilder WithNbfTime(DateTime nbfTime)
                {
                    payload.Add("nbf", new DateTimeOffset(nbfTime).ToUnixTimeSeconds());
                    return this;
                }

                /// <summary>
                /// Sets the value for the room name.
                /// This field supports also wildcard("*") if the token is issued for all rooms.
                /// </summary>
                /// <param name="roomName">
                /// The meeting room value for which the token is issued.
                /// </param>
                /// <returns>
                /// A builder with room claim set.
                /// </returns>
                public JaaSJwtBuilder WithRoomName(String roomName)
                {
                    payload.Add("room", roomName);
                    return this;
                }

                /// <summary>
                /// Sets the value for the sub claim representing the AppID ( previously tenant name/unique identifier).
                /// </summary>
                /// <param name="appId">The AppID that identifies your application.</param>
                /// <returns></returns>
                public JaaSJwtBuilder WithAppID(String appId)
                {
                    payload.Add("sub", appId);
                    return this;
                }

                /// <summary>
                /// Sets the value for the id claim.
                /// </summary>
                /// <param name="id">
                /// The user's unique identifier.
                /// </param>
                /// <returns>
                /// A builder with kid claim set.
                /// </returns>
                public JaaSJwtBuilder WithUserId(String id)
                {
                    userClaims.Add("id", id);
                    return this;
                }

                /// <summary>
                /// Fills the default values for required claims.
                /// </summary>
                /// <returns>
                /// A builder with needed claim set to default values.
                /// </returns>
                public JaaSJwtBuilder WithDefaults()
                {
                    return WithExpTime(DateTime.UtcNow.AddSeconds(EXP_TIME_DELAY_SEC))
                        .WithNbfTime(DateTime.UtcNow.AddSeconds(-NBF_TIME_DELAY_SEC))
                        .WithLiveStreamingEnabled(true)
                        .WithRecordingEnabled(true)
                        .WithOutboundCallEnabled(true)
                        .WithTranscriptionEnabled(true)
                        .WithModerator(true)
                        .WithRoomName("*") ;
                }

                /// Generate a signed JaaS JWT token string.
                /// <param name="privateKey">The private key used to sign the JWT.</param>
                /// <returns></returns>
                public String SignWith(RSA privateKey)
                {
                    var context = new Dictionary<String, Object>();
                    context.Add("user", userClaims);
                    context.Add("features", featureClaims);
                    payload.Add("context", context);
                    var extraHeaders = new Dictionary<String, Object>();
                    extraHeaders.Add("kid", this.apiKey);
                    extraHeaders.Add("typ", "JWT");
                    // var cred = new Microsoft.IdentityModel.Tokens.SigningCredentials(jwk, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256);
                    // var secToken = new  JwtSecurityToken(new JwtHeader(cred), this.payload);
                    // return jwtHandler.WriteToken(secToken);
                    // https://superdry.apphb.com/tools/online-rsa-key-converter
                    string token3 = Jose.JWT.Encode(payload, privateKey, Jose.JwsAlgorithm.RS256,extraHeaders);
                    return token3;
                }
            }

            /// <param name="privateKeyFilePath"></param>
            /// <param name="pkType">The type of key from the file: PKCS#1 or PKCS#8</param>
            /// <returns>The private key object</returns>
            static RSA ReadPrivateKeyFromFile(String privateKeyFilePath, PKType pkType)
            {
                var rsa = RSA.Create();
                var privateKeyContent = String.Empty;
                privateKeyContent = privateKeyContent.Replace(pkType == PKType.PKCS1 ? JAASProgram.BEGIN_PKCS1_PRIVATE_KEY : JAASProgram.BEGIN_PKCS8_PRIVATE_KEY, "");
                privateKeyContent = privateKeyContent.Replace(pkType == PKType.PKCS1 ? JAASProgram.END_PKCS1_PRIVATE_KEY : JAASProgram.END_PKCS8_PRIVATE_KEY, "");
                var privateKeyDecoded = Convert.FromBase64String(privateKeyContent);
                string converted = Encoding.UTF8.GetString(privateKeyDecoded, 0, privateKeyDecoded.Length);
                //string privateKey = BiblePayCommon.Encryption.CommonConfig("jitsiprivkey");
                string privateKey = System.IO.File.ReadAllText("c:\\inetpub\\wwwroot\\jitsi.privkey");
                rsa.FromXmlString(privateKey);
                return rsa;
            }

            public static string GenerateJaasToken(string sFullUserName, string UserID, string EmailAddress, string AvatarURL, string sMyAppID, string sMyApiKey)
            {
                try
                {
                    var rsaPrivateKey = ReadPrivateKeyFromFile("dummyfile_nolongerused.pk", PKType.PKCS8);
                    var token = JaaSJwtBuilder.Builder()
                                        .WithDefaults()
                                        .WithApiKey(sMyApiKey)
                                        .WithUserName(sFullUserName)
                                        .WithUserEmail(EmailAddress)
                                        .WithUserAvatar(AvatarURL)
                                        .WithAppID(sMyAppID)
                                        .WithUserId(UserID)
                                        .SignWith(rsaPrivateKey);
                    /// Write JaaS JWT to standard output.
                    Console.Write(token);
                    return token;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return String.Empty;
            }


            static void Main(string[] args)
            {
                try
                {
                    /// Read private key from file.
                    var rsaPrivateKey = ReadPrivateKeyFromFile("deprecated.pk", PKType.PKCS8);
                    /// Create new JaaSJwtBuilder and setup the claims and sign using the private key.
                    var token = JaaSJwtBuilder.Builder()
                                        .WithDefaults()
                                        .WithApiKey("my api key")
                                        .WithUserName("my user name")
                                        .WithUserEmail("my user email")
                                        .WithUserAvatar("https://avatarurl.com/avatar/url")
                                        .WithAppID("my AppID")
                                        .SignWith(rsaPrivateKey);

                    /// Write JaaS JWT to standard output.
                    Console.Write(token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

