﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("DbModels", "FK_Challenge_0", "Customer", System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(DareyaAPI.Database.Customer), "Challenge", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(DareyaAPI.Database.Challenge), true)]

#endregion

namespace DareyaAPI.Database
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class DYDbEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new DYDbEntities object using the connection string found in the 'DYDbEntities' section of the application configuration file.
        /// </summary>
        public DYDbEntities() : base("name=DYDbEntities", "DYDbEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DYDbEntities object.
        /// </summary>
        public DYDbEntities(string connectionString) : base(connectionString, "DYDbEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new DYDbEntities object.
        /// </summary>
        public DYDbEntities(EntityConnection connection) : base(connection, "DYDbEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Challenge> Challenge
        {
            get
            {
                if ((_Challenge == null))
                {
                    _Challenge = base.CreateObjectSet<Challenge>("Challenge");
                }
                return _Challenge;
            }
        }
        private ObjectSet<Challenge> _Challenge;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Customer> Customer
        {
            get
            {
                if ((_Customer == null))
                {
                    _Customer = base.CreateObjectSet<Customer>("Customer");
                }
                return _Customer;
            }
        }
        private ObjectSet<Customer> _Customer;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Bid> Bid
        {
            get
            {
                if ((_Bid == null))
                {
                    _Bid = base.CreateObjectSet<Bid>("Bid");
                }
                return _Bid;
            }
        }
        private ObjectSet<Bid> _Bid;

        #endregion

        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Challenge EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToChallenge(Challenge challenge)
        {
            base.AddObject("Challenge", challenge);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Customer EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToCustomer(Customer customer)
        {
            base.AddObject("Customer", customer);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Bid EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToBid(Bid bid)
        {
            base.AddObject("Bid", bid);
        }

        #endregion

    }

    #endregion

    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DbModels", Name="Bid")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Bid : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Bid object.
        /// </summary>
        /// <param name="id">Initial value of the ID property.</param>
        /// <param name="customerID">Initial value of the CustomerID property.</param>
        /// <param name="amount">Initial value of the Amount property.</param>
        public static Bid CreateBid(global::System.Int64 id, global::System.Int64 customerID, global::System.Int32 amount)
        {
            Bid bid = new Bid();
            bid.ID = id;
            bid.CustomerID = customerID;
            bid.Amount = amount;
            return bid;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    OnIDChanging(value);
                    ReportPropertyChanging("ID");
                    _ID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("ID");
                    OnIDChanged();
                }
            }
        }
        private global::System.Int64 _ID;
        partial void OnIDChanging(global::System.Int64 value);
        partial void OnIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                OnCustomerIDChanging(value);
                ReportPropertyChanging("CustomerID");
                _CustomerID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CustomerID");
                OnCustomerIDChanged();
            }
        }
        private global::System.Int64 _CustomerID;
        partial void OnCustomerIDChanging(global::System.Int64 value);
        partial void OnCustomerIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                OnAmountChanging(value);
                ReportPropertyChanging("Amount");
                _Amount = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Amount");
                OnAmountChanged();
            }
        }
        private global::System.Int32 _Amount;
        partial void OnAmountChanging(global::System.Int32 value);
        partial void OnAmountChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> ChallengeID
        {
            get
            {
                return _ChallengeID;
            }
            set
            {
                OnChallengeIDChanging(value);
                ReportPropertyChanging("ChallengeID");
                _ChallengeID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ChallengeID");
                OnChallengeIDChanged();
            }
        }
        private Nullable<global::System.Int64> _ChallengeID;
        partial void OnChallengeIDChanging(Nullable<global::System.Int64> value);
        partial void OnChallengeIDChanged();

        #endregion

    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DbModels", Name="Challenge")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Challenge : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Challenge object.
        /// </summary>
        /// <param name="id">Initial value of the ID property.</param>
        /// <param name="title">Initial value of the Title property.</param>
        /// <param name="description">Initial value of the Description property.</param>
        /// <param name="customerID">Initial value of the CustomerID property.</param>
        public static Challenge CreateChallenge(global::System.Int64 id, global::System.String title, global::System.String description, global::System.Int64 customerID)
        {
            Challenge challenge = new Challenge();
            challenge.ID = id;
            challenge.Title = title;
            challenge.Description = description;
            challenge.CustomerID = customerID;
            return challenge;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    OnIDChanging(value);
                    ReportPropertyChanging("ID");
                    _ID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("ID");
                    OnIDChanged();
                }
            }
        }
        private global::System.Int64 _ID;
        partial void OnIDChanging(global::System.Int64 value);
        partial void OnIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                OnTitleChanging(value);
                ReportPropertyChanging("Title");
                _Title = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Title");
                OnTitleChanged();
            }
        }
        private global::System.String _Title;
        partial void OnTitleChanging(global::System.String value);
        partial void OnTitleChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ReportPropertyChanging("Description");
                _Description = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private global::System.String _Description;
        partial void OnDescriptionChanging(global::System.String value);
        partial void OnDescriptionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Byte> Privacy
        {
            get
            {
                return _Privacy;
            }
            set
            {
                OnPrivacyChanging(value);
                ReportPropertyChanging("Privacy");
                _Privacy = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Privacy");
                OnPrivacyChanged();
            }
        }
        private Nullable<global::System.Byte> _Privacy;
        partial void OnPrivacyChanging(Nullable<global::System.Byte> value);
        partial void OnPrivacyChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Byte> Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                ReportPropertyChanging("Type");
                _Type = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Type");
                OnTypeChanged();
            }
        }
        private Nullable<global::System.Byte> _Type;
        partial void OnTypeChanging(Nullable<global::System.Byte> value);
        partial void OnTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 CustomerID
        {
            get
            {
                return _CustomerID;
            }
            set
            {
                OnCustomerIDChanging(value);
                ReportPropertyChanging("CustomerID");
                _CustomerID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CustomerID");
                OnCustomerIDChanged();
            }
        }
        private global::System.Int64 _CustomerID;
        partial void OnCustomerIDChanging(global::System.Int64 value);
        partial void OnCustomerIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> CurrentBid
        {
            get
            {
                return _CurrentBid;
            }
            set
            {
                OnCurrentBidChanging(value);
                ReportPropertyChanging("CurrentBid");
                _CurrentBid = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CurrentBid");
                OnCurrentBidChanged();
            }
        }
        private Nullable<global::System.Int32> _CurrentBid;
        partial void OnCurrentBidChanging(Nullable<global::System.Int32> value);
        partial void OnCurrentBidChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int64> TargetCustomerID
        {
            get
            {
                return _TargetCustomerID;
            }
            set
            {
                OnTargetCustomerIDChanging(value);
                ReportPropertyChanging("TargetCustomerID");
                _TargetCustomerID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("TargetCustomerID");
                OnTargetCustomerIDChanged();
            }
        }
        private Nullable<global::System.Int64> _TargetCustomerID;
        partial void OnTargetCustomerIDChanging(Nullable<global::System.Int64> value);
        partial void OnTargetCustomerIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> State
        {
            get
            {
                return _State;
            }
            set
            {
                OnStateChanging(value);
                ReportPropertyChanging("State");
                _State = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("State");
                OnStateChanged();
            }
        }
        private Nullable<global::System.Int32> _State;
        partial void OnStateChanging(Nullable<global::System.Int32> value);
        partial void OnStateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Byte> Anonymous
        {
            get
            {
                return _Anonymous;
            }
            set
            {
                OnAnonymousChanging(value);
                ReportPropertyChanging("Anonymous");
                _Anonymous = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Anonymous");
                OnAnonymousChanged();
            }
        }
        private Nullable<global::System.Byte> _Anonymous;
        partial void OnAnonymousChanging(Nullable<global::System.Byte> value);
        partial void OnAnonymousChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DbModels", "FK_Challenge_0", "Customer")]
        public Customer Customer
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("DbModels.FK_Challenge_0", "Customer").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("DbModels.FK_Challenge_0", "Customer").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Customer> CustomerReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Customer>("DbModels.FK_Challenge_0", "Customer");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Customer>("DbModels.FK_Challenge_0", "Customer", value);
                }
            }
        }

        #endregion

    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="DbModels", Name="Customer")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Customer : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Customer object.
        /// </summary>
        /// <param name="id">Initial value of the ID property.</param>
        /// <param name="firstName">Initial value of the FirstName property.</param>
        /// <param name="lastName">Initial value of the LastName property.</param>
        public static Customer CreateCustomer(global::System.Int64 id, global::System.String firstName, global::System.String lastName)
        {
            Customer customer = new Customer();
            customer.ID = id;
            customer.FirstName = firstName;
            customer.LastName = lastName;
            return customer;
        }

        #endregion

        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int64 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    OnIDChanging(value);
                    ReportPropertyChanging("ID");
                    _ID = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("ID");
                    OnIDChanged();
                }
            }
        }
        private global::System.Int64 _ID;
        partial void OnIDChanging(global::System.Int64 value);
        partial void OnIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                OnFirstNameChanging(value);
                ReportPropertyChanging("FirstName");
                _FirstName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("FirstName");
                OnFirstNameChanged();
            }
        }
        private global::System.String _FirstName;
        partial void OnFirstNameChanging(global::System.String value);
        partial void OnFirstNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                OnLastNameChanging(value);
                ReportPropertyChanging("LastName");
                _LastName = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("LastName");
                OnLastNameChanged();
            }
        }
        private global::System.String _LastName;
        partial void OnLastNameChanging(global::System.String value);
        partial void OnLastNameChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Address1
        {
            get
            {
                return _Address1;
            }
            set
            {
                OnAddress1Changing(value);
                ReportPropertyChanging("Address1");
                _Address1 = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Address1");
                OnAddress1Changed();
            }
        }
        private global::System.String _Address1;
        partial void OnAddress1Changing(global::System.String value);
        partial void OnAddress1Changed();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Address2
        {
            get
            {
                return _Address2;
            }
            set
            {
                OnAddress2Changing(value);
                ReportPropertyChanging("Address2");
                _Address2 = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Address2");
                OnAddress2Changed();
            }
        }
        private global::System.String _Address2;
        partial void OnAddress2Changing(global::System.String value);
        partial void OnAddress2Changed();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String City
        {
            get
            {
                return _City;
            }
            set
            {
                OnCityChanging(value);
                ReportPropertyChanging("City");
                _City = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("City");
                OnCityChanged();
            }
        }
        private global::System.String _City;
        partial void OnCityChanging(global::System.String value);
        partial void OnCityChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String State
        {
            get
            {
                return _State;
            }
            set
            {
                OnStateChanging(value);
                ReportPropertyChanging("State");
                _State = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("State");
                OnStateChanged();
            }
        }
        private global::System.String _State;
        partial void OnStateChanging(global::System.String value);
        partial void OnStateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ZIPCode
        {
            get
            {
                return _ZIPCode;
            }
            set
            {
                OnZIPCodeChanging(value);
                ReportPropertyChanging("ZIPCode");
                _ZIPCode = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ZIPCode");
                OnZIPCodeChanged();
            }
        }
        private global::System.String _ZIPCode;
        partial void OnZIPCodeChanging(global::System.String value);
        partial void OnZIPCodeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> BillingType
        {
            get
            {
                return _BillingType;
            }
            set
            {
                OnBillingTypeChanging(value);
                ReportPropertyChanging("BillingType");
                _BillingType = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("BillingType");
                OnBillingTypeChanged();
            }
        }
        private Nullable<global::System.Int32> _BillingType;
        partial void OnBillingTypeChanging(Nullable<global::System.Int32> value);
        partial void OnBillingTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String BillingID
        {
            get
            {
                return _BillingID;
            }
            set
            {
                OnBillingIDChanging(value);
                ReportPropertyChanging("BillingID");
                _BillingID = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("BillingID");
                OnBillingIDChanged();
            }
        }
        private global::System.String _BillingID;
        partial void OnBillingIDChanging(global::System.String value);
        partial void OnBillingIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Password
        {
            get
            {
                return _Password;
            }
            set
            {
                OnPasswordChanging(value);
                ReportPropertyChanging("Password");
                _Password = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Password");
                OnPasswordChanged();
            }
        }
        private global::System.String _Password;
        partial void OnPasswordChanging(global::System.String value);
        partial void OnPasswordChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String EmailAddress
        {
            get
            {
                return _EmailAddress;
            }
            set
            {
                OnEmailAddressChanging(value);
                ReportPropertyChanging("EmailAddress");
                _EmailAddress = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("EmailAddress");
                OnEmailAddressChanged();
            }
        }
        private global::System.String _EmailAddress;
        partial void OnEmailAddressChanging(global::System.String value);
        partial void OnEmailAddressChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Verified
        {
            get
            {
                return _Verified;
            }
            set
            {
                OnVerifiedChanging(value);
                ReportPropertyChanging("Verified");
                _Verified = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Verified");
                OnVerifiedChanged();
            }
        }
        private Nullable<global::System.Int32> _Verified;
        partial void OnVerifiedChanging(Nullable<global::System.Int32> value);
        partial void OnVerifiedChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String FacebookUserID
        {
            get
            {
                return _FacebookUserID;
            }
            set
            {
                OnFacebookUserIDChanging(value);
                ReportPropertyChanging("FacebookUserID");
                _FacebookUserID = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("FacebookUserID");
                OnFacebookUserIDChanged();
            }
        }
        private global::System.String _FacebookUserID;
        partial void OnFacebookUserIDChanging(global::System.String value);
        partial void OnFacebookUserIDChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String FacebookAccessToken
        {
            get
            {
                return _FacebookAccessToken;
            }
            set
            {
                OnFacebookAccessTokenChanging(value);
                ReportPropertyChanging("FacebookAccessToken");
                _FacebookAccessToken = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("FacebookAccessToken");
                OnFacebookAccessTokenChanged();
            }
        }
        private global::System.String _FacebookAccessToken;
        partial void OnFacebookAccessTokenChanging(global::System.String value);
        partial void OnFacebookAccessTokenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String FacebookExpires
        {
            get
            {
                return _FacebookExpires;
            }
            set
            {
                OnFacebookExpiresChanging(value);
                ReportPropertyChanging("FacebookExpires");
                _FacebookExpires = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("FacebookExpires");
                OnFacebookExpiresChanged();
            }
        }
        private global::System.String _FacebookExpires;
        partial void OnFacebookExpiresChanging(global::System.String value);
        partial void OnFacebookExpiresChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                OnPhoneNumberChanging(value);
                ReportPropertyChanging("PhoneNumber");
                _PhoneNumber = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("PhoneNumber");
                OnPhoneNumberChanged();
            }
        }
        private global::System.String _PhoneNumber;
        partial void OnPhoneNumberChanging(global::System.String value);
        partial void OnPhoneNumberChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                ReportPropertyChanging("Type");
                _Type = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Type");
                OnTypeChanged();
            }
        }
        private Nullable<global::System.Int32> _Type;
        partial void OnTypeChanging(Nullable<global::System.Int32> value);
        partial void OnTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> CurrentBalance
        {
            get
            {
                return _CurrentBalance;
            }
            set
            {
                OnCurrentBalanceChanging(value);
                ReportPropertyChanging("CurrentBalance");
                _CurrentBalance = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("CurrentBalance");
                OnCurrentBalanceChanged();
            }
        }
        private Nullable<global::System.Decimal> _CurrentBalance;
        partial void OnCurrentBalanceChanging(Nullable<global::System.Decimal> value);
        partial void OnCurrentBalanceChanged();

        #endregion

    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("DbModels", "FK_Challenge_0", "Challenge")]
        public EntityCollection<Challenge> Challenge
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Challenge>("DbModels.FK_Challenge_0", "Challenge");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Challenge>("DbModels.FK_Challenge_0", "Challenge", value);
                }
            }
        }

        #endregion

    }

    #endregion

    
}
