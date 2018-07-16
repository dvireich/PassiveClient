using Authentication;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics;
using System;
using System.ServiceModel;

namespace PassiveClient
{
    [Log(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Private | MulticastAttributes.Public)]
    public class AuthenticationClient : CommunicationClient
    {
        protected string _username = string.Empty;
        protected string _password = string.Empty;

        private const string userType = "PassiveClient";

        public bool Authenticate(string userName, string password, out string error, out string result)
        {
            var auth = (IAuthentication)initializeServiceReferences<IAuthentication>("Authentication");
            var resp = auth.AuthenticateAndSignIn(new AuthenticateAndSignInRequest()
            {
                userName = userName,
                password = password,
                userType = userType
            });
            if (auth != null)
            {
                ((ICommunicationObject)auth).Close();
            }
            error = resp.error;
            result = resp.AuthenticateAndSignInResult;
            return !string.IsNullOrEmpty(resp.AuthenticateAndSignInResult);
        }

        public bool Logout(string userName, out string error)
        {
            try
            {
                var auth = (IAuthentication)initializeServiceReferences<IAuthentication>("Authentication");
                var resp = auth.Logout(new LogoutRequest()
                {
                    userName = userName,
                    userType = userType
                });
                if (auth != null)
                {
                    ((ICommunicationObject)auth).Close();
                }
                error = resp.error;

                return resp.LogoutResult;
            }
            catch (Exception)
            {
                error = $"Could not logout for user name: {userName} and user type: {userType}";
                return false;
            }

        }
    }
}
