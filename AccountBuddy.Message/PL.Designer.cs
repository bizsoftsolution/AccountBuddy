﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountBuddy.Message {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class PL {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PL() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AccountBuddy.Message.PL", typeof(PL).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Record Removed.
        /// </summary>
        public static string Delete_Alert {
            get {
                return ResourceManager.GetString("Delete_Alert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do you want to Delete this record?.
        /// </summary>
        public static string Delete_confirmation {
            get {
                return ResourceManager.GetString("Delete_confirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Permission to Delete the {0}.
        /// </summary>
        public static string DenyDelete {
            get {
                return ResourceManager.GetString("DenyDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Permission to view the {0}.
        /// </summary>
        public static string DenyFormShow {
            get {
                return ResourceManager.GetString("DenyFormShow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Permission to Insert  the {0}.
        /// </summary>
        public static string DenyInsert {
            get {
                return ResourceManager.GetString("DenyInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No Permission to Update the {0}.
        /// </summary>
        public static string DenyUpdate {
            get {
                return ResourceManager.GetString("DenyUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is Empty!.
        /// </summary>
        public static string Empty_Record {
            get {
                return ResourceManager.GetString("Empty_Record", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}, Already Exist..
        /// </summary>
        public static string Existing_Data {
            get {
                return ResourceManager.GetString("Existing_Data", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Saved Successfully.
        /// </summary>
        public static string Saved_Alert {
            get {
                return ResourceManager.GetString("Saved_Alert", resourceCulture);
            }
        }
    }
}
