﻿namespace OctoHook
{
	using Octokit;
	using Octokit.Events;
	using System;
	using System.Text.RegularExpressions;
	using System.Linq;
	using System.Collections.ObjectModel;
	using OctoHook.CommonComposition;
	using System.Threading.Tasks;
	using OctoHook.Diagnostics;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using OctoHook.Properties;

	[Component]
	public class AutoLink : IOctoIssuer
	{
		static readonly ITracer tracer = Tracer.Get<AutoLink>();
		static readonly Regex storyPrefixExpr = new Regex(@"\[[^\]]+\]", RegexOptions.Compiled);
		static readonly Regex issueLinkExpr = new Regex(@"(?<=\#)\d+", RegexOptions.Compiled);

		private IGitHubClient github;
		// Since this component is per-request, we just keep in-memory track of what we've 
		// already processed to short-circuit on Process.
		private HashSet<IssuesEvent> processed = new HashSet<IssuesEvent>();

		public AutoLink(IGitHubClient github)
		{
			this.github = github;
		}

		public bool Process(IssuesEvent issue, IssueUpdate update)
		{
			if (processed.Contains(issue))
				return false;

			var updated = ProcessAsync(issue, update).Result;
			processed.Add(issue);

			return updated;
		}

		private async Task<bool> ProcessAsync(IssuesEvent issue, IssueUpdate update)
		{
			var storyPrefix = storyPrefixExpr.Match(issue.Issue.Title);
			if (!storyPrefix.Success)
			{
				tracer.Verbose("Skipping issue {0}/{1}#{2} without a story prefix: '{3}'.",
					issue.Repository.Owner.Login,
					issue.Repository.Name,
					issue.Issue.Number,
					issue.Issue.Title);
				return false;
			}

			// Skip issues that are the story itself.
			if (issue.Issue.Labels != null &&
				issue.Issue.Labels.Any(l => string.Equals(l.Name, "story", StringComparison.OrdinalIgnoreCase)))
				return false;

			// Skip the issue if it already has a story link
			if (!string.IsNullOrEmpty(issue.Issue.Body))
			{
				foreach (var number in issueLinkExpr.Matches(issue.Issue.Body).OfType<Match>().Where(m => m.Success).Select(m => int.Parse(m.Value)))
				{
					try
					{
						var linkedIssue = await github.Issue.Get(issue.Repository.Owner.Login, issue.Repository.Name, number);
						if (linkedIssue.Labels.Any(l => string.Equals(l.Name, "story", StringComparison.OrdinalIgnoreCase)))
						{
							tracer.Info("Skipping issue {0}/{1}#{2} as it already contains story link to #{3}.",
								issue.Repository.Owner.Login,
								issue.Repository.Name,
								issue.Issue.Number,
								number);
							return false;
						}
					}
					catch (NotFoundException)
					{
						// It may be a link to a bug/issue in another system.
					}
				}
			}

			// Find the story with the same prefix.
			var repository = issue.Repository.Owner.Login + "/" + issue.Repository.Name;
			var story = await FindOpenStoryByPrefixAsync(repository, storyPrefix.Value);
			if (story == null || story.State == ItemState.Closed)
			{
				tracer.Warn("Issue {0}/{1}#{2} has story prefix '{3}' but no matching opened issue with the label 'Story' or 'story' was found with such prefix.",
					issue.Repository.Owner.Login,
					issue.Repository.Name,
					issue.Issue.Number,
					storyPrefix.Value);
				return false;
			}

			update.State = issue.Issue.State;
			update.Body = (issue.Issue.Body == null ? "" : issue.Issue.Body + @"

")
					+ "Story #" + story.Number;

			tracer.Info("Established new story link between issue {0}/{1}#{2} and story #{3}.",
				issue.Repository.Owner.Login,
				issue.Repository.Name,
				issue.Issue.Number,
				story.Number);

			return true;
		}

		private async Task<Issue> FindOpenStoryByPrefixAsync(string repository, string storyPrefix)
		{
			var story = await FindOpenStoryByPrefixAsync(repository, storyPrefix, ItemState.Open, "Story");
			if (story == null)
				story = await FindOpenStoryByPrefixAsync(repository, storyPrefix, ItemState.Open, "story");
			
			// Finding closed stories caused those stories to be re-opened, which wasn't 
			// all that useful. So we stop looking for closed stories now.
			//if (story == null)
			//	story = await FindIssueAsync(repository, query, ItemState.Closed, "Story");
			//if (story == null)
			//	story = await FindIssueAsync(repository, query, ItemState.Closed, "story");

			return story;
		}

		private async Task<Issue> FindOpenStoryByPrefixAsync(string repository, string storyPrefix, ItemState state, string label)
		{
			tracer.Verbose("Querying for '{0}' on repo '{1}' with state '{2}' and label '{3}'.",
				storyPrefix, repository, state, label);

			var stories = await github.Search.SearchIssues(new SearchIssuesRequest(storyPrefix)
			{
				Labels = new[] { label },
				Repo = repository,
				Type = IssueTypeQualifier.Issue,
				State = state,
				// Always point to newest found first.
				Order = SortDirection.Descending,
				// Restrict search to title only
				In = new[] { IssueInQualifier.Title }
			});

			tracer.Verbose("Results: {0}.", stories.TotalCount);

			// We need an extra safeguard here since the enclosing square brackets are 
			// ignored by GitHub, so we need to filter the results again.
			return stories.Items.FirstOrDefault(x => x.Title.StartsWith(storyPrefix));
		}
	}
}