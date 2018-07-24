using Authentication;
using PassiveShell;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.ServiceModel;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class AuthenticationClient : CommunicationClient, IDisposable
    {
        IAuthentication authentication;

        protected string _username = string.Empty;
        protected string _password = string.Empty;

        public AuthenticationClient(IPassiveShell passiveShell,
                                    IAuthentication authentication,
                                    Guid id) : base(passiveShell,
                                                    id)
        {
            this.authentication = authentication;
        }

        public AuthenticationClient()
        {
            authentication = (IAuthentication)InitializeServiceReferences<IAuthentication>("Authentication");
        }

        private const string userType = "PassiveClient";

        public bool Authenticate(string userName, string password, out string error, out string result)
        {
            var resp = authentication.AuthenticatePassiveClientAndSignIn(new AuthenticatePassiveClientAndSignInRequest()
            {
                userName = userName,
                password  = password
            });
            
            error = resp.error;
            result = resp.AuthenticatePassiveClientAndSignInResult;
            return !string.IsNullOrEmpty(resp.AuthenticatePassiveClientAndSignInResult);
        }

        public bool Logout(string userName, out string error)
        {
            try
            {
                var resp = authentication.PassiveLogout(new PassiveLogoutRequest()
                {
                    userName = userName,
                    userType = userType
                });
                error = resp.error;
                return resp.PassiveLogoutResult;
            }
            catch (Exception)
            {
                error = $"Could not logout for user name: {userName} and user type: {userType}";
                return false;
            }
        }

        public void Dispose()
        {
            try
            {
                if (authentication != null)
                {
                    ((ICommunicationObject)authentication).Close();
                }
            }
            catch { }
        }
    }
}
