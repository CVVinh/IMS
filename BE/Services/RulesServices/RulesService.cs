using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using BE.shared.Interface;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Tls;
using System;

namespace BE.Services.RulesServices
{
	public class RulesService : IRulesService
	{
		private readonly AppDbContext _db;
		private readonly IMapper _mapper;

		public RulesService(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<BaseResponse<Rules>> CreateRules(AddOrUpdateRulesDTO addRulesDto, string uploads, string localServer)
		{
			var success = false;
			var message = "";
			try
			{
				var rule = _mapper.Map<Rules>(addRulesDto);
				if (addRulesDto.formFile != null)
				{
					rule.pathFile = localServer + FilesHelper.UploadFileAndReturnPath(addRulesDto.formFile, uploads, "/UploadRules/");
				}
				rule.dateCreated = DateTime.Now;
				rule.userCreated = addRulesDto.idUser;
				await _db.Rules.AddAsync(rule);
				await _db.SaveChangesAsync();
				success = true;
				message = "Add new Rules successfully";
				return new BaseResponse<Rules>(success, message, rule);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Adding new Rules failed! {ex.Message}";
				return new BaseResponse<Rules>(success, message, new Rules());
			}
		}
		public async Task<BaseResponse<Rules>> UpdateRules(int id, AddOrUpdateRulesDTO updateRulesDto, string upload, string localServer)
		{
			var success = false;
			var message = "";
			var data = new Rules();
			try
			{
				var rule = await _db.Rules.Where(s => s.id.Equals(id))
					.FirstOrDefaultAsync();
				if (rule is null)
				{
					message = "Rules doesn't exist !";
					data = null;
					return new BaseResponse<Rules>(success, message, data);
				}
				var ruleMapdata = _mapper.Map<AddOrUpdateRulesDTO, Rules>(updateRulesDto, rule);
				if (updateRulesDto.formFile != null)
				{
					var fileName = rule.pathFile?.Replace($"{localServer}/UploadRules/", "");
					string filePath = System.IO.Path.Combine(upload, "UploadRules", fileName ?? "");
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
					ruleMapdata.pathFile = localServer + FilesHelper.UploadFileAndReturnPath(updateRulesDto.formFile, upload, "/UploadRules/");
				}
				ruleMapdata.dateUpdated = DateTime.Now;
				ruleMapdata.userUpdated = updateRulesDto.idUser;
				_db.Rules.Update(ruleMapdata);
				await _db.SaveChangesAsync();
				success = true;
				message = "Editing Rules successfully";
				data = ruleMapdata;
				return new BaseResponse<Rules>(success, message, data);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Editing Rules failed! {ex.Message}";
				return new BaseResponse<Rules>(success, message, data = null);
			}
		}
		public async Task<BaseResponse<Rules>> DeleteRules(int idRules, DeleteRulesDTO deleteRules)
		{
			var success = false;
			var message = "";
			var data = new Rules();
			try
			{
				var rule = await _db.Rules.Where(s => s.id.Equals(idRules)).FirstOrDefaultAsync();
				if (rule is null)
				{
					success = false;
					message = "Rules doesn't exist !";
					return new BaseResponse<Rules>(success, message, data = null);
				}
				rule.dateDeleted = DateTime.Now;
				rule.isDeleted = true;
				rule.userDeleted = deleteRules.idUser;
				_db.Rules.Update(rule);
				await _db.SaveChangesAsync();
				success = true;
				message = "Deleting Rules successfully";
				return new BaseResponse<Rules>(success, message, rule);
			}
			catch (Exception ex)
			{
				success = false;
				message = $"Deleting Rules failed! {ex.InnerException}";
				return new BaseResponse<Rules>(success, message, data = null);
			}
		}
		public async Task<BaseResponse<List<Rules>>> GetAllRulesAsync()
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules
					.Where(s => s.isDeleted == false)
					.OrderByDescending(s => s.dateCreated)
					.ToListAsync();

				success = true;
				message = "Get all data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}
		public async Task<BaseResponse<List<Rules>>> GetRulesWithUserId(int userId)
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules
					.OrderByDescending(s => s.dateCreated)
					.Where(s => s.isDeleted == false
							&& s.userCreated.Equals(userId))
					.ToListAsync();
				success = true;
				message = "Get data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}
		public async Task<BaseResponse<List<Rules>>> FindRulesByTitle(string searchTitle)
		{
			var success = false;
			var message = "";
			var data = new List<Rules>();
			try
			{
				var rule = await _db.Rules.Where(s =>
						s.title.ToLower().Contains(searchTitle.ToLower().Trim())
						&& s.isDeleted == false).ToListAsync();

				success = true;
				message = "Get all data successfully";
				data.AddRange(rule);
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
			catch (Exception ex)
			{
				success = false;
				message = ex.Message;
				return (new BaseResponse<List<Rules>>(success, message, data));
			}
		}
	}
}
