using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreFeed.Core.Com.Entity.App;
using CoreFeed.Core.KeyVault;
using CoreFeed.Core.Telemetry;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Claims;
//using System.Threading;
using System.Threading.Tasks;
using CoreFeed.Core.WorkFlowBuilderPackage.Funnel;

[assembly: SuppressIldasmAttribute()]

namespace CoreFeed.Core.Auth
{
    public class Auth : IAuth
    {
        private static string AuthByPass;
        private readonly IKeyVault _keyVault;
        private readonly string Secret;
        private readonly int SecretExpireBy;
        private readonly int funnelData = 1;

        /// <summary>
        /// Set by "DebugMode" = "True" in configuration
        /// </summary>
        private bool isDebugMode = false;

        /// <summary>
        /// Set by "isHiddenValueSet" = "yes" in configuration
        /// </summary>
        private bool isHiddenValueSet = false;

        private string envValue = "local";
        private bool isProdVault = false;

        private List<string> ipsAllowedToAccessLocal = new List<string>();

        public Auth(IKeyVault keyVault, ITelemetry telemetry, IClaimDataExtractor claimDataExtractor, IServerRoleAuthorization serverRoleAuthorization)
        {
            _keyVault = keyVault;
            Telemetry = telemetry;
            ClaimDataExtractor = claimDataExtractor;
            ServerRoleAuthorization = serverRoleAuthorization;
            AuthByPass = _keyVault.GetValue("AuthByPass");
            Secret = _keyVault.GetValue("AuthSecret");
            funnelData = 0;
            SecretExpireBy = Convert.ToInt32(_keyVault.GetValue("AuthTimeout"));

            var isDebugModePrefix = ConfigurationManager.AppSettings["DebugMode"] ?? "";
            if (isDebugModePrefix == "debug") 
            {
                isDebugMode = true;
            }

            var isHiddenValueSetPrefix = ConfigurationManager.AppSettings["isHiddenValueSet"] ?? "";
            if (isHiddenValueSetPrefix == "yes")
            {
                isHiddenValueSet = true;
            }

            envValue = (ConfigurationManager.AppSettings["Env"] ?? "").ToLower();

            isProdVault = (ConfigurationManager.AppSettings["KeyName"] ?? "").ToLower().Equals("pvintegrated-prodkeys");

            //this for allowing only local ips : to be done later

            var keyValue = keyVault.GetValue("PV_Keys");
            var result = JObject.Parse(keyValue)["LocalIpAllowed"].ToString();
            ipsAllowedToAccessLocal = JsonConvert.DeserializeObject<string[]>(result).ToList();
        }

        public ITelemetry Telemetry { get; }
        public IClaimDataExtractor ClaimDataExtractor { get; }
        public IServerRoleAuthorization ServerRoleAuthorization { get; }

        public Response AuthInjector(Response action, Request request)
        {
            var response = action;

            if ((request.RequestType == "UserLogin")
                    && (request.RequestSubType == "Login"))
            {
                var res = response as dynamic;
             
                 if (res != null && res.Success)
               // if (res != null )
                {
                    string userEmail = res.UserClaim.Email;

                    string dpftDataString = res.UserClaim.DPFT; // Data to be Provided in IClaimDataExtractor called 'D'ata'P'rovider'For''T'oken: DPFT
                    var dataToBeFilled = this.ClaimDataExtractor.ExtractData(dpftDataString);

                    var token = GenerateToken(userEmail, dataToBeFilled);
                    res.UserClaim.AuthToken = token;

                    return res;
                }
            }

            return response;
        }

        public (bool Result, Lazy<Dictionary<string, string>> DPFTLazy) AuthValidator(HttpRequestMessage requestMessage, Request request)
        {
            try
            {
                var requestType = request.RequestType;
                var requestSubType = request.RequestSubType;
                var AuthByPassArray = JsonConvert.DeserializeObject<List<AuthByPass>>(AuthByPass);

                if (AuthByPassArray.Any(a => a.Request == requestType && (a.RequestSubType == requestSubType)))
                {
                    return (true, null);
                }

                if (request.RequestSubType == "SignInByAssociation")
                {
                    CheckCrossDomainAuth(requestMessage);

                    return (true, null);
                }

                var token = requestMessage.Headers.GetValues("Authorization").FirstOrDefault();

                var simplePrinciple = GetPrincipal(token);
                
                var identity = simplePrinciple.Identity as ClaimsIdentity;

                if (identity == null)
                    throw new AuthenticationException("User is not authorized.");

                if (!identity.IsAuthenticated)
                    throw new AuthenticationException("User is not authorized.");

                var usernameClaim = identity.FindFirst(ClaimTypes.Name);
                var username = usernameClaim?.Value;

                if (string.IsNullOrEmpty(username))
                    throw new AuthenticationException("User is not authorized.");

                var DPFTLazyData = new Lazy<Dictionary<string, string>>(() =>
                {
                    Dictionary<string, string> claimData = new Dictionary<string, string>();
                    try
                    {
                        identity.Claims.ToList().ForEach(p =>
                        {
                            if (!claimData.ContainsKey(p.Type))
                            {
                                claimData.Add(p.Type, p.Value);
                            }
                        });
                    }
                    catch
                    {
                        //silent
                    }

                    return claimData;
                });

                return (true, DPFTLazyData);
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex,
                    new Dictionary<string, string> { { "Auth", "AuthCore.AuthValidator" }, { "CopyRight@2019", "Contact MS for Help here if this is not the expected result." } }, request);
                throw new AuthenticationException("User is not authorized.");
            }
        }

        public async Task<Response> Execute(HttpRequestMessage request, ITelemetry telemetry, dynamic obj, Func<string, dynamic> handlerDy, Func<Func<string, object>> wf)
        {
            var defaultResponse = new Response();
            var handlerRequest = JsonConvert.DeserializeObject<Request>(await request.Content.ReadAsStringAsync());

            try
            {
                return await telemetry.TrackV2(async () =>
                {
                    if (isDebugMode)
                    {
                        defaultResponse.Errors.Add(new Error { Code = "DebugError:funnelDataCode", Message = funnelData.ToString() });
                        defaultResponse.Errors.Add(new Error { Code = "DebugError:RequestType", Message = handlerRequest?.RequestType ?? "" });
                    }


                    // we should not allow users to debug production or dev env, only exception when isHiddenValueSet = true && hitting local url

#if DEBUG


                    if(!isHiddenValueSet)
                    {
                        if (request.RequestUri.Host.ToLower().Contains("localhost"))
                        {
                            // Don t allow when prod key vault
                            if (isProdVault)
                            {
                                throw new AuthenticationException("User is not authorized.");
                            }

                            // Don t allow when prod env
                            if (envValue.Contains("prod"))
                            {
                                throw new AuthenticationException("User is not authorized.");
                            }

                            // Don't allow when env is different then dev and prod
                            if (!envValue.Contains("dev") && !envValue.Contains("prod"))
                            {
                                throw new AuthenticationException("User is not authorized.");
                            }                

                        }
                    }
#endif
                    var requestType = handlerRequest.RequestType;

                    var authResult = AuthValidator(request, handlerRequest);

                    if (string.IsNullOrEmpty(requestType))
                        return new Response() { Errors = new List<Error>() { new Error { Message = "Request not mapped." } } };

                    if (requestType == "RefreshToken")
                    {
                        return RefreshAuth(request, handlerRequest, obj);
                    }
                    else
                    { 
                        var handler = handlerDy.Invoke(requestType);

                        if (handler == null)
                            return new Response() { Errors = new List<Error>() { new Error { Message = "Request's handler not mapped." } } };

                        if (string.IsNullOrEmpty(handlerRequest.ExecutionContext?.TrackingId))
                        {
                            if (handlerRequest.ExecutionContext == null)
                            {
                                handlerRequest.ExecutionContext = new ExecutionContext();
                            }
                            handlerRequest.ExecutionContext.TrackingId = Guid.NewGuid().ToString();
                        }

                        if (handlerRequest?.ExecutionContext != null)
                        {
                            handlerRequest.ExecutionContext.DPFT = authResult.DPFTLazy;
                        }

                        //   handlerRequest.DTO = (dTo) => registration.Resolve<IWorkFlowDataProvider>(dTo);
                        //Limitation : DTO object only supported in WorkFlow
                        if (requestType == "WorkFlowRequest")
                        {
                            handlerRequest.DTO = wf.Invoke();
                        }

                        // Auth based on roles when user is already authorized.
                        //For MS token purpose, we will not call this if token is not valid to avoid back tracking in case then will write telemetry to check why calls are delay.
                        if (funnelData == 1)
                        {
                            ServerRoleAuthorization.IsActionAuthorizedByServer(handlerRequest);
                        }
                        
                        var resWithAuth = await handler.Execute(handlerRequest);

                        resWithAuth.PageCriteria = handlerRequest.PageCriteria;
                        resWithAuth.FilterByCriteria = handlerRequest.FilterByCriteria;

                        return AuthInjector(resWithAuth, handlerRequest);
                    }
                }, handlerRequest.RequestType, handlerRequest, TelemetryArgs.InvokerInstance);
            }
            catch (AuthenticationException ex)
            {             
                defaultResponse.Errors.Add(new Error { Code = HttpStatusCode.Unauthorized.ToString(), Message = ex.Message });
                if (isDebugMode)
                {
                    defaultResponse.Errors.Add(new Error { Code = "DebugError", Message = JsonConvert.SerializeObject(ex) });
                }
                return defaultResponse;
            }
            catch (Exception ex)
            {
                defaultResponse.Errors.Add(new Error { Code = "InternalServerError", Message = ex.Message });
                if (isDebugMode)
                {
                    defaultResponse.Errors.Add(new Error { Code = "DebugError", Message = JsonConvert.SerializeObject(ex) });
                }
                return defaultResponse;
            }
        }

        public Response RefreshAuth(HttpRequestMessage requestMessage, Request request, dynamic obj)
        {
            //Consider token is already checked.
            try
            {
                var token = requestMessage.Headers.GetValues("Authorization").FirstOrDefault();
                var simplePrinciple = GetPrincipal(token);
                var identity = simplePrinciple.Identity as ClaimsIdentity;

                //var usernameClaim = identity.FindFirst(ClaimTypes.Name);

                //var obj2 = new Response();
                //obj2.Header = GenerateToken(usernameClaim.Value);

                //Dynamic does not support in confused cases
                obj.Token = GenerateToken("", null, identity);

                return obj;

                //UserLoginResponse id dynamic
                //return new UserLoginResponse()
                //{
                //    Token = GenerateToken(usernameClaim.Value)
                //};
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex,
                    new Dictionary<string, string> { { "Auth", "AuthCore.RefreshAuth" }, { "CopyRight@2019", "Contact MS for Help here if this is not the expected result." } }, request);
                throw new AuthenticationException("User is not authorized.");
            }
        }

        private void CheckCrossDomainAuth(HttpRequestMessage request)
        {
            //To be add
        }

        private string GenerateToken(string userName, Dictionary<string, string> tokenData, ClaimsIdentity claimsIdentity = null)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            if (claimsIdentity == null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, userName),
                    new Claim("Sytem", "PropVivo")
                };

                tokenData?.ToList().ForEach(dt =>
                {
                    claims.Add(new Claim(dt.Key, dt.Value));
                });

                claimsIdentity = new ClaimsIdentity(claims);
            }
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                
                Expires = now.AddMinutes(SecretExpireBy),
                
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(symmetricKey),
                     SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    internal class AuthByPass
    {
        [JsonProperty("Reason")]
        public string Reason { get; set; }

        [JsonProperty("Request")]
        public string Request { get; set; }

        [JsonProperty("RequestSubType")]
        public string RequestSubType { get; set; }
    }
}