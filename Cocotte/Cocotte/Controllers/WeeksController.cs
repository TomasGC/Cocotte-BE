using System;
using System.Reflection;
using Cocotte.APIs;
using Cocotte.Managers;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using CoreLib.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Serilog;

namespace Cocotte.Controllers {
	/// <summary>
	/// The controller for the Weeks.
	/// </summary>
	[Produces("application/json")]
    [ApiController]
    [Route("weeks/api")]
    public class WeeksController : ControllerBase {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Authentication infos.
        /// </summary>
        AuthenticationResponse auth;

        /// <summary>
        /// Controller to get the auth infos.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public WeeksController(IHttpContextAccessor httpContextAccessor) => auth = Authentication.AuthenticateSession(httpContextAccessor.HttpContext.Request);

        /// <summary>
        /// Generate the next week of recipes.
        /// </summary>
        /// <tags>Weeks</tags>
        /// <returns></returns>
        [HttpGet("generate")]
        public BaseResponse Generate() {
            if (!auth.Success)
                return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
                WeeksApi.Generate(auth.User._id);

                return new BaseResponse() { Success = true };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

		/// <summary>
		/// Modify a day of the current week of recipes.
		/// </summary>
		/// <tags>Weeks</tags>
		/// <param name="request">The request containing the user id and the day to update.</param>
		/// <returns>The modified week.</returns>
		[HttpPut("update/day")]
        public GetWeek UpdateDay(Day request) {
            if (!auth.Success)
                return new GetWeek() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
                return new GetWeek() { Success = true, Week = WeeksApi.UpdateDay(auth.User._id, request) };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new GetWeek() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

		/// <summary>
		/// Modify the current week of recipes.
		/// </summary>
		/// <tags>Weeks</tags>
		/// <param name="request">The request containing the user id and the day to update.</param>
		/// <returns>The modified week.</returns>
		[HttpPut("update")]
        public GetWeek UpdateWeek(Weeks request) {
            if (!auth.Success)
                return new GetWeek() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
                return new GetWeek() { Success = true, Week = WeeksApi.Update(request) };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new GetWeek() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

        /// <summary>
        /// Delete a week of recipes.
        /// </summary>
        /// <tag>Weeks</tag>
        /// <param name="id">Id of the week to remove.</param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        public BaseResponse Delete(ObjectId id) {
            if (!auth.Success)
                return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
				WeeksApi.Delete(id);

                return new BaseResponse() { Success = true };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

		/// <summary>
		/// Get the current week.
		/// </summary>
		/// <tags>Weeks</tags>
		/// <returns></returns>
		[HttpGet("current")]
        public GetWeek GetWeek() {
            if (!auth.Success)
                return new GetWeek() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
                return new GetWeek() { Success = true, Week = WeeksApi.GetWeek(auth.User._id) };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new GetWeek() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

		/// <summary>
		/// Get the current week ingredients.
		/// </summary>
		/// <tags>Weeks</tags>
		/// <returns></returns>
		[HttpGet("current/ingredients/all")]
        public GetWeekIngredients GetWeekIngredients() {
            if (!auth.Success)
                return new GetWeekIngredients() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
                return new GetWeekIngredients() { Success = true, WeekIngredients = WeeksApi.GetWeekIngredients(auth.User._id) };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new GetWeekIngredients() { Success = false, Rescode = -1, Message = e.Message };
            }
        }

		/// <summary>
		/// Update the current week ingredients.
		/// </summary>
		/// <tags>Weeks</tags>
		/// <param name="request">The WeekIngredients to update.</param>
		/// <returns></returns>
		[HttpPut("current/ingredients/update")]
        public BaseResponse GetWeekIngredients(WeekIngredients request) {
            if (!auth.Success)
                return new GetWeekIngredients() { Success = false, Rescode = -100, Message = "Bad authentication !" };

            try {
				WeeksApi.UpdateWeekIngredients(request);

				return new BaseResponse() { Success = true };
            }
            catch (Exception e) {
                log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
                return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
            }
        }
    };
}
