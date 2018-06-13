﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PassiveClient.Authentication {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Authentication.IAuthentication")]
    public interface IAuthentication {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/Authenticate", ReplyAction="http://tempuri.org/IAuthentication/AuthenticateResponse")]
        PassiveClient.Authentication.AuthenticateResponse Authenticate(PassiveClient.Authentication.AuthenticateRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/Authenticate", ReplyAction="http://tempuri.org/IAuthentication/AuthenticateResponse")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.AuthenticateResponse> AuthenticateAsync(PassiveClient.Authentication.AuthenticateRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/SignUp", ReplyAction="http://tempuri.org/IAuthentication/SignUpResponse")]
        PassiveClient.Authentication.SignUpResponse SignUp(PassiveClient.Authentication.SignUpRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/SignUp", ReplyAction="http://tempuri.org/IAuthentication/SignUpResponse")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.SignUpResponse> SignUpAsync(PassiveClient.Authentication.SignUpRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/ChangeUserPassword", ReplyAction="http://tempuri.org/IAuthentication/ChangeUserPasswordResponse")]
        PassiveClient.Authentication.ChangeUserPasswordResponse ChangeUserPassword(PassiveClient.Authentication.ChangeUserPasswordRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/ChangeUserPassword", ReplyAction="http://tempuri.org/IAuthentication/ChangeUserPasswordResponse")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.ChangeUserPasswordResponse> ChangeUserPasswordAsync(PassiveClient.Authentication.ChangeUserPasswordRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/SetSecurityQuestionAndAnswer", ReplyAction="http://tempuri.org/IAuthentication/SetSecurityQuestionAndAnswerResponse")]
        PassiveClient.Authentication.SetSecurityQuestionAndAnswerResponse SetSecurityQuestionAndAnswer(PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/SetSecurityQuestionAndAnswer", ReplyAction="http://tempuri.org/IAuthentication/SetSecurityQuestionAndAnswerResponse")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.SetSecurityQuestionAndAnswerResponse> SetSecurityQuestionAndAnswerAsync(PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/GetSecurityQuestion", ReplyAction="http://tempuri.org/IAuthentication/GetSecurityQuestionResponse")]
        PassiveClient.Authentication.GetSecurityQuestionResponse GetSecurityQuestion(PassiveClient.Authentication.GetSecurityQuestionRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/GetSecurityQuestion", ReplyAction="http://tempuri.org/IAuthentication/GetSecurityQuestionResponse")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.GetSecurityQuestionResponse> GetSecurityQuestionAsync(PassiveClient.Authentication.GetSecurityQuestionRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/RestorePasswordFromUserNameAndSecurityQuestion" +
            "", ReplyAction="http://tempuri.org/IAuthentication/RestorePasswordFromUserNameAndSecurityQuestion" +
            "Response")]
        PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionResponse RestorePasswordFromUserNameAndSecurityQuestion(PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAuthentication/RestorePasswordFromUserNameAndSecurityQuestion" +
            "", ReplyAction="http://tempuri.org/IAuthentication/RestorePasswordFromUserNameAndSecurityQuestion" +
            "Response")]
        System.Threading.Tasks.Task<PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionResponse> RestorePasswordFromUserNameAndSecurityQuestionAsync(PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Authenticate", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class AuthenticateRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string password;
        
        public AuthenticateRequest() {
        }
        
        public AuthenticateRequest(string userName, string password) {
            this.userName = userName;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="AuthenticateResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class AuthenticateResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string AuthenticateResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public AuthenticateResponse() {
        }
        
        public AuthenticateResponse(string AuthenticateResult, string error) {
            this.AuthenticateResult = AuthenticateResult;
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SignUp", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SignUpRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string password;
        
        public SignUpRequest() {
        }
        
        public SignUpRequest(string userName, string password) {
            this.userName = userName;
            this.password = password;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SignUpResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SignUpResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool SignUpResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public SignUpResponse() {
        }
        
        public SignUpResponse(bool SignUpResult, string error) {
            this.SignUpResult = SignUpResult;
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ChangeUserPassword", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ChangeUserPasswordRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string oldPassword;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string newPassword;
        
        public ChangeUserPasswordRequest() {
        }
        
        public ChangeUserPasswordRequest(string userName, string oldPassword, string newPassword) {
            this.userName = userName;
            this.oldPassword = oldPassword;
            this.newPassword = newPassword;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ChangeUserPasswordResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class ChangeUserPasswordResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool ChangeUserPasswordResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public ChangeUserPasswordResponse() {
        }
        
        public ChangeUserPasswordResponse(bool ChangeUserPasswordResult, string error) {
            this.ChangeUserPasswordResult = ChangeUserPasswordResult;
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SetSecurityQuestionAndAnswer", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SetSecurityQuestionAndAnswerRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string password;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string question;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=3)]
        public string answer;
        
        public SetSecurityQuestionAndAnswerRequest() {
        }
        
        public SetSecurityQuestionAndAnswerRequest(string userName, string password, string question, string answer) {
            this.userName = userName;
            this.password = password;
            this.question = question;
            this.answer = answer;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="SetSecurityQuestionAndAnswerResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class SetSecurityQuestionAndAnswerResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool SetSecurityQuestionAndAnswerResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public SetSecurityQuestionAndAnswerResponse() {
        }
        
        public SetSecurityQuestionAndAnswerResponse(bool SetSecurityQuestionAndAnswerResult, string error) {
            this.SetSecurityQuestionAndAnswerResult = SetSecurityQuestionAndAnswerResult;
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetSecurityQuestion", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetSecurityQuestionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        public GetSecurityQuestionRequest() {
        }
        
        public GetSecurityQuestionRequest(string userName) {
            this.userName = userName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetSecurityQuestionResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class GetSecurityQuestionResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string GetSecurityQuestionResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public GetSecurityQuestionResponse() {
        }
        
        public GetSecurityQuestionResponse(string GetSecurityQuestionResult, string error) {
            this.GetSecurityQuestionResult = GetSecurityQuestionResult;
            this.error = error;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RestorePasswordFromUserNameAndSecurityQuestion", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RestorePasswordFromUserNameAndSecurityQuestionRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string userName;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string answer;
        
        public RestorePasswordFromUserNameAndSecurityQuestionRequest() {
        }
        
        public RestorePasswordFromUserNameAndSecurityQuestionRequest(string userName, string answer) {
            this.userName = userName;
            this.answer = answer;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RestorePasswordFromUserNameAndSecurityQuestionResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class RestorePasswordFromUserNameAndSecurityQuestionResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string RestorePasswordFromUserNameAndSecurityQuestionResult;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string error;
        
        public RestorePasswordFromUserNameAndSecurityQuestionResponse() {
        }
        
        public RestorePasswordFromUserNameAndSecurityQuestionResponse(string RestorePasswordFromUserNameAndSecurityQuestionResult, string error) {
            this.RestorePasswordFromUserNameAndSecurityQuestionResult = RestorePasswordFromUserNameAndSecurityQuestionResult;
            this.error = error;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAuthenticationChannel : PassiveClient.Authentication.IAuthentication, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AuthenticationClient : System.ServiceModel.ClientBase<PassiveClient.Authentication.IAuthentication>, PassiveClient.Authentication.IAuthentication {
        
        public AuthenticationClient() {
        }
        
        public AuthenticationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AuthenticationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.AuthenticateResponse PassiveClient.Authentication.IAuthentication.Authenticate(PassiveClient.Authentication.AuthenticateRequest request) {
            return base.Channel.Authenticate(request);
        }
        
        public string Authenticate(string userName, string password, out string error) {
            PassiveClient.Authentication.AuthenticateRequest inValue = new PassiveClient.Authentication.AuthenticateRequest();
            inValue.userName = userName;
            inValue.password = password;
            PassiveClient.Authentication.AuthenticateResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).Authenticate(inValue);
            error = retVal.error;
            return retVal.AuthenticateResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.AuthenticateResponse> AuthenticateAsync(PassiveClient.Authentication.AuthenticateRequest request) {
            return base.Channel.AuthenticateAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.SignUpResponse PassiveClient.Authentication.IAuthentication.SignUp(PassiveClient.Authentication.SignUpRequest request) {
            return base.Channel.SignUp(request);
        }
        
        public bool SignUp(string userName, string password, out string error) {
            PassiveClient.Authentication.SignUpRequest inValue = new PassiveClient.Authentication.SignUpRequest();
            inValue.userName = userName;
            inValue.password = password;
            PassiveClient.Authentication.SignUpResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).SignUp(inValue);
            error = retVal.error;
            return retVal.SignUpResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.SignUpResponse> SignUpAsync(PassiveClient.Authentication.SignUpRequest request) {
            return base.Channel.SignUpAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.ChangeUserPasswordResponse PassiveClient.Authentication.IAuthentication.ChangeUserPassword(PassiveClient.Authentication.ChangeUserPasswordRequest request) {
            return base.Channel.ChangeUserPassword(request);
        }
        
        public bool ChangeUserPassword(string userName, string oldPassword, string newPassword, out string error) {
            PassiveClient.Authentication.ChangeUserPasswordRequest inValue = new PassiveClient.Authentication.ChangeUserPasswordRequest();
            inValue.userName = userName;
            inValue.oldPassword = oldPassword;
            inValue.newPassword = newPassword;
            PassiveClient.Authentication.ChangeUserPasswordResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).ChangeUserPassword(inValue);
            error = retVal.error;
            return retVal.ChangeUserPasswordResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.ChangeUserPasswordResponse> ChangeUserPasswordAsync(PassiveClient.Authentication.ChangeUserPasswordRequest request) {
            return base.Channel.ChangeUserPasswordAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.SetSecurityQuestionAndAnswerResponse PassiveClient.Authentication.IAuthentication.SetSecurityQuestionAndAnswer(PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest request) {
            return base.Channel.SetSecurityQuestionAndAnswer(request);
        }
        
        public bool SetSecurityQuestionAndAnswer(string userName, string password, string question, string answer, out string error) {
            PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest inValue = new PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest();
            inValue.userName = userName;
            inValue.password = password;
            inValue.question = question;
            inValue.answer = answer;
            PassiveClient.Authentication.SetSecurityQuestionAndAnswerResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).SetSecurityQuestionAndAnswer(inValue);
            error = retVal.error;
            return retVal.SetSecurityQuestionAndAnswerResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.SetSecurityQuestionAndAnswerResponse> SetSecurityQuestionAndAnswerAsync(PassiveClient.Authentication.SetSecurityQuestionAndAnswerRequest request) {
            return base.Channel.SetSecurityQuestionAndAnswerAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.GetSecurityQuestionResponse PassiveClient.Authentication.IAuthentication.GetSecurityQuestion(PassiveClient.Authentication.GetSecurityQuestionRequest request) {
            return base.Channel.GetSecurityQuestion(request);
        }
        
        public string GetSecurityQuestion(string userName, out string error) {
            PassiveClient.Authentication.GetSecurityQuestionRequest inValue = new PassiveClient.Authentication.GetSecurityQuestionRequest();
            inValue.userName = userName;
            PassiveClient.Authentication.GetSecurityQuestionResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).GetSecurityQuestion(inValue);
            error = retVal.error;
            return retVal.GetSecurityQuestionResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.GetSecurityQuestionResponse> GetSecurityQuestionAsync(PassiveClient.Authentication.GetSecurityQuestionRequest request) {
            return base.Channel.GetSecurityQuestionAsync(request);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionResponse PassiveClient.Authentication.IAuthentication.RestorePasswordFromUserNameAndSecurityQuestion(PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest request) {
            return base.Channel.RestorePasswordFromUserNameAndSecurityQuestion(request);
        }
        
        public string RestorePasswordFromUserNameAndSecurityQuestion(string userName, string answer, out string error) {
            PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest inValue = new PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest();
            inValue.userName = userName;
            inValue.answer = answer;
            PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionResponse retVal = ((PassiveClient.Authentication.IAuthentication)(this)).RestorePasswordFromUserNameAndSecurityQuestion(inValue);
            error = retVal.error;
            return retVal.RestorePasswordFromUserNameAndSecurityQuestionResult;
        }
        
        public System.Threading.Tasks.Task<PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionResponse> RestorePasswordFromUserNameAndSecurityQuestionAsync(PassiveClient.Authentication.RestorePasswordFromUserNameAndSecurityQuestionRequest request) {
            return base.Channel.RestorePasswordFromUserNameAndSecurityQuestionAsync(request);
        }
    }
}
