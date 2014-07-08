﻿namespace OctoHook.WebHooks
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

	[Component]
    public class AutoLink : IWebHook<IssuesEvent>
    {
		static readonly ITracer tracer = Tracer.Get<AutoLink>();
        static readonly Regex storyPrefixExpr = new Regex(@"\[[^\]]+\]", RegexOptions.Compiled);

		private IGitHubClient github;

		public AutoLink(IGitHubClient github)
		{
			this.github = github;
		}

		public async Task ProcessAsync(IssuesEvent @event)
		{
            var storyPrefix = storyPrefixExpr.Match(@event.Issue.Title);
            if (!storyPrefix.Success)
			{
				tracer.Verbose("Skipping issue #{0} without a story prefix: '{1}'.", @event.Issue.Number, @event.Issue.Title);
                return;
			}

            // Find the story with the same prefix.
			var story = await FindStoryAsync(@event.Repository.FullName, storyPrefix.Value);
			if (story == null)
			{
				tracer.Warn("Issue #{0} has story prefix '{1}' but no matching story was found with such prefix.",
					@event.Issue.Number, storyPrefix.Value);
				return;
			}

            // See if story link exists in the issue description.
            var issueLink = "#" + story.Number;
            if (@event.Issue.Body == null || !@event.Issue.Body.Contains(issueLink))
            {
                var update = new IssueUpdate
                {
                    Body = (@event.Issue.Body == null ? "" : @"

")
						+ "Story " + issueLink,
                    State = @event.Issue.State,
                };

				await github.Issue.Update(
					@event.Repository.Owner.Login, 
					@event.Repository.Name, 
					@event.Issue.Number, 
					update);
            }
			else
			{
				tracer.Verbose("Skipping issue #{0} since it already contains story link to #{1}.", @event.Issue.Number, issueLink);
			}
		}

		private async Task<Issue> FindStoryAsync(string repository, string query)
		{
			var story = await FindIssueAsync(repository, query, ItemState.Open, "Story");
			if (story == null)
				story = await FindIssueAsync(repository, query, ItemState.Closed, "Story");
			if (story == null)
				story = await FindIssueAsync(repository, query, ItemState.Open, "story");
			if (story == null)
				story = await FindIssueAsync(repository, query, ItemState.Closed, "story");

			return story;
		}

		private async Task<Issue> FindIssueAsync(string repository, string query, ItemState state, string label)
		{
            var stories = await github.Search.SearchIssues(new SearchIssuesRequest(query)
            {
                Labels = new[] { label },
                Repo = repository,
                Type = IssueTypeQualifier.Issue,
				State = state,
            });

			return stories.Items.FirstOrDefault();
		}
	}
}