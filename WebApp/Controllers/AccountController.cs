using System.Security.Claims;
using Infrastructure.Contexts;
using Infrastructure.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
	[Authorize]
	public class AccountController(UserManager<UserEntity> userManager, WebAppContext context) : Controller
	{
		private readonly UserManager<UserEntity> _userManager = userManager;
		private readonly WebAppContext _context = context;

		public async Task<IActionResult> Details()
		{
			var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
			var user = await _context.Users.Include(i => i.Address).FirstOrDefaultAsync(x => x.Id == nameIdentifier);

			var viewModel = new AccounDetailsViewModel
			{
				Basic = new AccountBasicInfo
				{
					FirstName = user!.FirstName,
					LastName = user!.LastName,
					Email = user!.Email!,
					PhoneNumber = user!.PhoneNumber,
					Bio = user!.Bio,

				},
				Address = new AccountAddressInfo
				{
					AddressLine_1 = user.Address?.AddressLine_1!,
					AddressLine_2 = user.Address?.AddressLine_2!,
					PostalCode = user.Address?.PostalCode!,
					City = user.Address?.City!,
				}
			};

			return View(viewModel);

		}

		[HttpPost]
		public async Task<IActionResult> UpdateBasicInfo(AccounDetailsViewModel model)
		{
			if (TryValidateModel(model.Basic!))
			{
				var user = await _userManager.GetUserAsync(User);
				if (user != null)
				{
					user.FirstName = model.Basic!.FirstName;
					user.LastName = model.Basic!.LastName;
					user.Email = model.Basic!.Email;
					user.PhoneNumber = model.Basic!.PhoneNumber;
					user.UserName = model.Basic!.Email;
					user.Bio = model.Basic!.Bio;


					var result = await _userManager.UpdateAsync(user);
					if (result.Succeeded)
					{
						TempData["StatusMessage"] = "Updatd basic information successully";

					}
					else
					{
						TempData["StatusMessage"] = "Unable to save basic information";
					}
				}
			}
			else
			{
				TempData["StatusMessage"] = "Unable to save basic information";

			}

			return RedirectToAction("Details", "Account");
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAddressInfo(AccounDetailsViewModel model)
		{

			if (TryValidateModel(model.Address!))
			{
				var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
				var user = await _context.Users.Include(i => i.Address).FirstOrDefaultAsync(x => x.Id == nameIdentifier);
				if (user != null)
				{
					try
					{
						if (user.Address != null)
						{
							user.Address.AddressLine_1 = model.Address!.AddressLine_1;
							user.Address.AddressLine_2 = model.Address!.AddressLine_2;
							user.Address.PostalCode = model.Address!.PostalCode;
							user.Address.City = model.Address!.City;
						}
						else
						{
							user.Address = new AddressEntity
							{
								AddressLine_1 = model.Address!.AddressLine_1,
								AddressLine_2 = model.Address!.AddressLine_2,
								PostalCode = model.Address!.PostalCode,
								City = model.Address!.City
							};
						}


						_context.Update(user);
						await _context.SaveChangesAsync();

						TempData["StatusMessage"] = "Updatd address info successully";

					}
					catch

					{
						TempData["StatusMessage"] = "Unable to save address information";
					}

				}
			}
			else
			{
				TempData["StatusMessage"] = "Unable to save basic address information";

			}

			return RedirectToAction("Details", "Account");
		}


		[HttpPost]
		public async Task<IActionResult> UploadProfileImage(IFormFile file)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user != null && file != null && file.Length != 0)
			{
				var fileName = $"p_{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/profiles", fileName);

				using var fs = new FileStream(filePath, FileMode.Create);
				await file.CopyToAsync(fs);

				user.ProfileImage = fileName;
				await _userManager.UpdateAsync(user);
			}
			else
			{
				TempData["StatusMessage"] = "Unable to upload profile images";

			}

			return RedirectToAction("Details", "Account");
		}
	}

}
