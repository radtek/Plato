﻿using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Abstractions.Query;
using Plato.Models.Users;
using Plato.Stores.Users;

namespace Plato.WebApi.Controllers
{
    public class UsersController : Controller
    {
        private readonly IPlatoUserStore _userStore;

        public UsersController(IPlatoUserStore userStore)
        {
            _userStore = userStore;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(
            int page = 1, 
            int pageSize = 5,
            string sortBy = "Id",
            OrderBy sortOrder = OrderBy.Asc)
        {

            var users = await _userStore.QueryAsync()
                .Page(page, pageSize)
                .Select<UserQueryParams>(q =>
                {
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy(sortBy, sortOrder)
                .ToList<User>();
            
            return new ObjectResult(new
            {
                users,
                StatusCode = HttpStatusCode.OK,
                Message = "Album created successfully."
            });
        }
    }
}