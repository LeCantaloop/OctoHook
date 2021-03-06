//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Globalization;

namespace OctoHook.Properties
{
	///	<summary>
	///	Provides access to string resources.
	///	</summary>
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("netfx-System.Strings", "1.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	static partial class Strings
	{
		/// <summary>
		/// Looks up a localized string similar to: 
		///	
		///	        
		///	<!-- This section was generated by OctoHook v{version} -->
		///	> {note} <a href="{wiki}" title="{title}"><img src="http://goo.gl/8P3okW" /></a>
		///	
		///	
		/// </summary>
		public static string FormatHeader(object version, object note, object wiki, object title)
		{
			return Resources.FormatHeader.FormatWith(new 
			{
				version = version,
				note = note,
				wiki = wiki,
				title = title,
			});
		}
	
		/// <summary>
		/// Looks up a localized string similar to: 
		///	- [{check}] {issue} {title}
		/// </summary>
		public static string FormatTask(object check, object issue, object title)
		{
			return Resources.FormatTask.FormatWith(new 
			{
				check = check,
				issue = issue,
				title = title,
			});
		}
	
		/// <summary>
		/// Looks up a localized string similar to: 
		///	This task list is updated automatically when the referenced issues are closed or reopened, via
		/// </summary>
		public static string Note { get { return Resources.Note; } }
	
		/// <summary>
		/// Looks up a localized string similar to: 
		///	Learn more about OctoHook's AutoTask
		/// </summary>
		public static string Title { get { return Resources.Title; } }
	
		/// <summary>
		/// Looks up a localized string similar to: 
		///	http://goo.gl/iB4ZFR
		/// </summary>
		public static string Wiki { get { return Resources.Wiki; } }
		
		///	<summary>
		///	Provides access to string resources.
		///	</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("netfx-System.Strings", "1.0.0.0")]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public static partial class Trace
		{
			/// <summary>
			/// Looks up a localized string similar to: 
			///	Created new task list and added new link: {taskLink}.
			/// </summary>
			public static string AddedLinkInNewList(object taskLink)
			{
				return Resources.Trace_AddedLinkInNewList.FormatWith(new 
				{
					taskLink = taskLink,
				});
			}
		
			/// <summary>
			/// Looks up a localized string similar to: 
			///	Processing found link to {linkedOwner}/{linkedRepo}#{linkedNumer} from body of {issueOwner}/{issueRepo}#{issueNumber}.
			/// </summary>
			public static string FoundLinkInBody(object linkedOwner, object linkedRepo, object linkedNumer, object issueOwner, object issueRepo, object issueNumber)
			{
				return Resources.Trace_FoundLinkInBody.FormatWith(new 
				{
					linkedOwner = linkedOwner,
					linkedRepo = linkedRepo,
					linkedNumer = linkedNumer,
					issueOwner = issueOwner,
					issueRepo = issueRepo,
					issueNumber = issueNumber,
				});
			}
		
			/// <summary>
			/// Looks up a localized string similar to: 
			///	Inserted new link in existing task list: {taskLink}.
			/// </summary>
			public static string InsertedLinkInExistingList(object taskLink)
			{
				return Resources.Trace_InsertedLinkInExistingList.FormatWith(new 
				{
					taskLink = taskLink,
				});
			}
		
			/// <summary>
			/// Looks up a localized string similar to: 
			///	Found existing link in task list. Updating with entry: {taskLink}.
			/// </summary>
			public static string UpdatedExistingLink(object taskLink)
			{
				return Resources.Trace_UpdatedExistingLink.FormatWith(new 
				{
					taskLink = taskLink,
				});
			}
		}
	}
}

