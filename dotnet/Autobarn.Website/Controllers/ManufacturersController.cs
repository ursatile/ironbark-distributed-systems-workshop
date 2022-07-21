﻿using AutoMate.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Autobarn.Website.Controllers {
	public class ManufacturersController : Controller {
		private readonly IAutoMateDatabase db;

		public ManufacturersController(IAutoMateDatabase db) {
			this.db = db;
		}
		public IActionResult Index() {
			var vehicles = db.ListManufacturers();
			return View(vehicles);
		}

		public IActionResult Models(string id) {
			var manufacturer = db.ListManufacturers().FirstOrDefault(m => m.Code == id);
			return View(manufacturer);
		}
	}
}
