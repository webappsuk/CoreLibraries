﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplications.Utilities.Scheduling {
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
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WebApplications.Utilities.Scheduling.Resource", typeof(Resource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create an aggregate schedule out of schedules which have different options..
        /// </summary>
        internal static string AggregateSchedule_Different_Options {
            get {
                return ResourceManager.GetString("AggregateSchedule_Different_Options", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified duration &apos;{0}&apos; must be less than &apos;{1}&apos;..
        /// </summary>
        internal static string DurationValidator_Validate_ExclusiveMaximum {
            get {
                return ResourceManager.GetString("DurationValidator_Validate_ExclusiveMaximum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified duration &apos;{0}&apos; must be greater than &apos;{1}&apos;..
        /// </summary>
        internal static string DurationValidator_Validate_ExclusiveMinimum {
            get {
                return ResourceManager.GetString("DurationValidator_Validate_ExclusiveMinimum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified duration &apos;{0}&apos; must be less than, or equal to, &apos;{1}&apos;..
        /// </summary>
        internal static string DurationValidator_Validate_InclusiveMaximum {
            get {
                return ResourceManager.GetString("DurationValidator_Validate_InclusiveMaximum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified duration &apos;{0}&apos; must be greater than, or equal to, &apos;{1}&apos;..
        /// </summary>
        internal static string DurationValidator_Validate_InclusiveMinimum {
            get {
                return ResourceManager.GetString("DurationValidator_Validate_InclusiveMinimum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified value must be a Duration..
        /// </summary>
        internal static string DurationValidator_Validate_NotDuration {
            get {
                return ResourceManager.GetString("DurationValidator_Validate_NotDuration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; does not represent a valid timezone..
        /// </summary>
        internal static string OneOffSchedule_InvalidTimeZone {
            get {
                return ResourceManager.GetString("OneOffSchedule_InvalidTimeZone", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid task status &apos;{0}&apos; in scheduled action continuation..
        /// </summary>
        internal static string ScheduledAction_ExecuteAsync_Invalid_Task_Status {
            get {
                return ResourceManager.GetString("ScheduledAction_ExecuteAsync_Invalid_Task_Status", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scheduled function returned a null task..
        /// </summary>
        internal static string ScheduledFunction_DoExecuteAsync_Null_Task {
            get {
                return ResourceManager.GetString("ScheduledFunction_DoExecuteAsync_Null_Task", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Schendule name cannot be null.
        /// </summary>
        internal static string Scheduler_AddSchedule_SchenduleNameNull {
            get {
                return ResourceManager.GetString("Scheduler_AddSchedule_SchenduleNameNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The default maximum history for a scheduler cannot be less than 1..
        /// </summary>
        internal static string Scheduler_DefaultMaximumHistory_Negative {
            get {
                return ResourceManager.GetString("Scheduler_DefaultMaximumHistory_Negative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; schedule was not found..
        /// </summary>
        internal static string Scheduler_GetSchedule_NotFound {
            get {
                return ResourceManager.GetString("Scheduler_GetSchedule_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A fatal exception occurred whilst trying to load the TimeZone DB &apos;{0}..
        /// </summary>
        internal static string Scheduler_Scheduler_TimeZoneDB_Failed {
            get {
                return ResourceManager.GetString("Scheduler_Scheduler_TimeZoneDB_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified path to the time zone database &apos;{0}&apos; does not exist..
        /// </summary>
        internal static string Scheduler_Scheduler_TimeZoneDB_Not_Found {
            get {
                return ResourceManager.GetString("Scheduler_Scheduler_TimeZoneDB_Not_Found", resourceCulture);
            }
        }
    }
}
