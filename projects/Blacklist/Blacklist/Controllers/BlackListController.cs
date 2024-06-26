﻿using Messaging;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlackDic.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class BlackListController : ControllerBase
    {
        private readonly MessageClient _messageClient;
        BlackListController(MessageClient messageClient)
        {
            _messageClient = messageClient;
        }

        [HttpPost]
        public IActionResult AddProfileToBlackDic(int profileID, int UserID)
        {
            if (!BlackDic.blackDic.ContainsKey(UserID))
            {
                Console.WriteLine($"User {UserID} does not have a blacklist creating a new one");
                BlackDic.blackDic.Add(UserID, new List<int>());
                _messageClient.Send<int>(profileID, "BlockedPersonAdded");
                Console.WriteLine($"BlackList created for {UserID}");
            }

            BlackDic.blackDic[UserID].Add(profileID);
            Console.WriteLine($"User: {UserID} is blacklisting this Profile {profileID}");
            return Ok();
        }

        [HttpGet]
        public IActionResult GetProfilesFromBlackDic(int UserID)
        { 
            Console.WriteLine($"Getting Blacklist for {UserID}");
            return Ok(BlackDic.blackDic.ContainsKey(UserID) ? BlackDic.blackDic[UserID] : new List<Guid>()); 
        }

        [HttpDelete]
        public IActionResult DeleteFromBlackDic(int profileID, int UserID)
        {
            if (BlackDic.blackDic.ContainsKey(UserID))
            {
                try
                {
                    Console.WriteLine($"Removing {profileID} from {UserID}'s blacklist");
                    BlackDic.blackDic[UserID].Remove(profileID);
                    _messageClient.Send<int>(profileID, "BlockedPersonRemoved");
                    return Ok();
                } catch (Exception ex)
                {
                    return BadRequest("Profile is not in the BlackList");
                }
            }
            return BadRequest("The User does not exist in our BlackList");
        }
    }
}
