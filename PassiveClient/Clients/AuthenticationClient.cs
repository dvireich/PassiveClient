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
            var resp = authentication.AuthenticateAndSignIn(new AuthenticateAndSignInRequest()
            {
                userName = userName,
                password = password,
                userType = userType
            });
            
            error = resp.error;
            result = resp.AuthenticateAndSignInResult;
            return !string.IsNullOrEmpty(resp.AuthenticateAndSignInResult);
        }

        public bool Logout(string userName, out string error)
        {
            try
            {
                var resp = authentication.Logout(new LogoutRequest()
                {
                    userName = userName,
                    userType = userType
                });
                error = resp.error;
                return resp.LogoutResult;
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
