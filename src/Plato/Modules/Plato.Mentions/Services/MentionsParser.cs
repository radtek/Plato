﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Mentions.Services
{
    
    public class MentionsParser : IMentionsParser
    {
        
        private const string SearchPattern = "@([^\\n|^\\s][a-z0-9\\-_\\/]*)";

        private readonly IContextFacade _contextFacade;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public MentionsParser(IPlatoUserStore<User> platoUserStore, IContextFacade contextFacade)
        {
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
        }
        
        #region "Implementation"

        public async Task<string> ParseAsync(string input)
        {
            var users = await GetUsersAsync(input);
            if (users != null)
            {
                input = await ConvertToUrlsAsync(input, users);
            }

            return input;

        }

        public async Task<IEnumerable<User>> GetUsersAsync(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var usernames = GetUniqueUsernames(input);
            if (usernames?.Length > 0)
            {
                return await GetUsersByUsernamesAsync(usernames.ToArray());
            }

            return null;

        }

        #endregion

        #region "Private Methods"
        
        async Task<string> ConvertToUrlsAsync(string input, IEnumerable<IUser> users)
        {

            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (users == null)
            {
                throw new ArgumentNullException(nameof(users));
            }

            var opts = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            var regex = new Regex(SearchPattern, opts);
            if (regex.IsMatch(input))
            {
                var userList = users.ToList();
                var baseUrl = await _contextFacade.GetBaseUrlAsync();
                foreach (Match match in regex.Matches(input))
                {
                    var username = match.Groups[1].Value;
                    var user = userList.FirstOrDefault(u => u.UserName.Equals(username, StringComparison.Ordinal));
                    if (user != null)
                    {
                        input = input.Replace(match.Value,
                            $"<a href=\"{baseUrl}/users/{user.Id}/{user.Alias}\">@{user.UserName}</a>");
                    }
                }
            }

            return input;

        }
        
        async Task<IEnumerable<User>> GetUsersByUsernamesAsync(string[] usernames)
        {
            var users = new List<User>();
            foreach (var username in usernames)
            {
                if (!String.IsNullOrEmpty(username))
                {
                    var user = await _platoUserStore.GetByUserNameAsync(username);
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
            }

            return users;

        }

        string[] GetUniqueUsernames(string input)
        {

            List<string> output = null;
            var regex = new Regex(SearchPattern, RegexOptions.IgnoreCase);
            if (regex.IsMatch(input))
            {
                output = new List<string>();
                foreach (Match match in regex.Matches(input))
                {
                    var username = match.Groups[1].Value;
                    if (!output.Contains(username))
                        output.Add(username);
                }
            }

            return output?.ToArray();

        }

        #endregion

    }

}