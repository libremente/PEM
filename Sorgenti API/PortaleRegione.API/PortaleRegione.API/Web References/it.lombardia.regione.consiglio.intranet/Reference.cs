﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:4.0.30319.42000
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Il codice sorgente è stato generato automaticamente da Microsoft.VSDesigner, versione 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace PortaleRegione.API.it.lombardia.regione.consiglio.intranet {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="proxyADSoap", Namespace="https://intranet.consiglio.regione.lombardia.it/proxyAD")]
    public partial class proxyAD : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ConnTestOperationCompleted;
        
        private System.Threading.SendOrPostCallback AuthenticateOperationCompleted;
        
        private System.Threading.SendOrPostCallback PasswordExpireOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetGroupsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetUser_in_GroupOperationCompleted;
        
        private System.Threading.SendOrPostCallback RemoveUserFromADGroupOperationCompleted;
        
        private System.Threading.SendOrPostCallback AddUserToADGroupOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreatePEMADUserOperationCompleted;
        
        private System.Threading.SendOrPostCallback ChangeADUserPassOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreatePEMADGroupOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public proxyAD() {
            this.Url = global::PortaleRegione.API.Properties.Settings.Default.PortaleRegione_API_it_lombardia_regione_consiglio_intranet_proxyAD;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event ConnTestCompletedEventHandler ConnTestCompleted;
        
        /// <remarks/>
        public event AuthenticateCompletedEventHandler AuthenticateCompleted;
        
        /// <remarks/>
        public event PasswordExpireCompletedEventHandler PasswordExpireCompleted;
        
        /// <remarks/>
        public event GetGroupsCompletedEventHandler GetGroupsCompleted;
        
        /// <remarks/>
        public event GetUser_in_GroupCompletedEventHandler GetUser_in_GroupCompleted;
        
        /// <remarks/>
        public event RemoveUserFromADGroupCompletedEventHandler RemoveUserFromADGroupCompleted;
        
        /// <remarks/>
        public event AddUserToADGroupCompletedEventHandler AddUserToADGroupCompleted;
        
        /// <remarks/>
        public event CreatePEMADUserCompletedEventHandler CreatePEMADUserCompleted;
        
        /// <remarks/>
        public event ChangeADUserPassCompletedEventHandler ChangeADUserPassCompleted;
        
        /// <remarks/>
        public event CreatePEMADGroupCompletedEventHandler CreatePEMADGroupCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/ConnTest", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ConnTest() {
            object[] results = this.Invoke("ConnTest", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConnTestAsync() {
            this.ConnTestAsync(null);
        }
        
        /// <remarks/>
        public void ConnTestAsync(object userState) {
            if ((this.ConnTestOperationCompleted == null)) {
                this.ConnTestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConnTestOperationCompleted);
            }
            this.InvokeAsync("ConnTest", new object[0], this.ConnTestOperationCompleted, userState);
        }
        
        private void OnConnTestOperationCompleted(object arg) {
            if ((this.ConnTestCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConnTestCompleted(this, new ConnTestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/Authenticate", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool Authenticate(string userName, string password, string domain, string token) {
            object[] results = this.Invoke("Authenticate", new object[] {
                        userName,
                        password,
                        domain,
                        token});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void AuthenticateAsync(string userName, string password, string domain, string token) {
            this.AuthenticateAsync(userName, password, domain, token, null);
        }
        
        /// <remarks/>
        public void AuthenticateAsync(string userName, string password, string domain, string token, object userState) {
            if ((this.AuthenticateOperationCompleted == null)) {
                this.AuthenticateOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAuthenticateOperationCompleted);
            }
            this.InvokeAsync("Authenticate", new object[] {
                        userName,
                        password,
                        domain,
                        token}, this.AuthenticateOperationCompleted, userState);
        }
        
        private void OnAuthenticateOperationCompleted(object arg) {
            if ((this.AuthenticateCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AuthenticateCompleted(this, new AuthenticateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/PasswordExpire", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long PasswordExpire(string userName, string password, string domain, string token) {
            object[] results = this.Invoke("PasswordExpire", new object[] {
                        userName,
                        password,
                        domain,
                        token});
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public void PasswordExpireAsync(string userName, string password, string domain, string token) {
            this.PasswordExpireAsync(userName, password, domain, token, null);
        }
        
        /// <remarks/>
        public void PasswordExpireAsync(string userName, string password, string domain, string token, object userState) {
            if ((this.PasswordExpireOperationCompleted == null)) {
                this.PasswordExpireOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPasswordExpireOperationCompleted);
            }
            this.InvokeAsync("PasswordExpire", new object[] {
                        userName,
                        password,
                        domain,
                        token}, this.PasswordExpireOperationCompleted, userState);
        }
        
        private void OnPasswordExpireOperationCompleted(object arg) {
            if ((this.PasswordExpireCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PasswordExpireCompleted(this, new PasswordExpireCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/GetGroups", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetGroups(string userName, string simplefilter, string token) {
            object[] results = this.Invoke("GetGroups", new object[] {
                        userName,
                        simplefilter,
                        token});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetGroupsAsync(string userName, string simplefilter, string token) {
            this.GetGroupsAsync(userName, simplefilter, token, null);
        }
        
        /// <remarks/>
        public void GetGroupsAsync(string userName, string simplefilter, string token, object userState) {
            if ((this.GetGroupsOperationCompleted == null)) {
                this.GetGroupsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetGroupsOperationCompleted);
            }
            this.InvokeAsync("GetGroups", new object[] {
                        userName,
                        simplefilter,
                        token}, this.GetGroupsOperationCompleted, userState);
        }
        
        private void OnGetGroupsOperationCompleted(object arg) {
            if ((this.GetGroupsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetGroupsCompleted(this, new GetGroupsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/GetUser_in_Group", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetUser_in_Group(string groupDn, string token) {
            object[] results = this.Invoke("GetUser_in_Group", new object[] {
                        groupDn,
                        token});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetUser_in_GroupAsync(string groupDn, string token) {
            this.GetUser_in_GroupAsync(groupDn, token, null);
        }
        
        /// <remarks/>
        public void GetUser_in_GroupAsync(string groupDn, string token, object userState) {
            if ((this.GetUser_in_GroupOperationCompleted == null)) {
                this.GetUser_in_GroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetUser_in_GroupOperationCompleted);
            }
            this.InvokeAsync("GetUser_in_Group", new object[] {
                        groupDn,
                        token}, this.GetUser_in_GroupOperationCompleted, userState);
        }
        
        private void OnGetUser_in_GroupOperationCompleted(object arg) {
            if ((this.GetUser_in_GroupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetUser_in_GroupCompleted(this, new GetUser_in_GroupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/RemoveUserFromADGroup", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool RemoveUserFromADGroup(string groupDn, string userDn, string token) {
            object[] results = this.Invoke("RemoveUserFromADGroup", new object[] {
                        groupDn,
                        userDn,
                        token});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void RemoveUserFromADGroupAsync(string groupDn, string userDn, string token) {
            this.RemoveUserFromADGroupAsync(groupDn, userDn, token, null);
        }
        
        /// <remarks/>
        public void RemoveUserFromADGroupAsync(string groupDn, string userDn, string token, object userState) {
            if ((this.RemoveUserFromADGroupOperationCompleted == null)) {
                this.RemoveUserFromADGroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRemoveUserFromADGroupOperationCompleted);
            }
            this.InvokeAsync("RemoveUserFromADGroup", new object[] {
                        groupDn,
                        userDn,
                        token}, this.RemoveUserFromADGroupOperationCompleted, userState);
        }
        
        private void OnRemoveUserFromADGroupOperationCompleted(object arg) {
            if ((this.RemoveUserFromADGroupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RemoveUserFromADGroupCompleted(this, new RemoveUserFromADGroupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/AddUserToADGroup", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int AddUserToADGroup(string groupDn, string userDn, string token) {
            object[] results = this.Invoke("AddUserToADGroup", new object[] {
                        groupDn,
                        userDn,
                        token});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void AddUserToADGroupAsync(string groupDn, string userDn, string token) {
            this.AddUserToADGroupAsync(groupDn, userDn, token, null);
        }
        
        /// <remarks/>
        public void AddUserToADGroupAsync(string groupDn, string userDn, string token, object userState) {
            if ((this.AddUserToADGroupOperationCompleted == null)) {
                this.AddUserToADGroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddUserToADGroupOperationCompleted);
            }
            this.InvokeAsync("AddUserToADGroup", new object[] {
                        groupDn,
                        userDn,
                        token}, this.AddUserToADGroupOperationCompleted, userState);
        }
        
        private void OnAddUserToADGroupOperationCompleted(object arg) {
            if ((this.AddUserToADGroupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddUserToADGroupCompleted(this, new AddUserToADGroupCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/CreatePEMADUser", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void CreatePEMADUser(string userDn, string Password, bool usrGiunta, string token) {
            this.Invoke("CreatePEMADUser", new object[] {
                        userDn,
                        Password,
                        usrGiunta,
                        token});
        }
        
        /// <remarks/>
        public void CreatePEMADUserAsync(string userDn, string Password, bool usrGiunta, string token) {
            this.CreatePEMADUserAsync(userDn, Password, usrGiunta, token, null);
        }
        
        /// <remarks/>
        public void CreatePEMADUserAsync(string userDn, string Password, bool usrGiunta, string token, object userState) {
            if ((this.CreatePEMADUserOperationCompleted == null)) {
                this.CreatePEMADUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreatePEMADUserOperationCompleted);
            }
            this.InvokeAsync("CreatePEMADUser", new object[] {
                        userDn,
                        Password,
                        usrGiunta,
                        token}, this.CreatePEMADUserOperationCompleted, userState);
        }
        
        private void OnCreatePEMADUserOperationCompleted(object arg) {
            if ((this.CreatePEMADUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreatePEMADUserCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/ChangeADUserPass", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string ChangeADUserPass(string userDn, string oldPassword, string newPassword, string token) {
            object[] results = this.Invoke("ChangeADUserPass", new object[] {
                        userDn,
                        oldPassword,
                        newPassword,
                        token});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ChangeADUserPassAsync(string userDn, string oldPassword, string newPassword, string token) {
            this.ChangeADUserPassAsync(userDn, oldPassword, newPassword, token, null);
        }
        
        /// <remarks/>
        public void ChangeADUserPassAsync(string userDn, string oldPassword, string newPassword, string token, object userState) {
            if ((this.ChangeADUserPassOperationCompleted == null)) {
                this.ChangeADUserPassOperationCompleted = new System.Threading.SendOrPostCallback(this.OnChangeADUserPassOperationCompleted);
            }
            this.InvokeAsync("ChangeADUserPass", new object[] {
                        userDn,
                        oldPassword,
                        newPassword,
                        token}, this.ChangeADUserPassOperationCompleted, userState);
        }
        
        private void OnChangeADUserPassOperationCompleted(object arg) {
            if ((this.ChangeADUserPassCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ChangeADUserPassCompleted(this, new ChangeADUserPassCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://intranet.consiglio.regione.lombardia.it/proxyAD/CreatePEMADGroup", RequestNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", ResponseNamespace="https://intranet.consiglio.regione.lombardia.it/proxyAD", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void CreatePEMADGroup(string groupDn, string users, string token) {
            this.Invoke("CreatePEMADGroup", new object[] {
                        groupDn,
                        users,
                        token});
        }
        
        /// <remarks/>
        public void CreatePEMADGroupAsync(string groupDn, string users, string token) {
            this.CreatePEMADGroupAsync(groupDn, users, token, null);
        }
        
        /// <remarks/>
        public void CreatePEMADGroupAsync(string groupDn, string users, string token, object userState) {
            if ((this.CreatePEMADGroupOperationCompleted == null)) {
                this.CreatePEMADGroupOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreatePEMADGroupOperationCompleted);
            }
            this.InvokeAsync("CreatePEMADGroup", new object[] {
                        groupDn,
                        users,
                        token}, this.CreatePEMADGroupOperationCompleted, userState);
        }
        
        private void OnCreatePEMADGroupOperationCompleted(object arg) {
            if ((this.CreatePEMADGroupCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreatePEMADGroupCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void ConnTestCompletedEventHandler(object sender, ConnTestCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConnTestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConnTestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void AuthenticateCompletedEventHandler(object sender, AuthenticateCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AuthenticateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AuthenticateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void PasswordExpireCompletedEventHandler(object sender, PasswordExpireCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PasswordExpireCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PasswordExpireCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void GetGroupsCompletedEventHandler(object sender, GetGroupsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetGroupsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetGroupsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void GetUser_in_GroupCompletedEventHandler(object sender, GetUser_in_GroupCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetUser_in_GroupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetUser_in_GroupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void RemoveUserFromADGroupCompletedEventHandler(object sender, RemoveUserFromADGroupCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RemoveUserFromADGroupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RemoveUserFromADGroupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void AddUserToADGroupCompletedEventHandler(object sender, AddUserToADGroupCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddUserToADGroupCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddUserToADGroupCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void CreatePEMADUserCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void ChangeADUserPassCompletedEventHandler(object sender, ChangeADUserPassCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ChangeADUserPassCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ChangeADUserPassCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.3752.0")]
    public delegate void CreatePEMADGroupCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591