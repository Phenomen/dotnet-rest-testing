﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Accounts.ServiceReference1 {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AccountModel", Namespace="http://schemas.datacontract.org/2004/07/Rokolabs.AutomationTestingTask.Soap.Model" +
        "s")]
    [System.SerializableAttribute()]
    public partial class AccountModel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LoginField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Login {
            get {
                return this.LoginField;
            }
            set {
                if ((object.ReferenceEquals(this.LoginField, value) != true)) {
                    this.LoginField = value;
                    this.RaisePropertyChanged("Login");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IAccounts")]
    public interface IAccounts {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Login", ReplyAction="http://tempuri.org/IAccounts/LoginResponse")]
        string Login([System.ServiceModel.MessageParameterAttribute(Name="login")] string login1, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Login", ReplyAction="http://tempuri.org/IAccounts/LoginResponse")]
        System.Threading.Tasks.Task<string> LoginAsync(string login, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Logout", ReplyAction="http://tempuri.org/IAccounts/LogoutResponse")]
        bool Logout(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Logout", ReplyAction="http://tempuri.org/IAccounts/LogoutResponse")]
        System.Threading.Tasks.Task<bool> LogoutAsync(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Registrate", ReplyAction="http://tempuri.org/IAccounts/RegistrateResponse")]
        Accounts.ServiceReference1.AccountModel Registrate(string login, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/Registrate", ReplyAction="http://tempuri.org/IAccounts/RegistrateResponse")]
        System.Threading.Tasks.Task<Accounts.ServiceReference1.AccountModel> RegistrateAsync(string login, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/GetUserBySessionId", ReplyAction="http://tempuri.org/IAccounts/GetUserBySessionIdResponse")]
        Accounts.ServiceReference1.AccountModel GetUserBySessionId(string sessionId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAccounts/GetUserBySessionId", ReplyAction="http://tempuri.org/IAccounts/GetUserBySessionIdResponse")]
        System.Threading.Tasks.Task<Accounts.ServiceReference1.AccountModel> GetUserBySessionIdAsync(string sessionId);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAccountsChannel : Accounts.ServiceReference1.IAccounts, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AccountsClient : System.ServiceModel.ClientBase<Accounts.ServiceReference1.IAccounts>, Accounts.ServiceReference1.IAccounts {
        
        public AccountsClient() {
        }
        
        public AccountsClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AccountsClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AccountsClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AccountsClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Login(string login1, string password) {
            return base.Channel.Login(login1, password);
        }
        
        public System.Threading.Tasks.Task<string> LoginAsync(string login, string password) {
            return base.Channel.LoginAsync(login, password);
        }
        
        public bool Logout(string sessionId) {
            return base.Channel.Logout(sessionId);
        }
        
        public System.Threading.Tasks.Task<bool> LogoutAsync(string sessionId) {
            return base.Channel.LogoutAsync(sessionId);
        }
        
        public Accounts.ServiceReference1.AccountModel Registrate(string login, string password) {
            return base.Channel.Registrate(login, password);
        }
        
        public System.Threading.Tasks.Task<Accounts.ServiceReference1.AccountModel> RegistrateAsync(string login, string password) {
            return base.Channel.RegistrateAsync(login, password);
        }
        
        public Accounts.ServiceReference1.AccountModel GetUserBySessionId(string sessionId) {
            return base.Channel.GetUserBySessionId(sessionId);
        }
        
        public System.Threading.Tasks.Task<Accounts.ServiceReference1.AccountModel> GetUserBySessionIdAsync(string sessionId) {
            return base.Channel.GetUserBySessionIdAsync(sessionId);
        }
    }
}
