﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pustok2.Contexts;
using Pustok2.Helpers;
using Pustok2.Models;
using Pustok2.ViewModel.SettingVm;
using Pustok2.ViewModel.SliderVM;

namespace Pustok2.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class SettingController : Controller
    {
        PustokDbContext _db { get; }
        public SettingController(PustokDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Settings.Select(c => new SettingListVm
            {
                Id = c.Id,
                Number = c.Number,
                Email = c.Email,
                Address = c.Address,
                Logo = c.Logo,
            }).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SettingCreateVm vm)
        {
            if(vm.LogoFile != null)
            {
                if (!vm.LogoFile.IsCorrectType())
                {
                    ModelState.AddModelError("MainImage", "Wrong fie type");
                }
            }
            if(vm.LogoFile != null)
            {
                if (!vm.LogoFile.IsValidSize(200))
                {
                    ModelState.AddModelError("LogoFile", "File must be less than given kb ");
                }
            }
            Setting setting = new Setting 
            {
                Number = vm.Number,
                Email = vm.Email,
                Address = vm.Address,
                Logo = await vm.LogoFile.SaveAsync(PathConstants.Product),
            };
            await _db.Settings.AddAsync(setting);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            TempData["Response"] = false;
            if (id == null) return BadRequest();

            var data = await _db.Settings.FindAsync(id);
            if (data == null) return NotFound();
            _db.Settings.Remove(data);
            TempData["Response"] = true;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id <= 0) return BadRequest();
            var data = await _db.Settings.FindAsync(id);
            if (data == null) return NotFound();
            return View(new SettingUptadeVm
            {
                Number = data.Number,
                Logo = data.Logo,
                Email = data.Email,
                Address = data.Address
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, SettingUptadeVm vm)
        {
            if (id == null || id <= 0) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(vm);

            }
            var data = await _db.Settings.FindAsync(id);
            if (data == null) return NotFound();

            if (data.Logo != vm.Logo || data.Number != vm.Number || data.Address!= vm.Address|| data.Email != vm.Email)
            {
                data.Logo = vm.Logo;
                data.Number = vm.Number;
                data.Address = vm.Address;
                data.Email = vm.Email;
                await _db.SaveChangesAsync();
                TempData["Salam"] = true;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Salam"] = false;
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
