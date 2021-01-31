using System;
using System.Reflection;
using Cocotte.APIs;
using Cocotte.Managers;
using Cocotte.MongoDb.Types;
using Cocotte.Types.Responses;
using Common.Request;
using CoreLib.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Serilog;

namespace Cocotte.Controllers {
	/// <summary>
	/// The controller for the Recipes.
	/// </summary>
	[Produces("application/json")]
	[ApiController]
	[Route("recipes/api")]
	public class RecipesController : ControllerBase {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Authentication infos.
		/// </summary>
		AuthenticationResponse auth;

		/// <summary>
		/// Controller to get the auth infos.
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		public RecipesController(IHttpContextAccessor httpContextAccessor) => auth = Authentication.AuthenticateSession(httpContextAccessor.HttpContext.Request);

		/// <summary>
		/// Create a recipe.
		/// </summary>
		/// <tags>Recipes</tags>
		/// <param name="request">New recipe to add.</param>
		/// <returns></returns>
		[HttpPost("create")]
		public BaseResponse Create(Recipes request) {
			if (!auth.Success)
				return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				RecipesApi.Create(request);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Modify a recipe.
		/// </summary>
		/// <tags>Recipes</tags>
		/// <param name="request">Modified recipe.</param>
		/// <returns></returns>
		[HttpPut("update")]
		public BaseResponse Update(Recipes request) {
			if (!auth.Success)
				return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				RecipesApi.Update(request);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Modify a list of recipes.
		/// </summary>
		/// <tags>Recipes</tags>
		/// <param name="request">Modified recipes.</param>
		/// <returns></returns>
		[HttpPost("update/list")]
		public BaseResponse Update(UpdateRecipesRequest request) {
			if (!auth.Success)
				return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				RecipesApi.Update(request.Recipes);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Delete a recipe.
		/// </summary>
		/// <tags>Recipes</tags>
		/// <param name="id">Id of the recipe to remove.</param>
		/// <returns></returns>
		[HttpDelete("delete/{id}")]
		public BaseResponse Delete(ObjectId id) {
			if (!auth.Success)
				return new BaseResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				RecipesApi.Delete(id);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Get all recipes for a given userId.
		/// </summary>
		/// <tags>Recipes</tags>
		/// <returns></returns>
		[HttpGet("all")]
		public GetRecipesResponse GetAll() {
			if (!auth.Success)
				return new GetRecipesResponse() { Success = false, Rescode = -100, Message = "Bad authentication !" };

			try {
				return new GetRecipesResponse() { Success = true, Recipes = RecipesApi.GetAll(auth.User._id) };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new GetRecipesResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}
	};
}
