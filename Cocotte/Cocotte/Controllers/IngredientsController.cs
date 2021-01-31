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
	/// The controller for the Ingredients.
	/// </summary>
	[Produces("application/json")]
	[ApiController]
	[Route("ingredients/api")]
	public class IngredientsController : ControllerBase {
		static readonly ILogger log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Authentication infos.
		/// </summary>
		AuthenticationResponse auth;

		/// <summary>
		/// Controller to get the auth infos.
		/// </summary>
		/// <param name="httpContextAccessor"></param>
		public IngredientsController(IHttpContextAccessor httpContextAccessor) => auth = Authentication.AuthenticateSession(httpContextAccessor.HttpContext.Request);

		/// <summary>
		/// Create an ingredient.
		/// </summary>
		/// <tags>Ingredients</tags>
		/// <param name="request">New ingredient to add.</param>
		/// <returns></returns>
		[HttpPost("create")]
		public BaseResponse Create(Ingredients request) {
			if (!auth.Success)
				return new BaseResponse { Success = false, Rescode = -400, Message = "Bad authentication." };

			try {
				IngredientsApi.Create(request);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Modify an ingredient.
		/// </summary>
		/// <tags>Ingredients</tags>
		/// <param name="request">Modified ingredient.</param>
		/// <returns></returns>
		[HttpPut("update")]
		public BaseResponse Update(Ingredients request) {
			if (!auth.Success)
				return new BaseResponse { Success = false, Rescode = -400, Message = "Bad authentication." };

			try {
				IngredientsApi.Update(request);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Delete an ingredient.
		/// </summary>
		/// <tags>Ingredients</tags>
		/// <param name="id">Id of the ingredient to remove.</param>
		/// <returns></returns>
		[HttpDelete("delete/{id}")]
		public BaseResponse Delete(ObjectId id) {
			if (!auth.Success)
				return new BaseResponse { Success = false, Rescode = -400, Message = "Bad authentication." };

			try {
				IngredientsApi.Delete(id);

				return new BaseResponse() { Success = true };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new BaseResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}

		/// <summary>
		/// Get all ingredients.
		/// </summary>
		/// <tags>Ingredients</tags>
		/// <returns></returns>
		[HttpGet("all")]
		public GetIngredientsResponse GetAll() {
			if (!auth.Success)
				return new GetIngredientsResponse { Success = false, Rescode = -400, Message = "Bad authentication." };

			try {
				return new GetIngredientsResponse() { Success = true, Ingredients = IngredientsApi.GetAll(auth.User._id) };
			}
			catch (Exception e) {
				log.Error($"Error [{e.Message}] - Stack [{e.StackTrace}].");
				return new GetIngredientsResponse() { Success = false, Rescode = -1, Message = e.Message };
			}
		}
	};
}
