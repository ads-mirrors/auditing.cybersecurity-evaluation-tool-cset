﻿using System;
using CSETWebCore.Api.Models;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Helpers;
using CSETWebCore.Interfaces.Notification;
using CSETWebCore.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Collections.Generic;
using System.Linq;
using CSETWebCore.Business.Authorization;
using CSETWebCore.Interfaces.Helpers;

namespace CSETWebCore.Api.Controllers
{
    [ApiController]
    [CsetAuthorize]
    public class UserController : ControllerBase
    {
        private readonly CSETContext _context;
        private readonly IUserBusiness _userBusiness;
        private readonly INotificationBusiness _notificationBusiness;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public UserController(CSETContext context,
            IUserBusiness userBusiness,
            INotificationBusiness notificationBusiness,
            IConfiguration configuration, ITokenManager tokenManager)
        {
            _context = context;
            _userBusiness = userBusiness;
            _notificationBusiness = notificationBusiness;
            _configuration = configuration;
            _tokenManager = tokenManager;
            
        }


        /// <summary>
        /// Returns a collection of the USERS in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users")]
        public IActionResult GetUsers([FromQuery] bool? onlyInactive, [FromQuery] string apiKey)
        {           
            if (!IsApiKeyValid(apiKey))
            {
                return Unauthorized();
            }

            var resp = new List<UserAdmin>();


            var query = _context.USERS.AsQueryable();

            // the consumer can limit the response to inactive users only
            if (onlyInactive ?? false)
            {
                query =  _context.USERS.Where(x => !x.IsActive);
            }

            query.ToList().ForEach(u => 
            {
                var user = new UserAdmin() {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PrimaryEmail = u.PrimaryEmail,
                    IsActive = u.IsActive
                };

                resp.Add(user);
            });

            return Ok(resp);
        }


        /// <summary>
        /// Flip the IsActive flag on a USER.  
        /// When the flag is flipped to TRUE, send the user a temp password.
        /// 
        /// Randy's note:  I wrote this as a GET to quickly get the parms
        /// into the method signature.  If we want to do it as a POST we can 
        /// refactor them into their own object.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/activate")]
        public IActionResult ChangeUserActivation(
            [FromQuery] int userId, [FromQuery] bool isActive, [FromQuery] string apiKey)
        {
            LogManager.GetCurrentClassLogger().Info($"ChangeUserActivation:  changing isActive property to {isActive}");

            if (!IsApiKeyValid(apiKey))
            {
                return Unauthorized();
            }

            var user = _context.USERS.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return BadRequest();
            }


            // return if no change
            if (isActive == user.IsActive)
            {
                return Ok();
            }


            user.IsActive = isActive;
            _context.SaveChanges();


            // if the user is being activated, send them a new temp password
            if (isActive)
            {
                LogManager.GetCurrentClassLogger().Info($"ChangeUserActivation:  sending temporary password email to {user.PrimaryEmail}");

                var resetter = new UserAccountSecurityManager(_context, _userBusiness, _notificationBusiness, _configuration);
                resetter.ResetPassword(user.PrimaryEmail, "Temporary Password", "CSET");
            }


            // TODO:  What sort of response should we send?
            return Ok();
        }


        /// <summary>
        /// Checks the specified API Key against the secret in the DB.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        private bool IsApiKeyValid(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            var gp = new CSETGlobalProperties(_context);
            var secret = gp.GetProperty("UserApprovalApiKey");
            return (apiKey == secret);
        }
        
        /// <summary>
        /// Checks for current user role.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getRole")]
        public IActionResult GetRole()
        {
            try
            {
                var userId = _tokenManager.GetCurrentUserId();
                var role = _userBusiness.GetRole(userId);
                return Ok(new { Role = role });
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"... {exc}");
            }

            return Ok(); 
        }
        
        /// <summary>
        /// Grabs all users from the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getusers")]
        public IActionResult GetAllUsers()
        {           
            try
            {
                var users = _userBusiness.GetUsers();
                return Ok(users);
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"... {exc}");
            }

            return Ok();
        }
        
        /// <summary>
        /// Updates user role.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/updateuser")]
        public IActionResult UpdateUserRole([FromBody] UserRole user)
        {
            try
            {
                _userBusiness.UpdateRole(user.RoleId, user.UserId);
                return Ok();
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"... {exc}");
            }

            return Ok(); 
        }
        
        /// <summary>
        /// Grabs all current roles from the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getavailableroles")]
        public IActionResult GetAvailableRoles()
        {
            try
            {
                return Ok(_userBusiness.GetAvailableRoles());
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"... {exc}");
            }

            return Ok(); 
        }
    }
}
