using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HR.LeaveManagement.MVC.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly ILeaveTypeService leaveTypeService;
        private readonly ILeaveRequestService leaveRequestService;

        public LeaveRequestsController(ILeaveTypeService leaveTypeService, 
            ILeaveRequestService leaveRequestService)
        {
            this.leaveTypeService = leaveTypeService;
            this.leaveRequestService = leaveRequestService;
        }

        // GET: LeaveRequestsController
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            var model = await leaveRequestService.GetAdminLeaveRequestList();
            return View(model);
        }

        // GET: LeaveRequestsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var model = await leaveRequestService.GetLeaveRequest(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> ApproveRequest(int id, bool approved)
        {
            try
            {
                await leaveRequestService.ApproveLeaveRequest(id, approved);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                
            }
            return BadRequest();
        }

        // GET: LeaveRequestsController/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await leaveTypeService.GetLeaveTypes();
            var leaveTypeItems = new SelectList(leaveTypes, "Id", "Name");
            var model = new CreateLeaveRequestVM { LeaveTypes = leaveTypeItems };

            return View(model);
        }

        // POST: LeaveRequestsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM leaveRequest)
        {
            if (ModelState.IsValid)
            {
                var response = await leaveRequestService.CreateLeaveRequest(leaveRequest);
                if (response.Success)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", response.ValidationErrors);
            }

            var leaveTypes = await leaveTypeService.GetLeaveTypes();
            var leaveTypeItems = new SelectList(leaveTypes, "Id", "Name");
            leaveRequest.LeaveTypes = leaveTypeItems;

            return View(leaveRequest);
        }

        // GET: LeaveRequestsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
